using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public SpotStats[] playerSpots;
	public SpotStats[] enemySpots;
	public Crystal[] playerCrystals;
	public Crystal[] enemyCrystals;

	public Player player;
	public EnemyAI enemy;

	public Card selectedCard;
	public GameObject zoomCard;
	Card zoomCardStats;
	public GameObject endTurnButton;

	bool playerTurn = true;

	public Text endText;
	public GameObject restartButton;

	void Start() {
		int[] crystalHealth = SaveLoad.LoadCrystalHealth ();
		playerCrystals [0].SetHealth (crystalHealth[0]);
		playerCrystals [1].SetHealth (crystalHealth[1]);
		playerCrystals [2].SetHealth (crystalHealth[2]);

		Time.timeScale = 1.0f;
		zoomCardStats = zoomCard.GetComponent<Card> ();
		HideZoomCard ();
		enemy.aggroMeter = Random.Range (0, 4);
		restartButton.SetActive (false);
	}

	public void Restart() {
		SceneManager.LoadScene ("GameBoard");
	}

	public void HideZoomCard() {
		zoomCard.SetActive (false);
	}

	public void ShowZoomCard(Card card) {
		zoomCardStats.CopyStats (card);
		zoomCard.SetActive (true);
	}

	public bool isPlayerTurn() {
		return playerTurn;
	}

	void CheckGameEnded() {
		bool playerAlive = false;
		for (int i = 0; i < playerCrystals.Length; i++) {
			if (playerCrystals [i].isAlive ()) {
				playerAlive = true;
			}
		}
		if (!playerAlive) {
			SaveLoad.ResetCrystalHealth ();
			SaveLoad.ResetPlayerDeck ();
			endText.text = "Defeat";
			restartButton.SetActive (true);
			Time.timeScale = 0.0f;
		}

		bool enemyAlive = false;
		for (int i = 0; i < enemyCrystals.Length; i++) {
			if (enemyCrystals [i].isAlive ()) {
				enemyAlive = true;
			}
		}
		if (!enemyAlive) {
			SaveLoad.SaveCrystalHealth (playerCrystals [0].GetHealth(), playerCrystals [1].GetHealth(), playerCrystals [2].GetHealth());
			endText.text = "Victory!";
			restartButton.SetActive (true);
			Time.timeScale = 0.0f;
		}
	}

	//Unit at playerSpots[i] hits the crystal in the same row
	void HitCrystal(int unitIteration, bool hitEnemySide) {
		if (hitEnemySide) {
			if (playerSpots [unitIteration].row == 1) {
				enemyCrystals [0].TakeDamage (playerSpots [unitIteration].damage);
			} else if (playerSpots [unitIteration].row == 2) {
				enemyCrystals [1].TakeDamage (playerSpots [unitIteration].damage);
			} else if (playerSpots [unitIteration].row == 3) {
				enemyCrystals [2].TakeDamage (playerSpots [unitIteration].damage);
			}
		} else {
			if (enemySpots [unitIteration].row == 1) {
				playerCrystals [0].TakeDamage (enemySpots [unitIteration].damage);
			} else if (playerSpots [unitIteration].row == 2) {
				playerCrystals [1].TakeDamage (enemySpots [unitIteration].damage);
			} else if (playerSpots [unitIteration].row == 3) {
				playerCrystals [2].TakeDamage (enemySpots [unitIteration].damage);
			}
		}
		CheckGameEnded ();
	}

	void HitCrystal(int unitIteration, bool hitEnemySide, int damage) {
		if (hitEnemySide) {
			if (playerSpots [unitIteration].row == 1) {
				enemyCrystals [0].TakeDamage (damage);
			} else if (playerSpots [unitIteration].row == 2) {
				enemyCrystals [1].TakeDamage (damage);
			} else if (playerSpots [unitIteration].row == 3) {
				enemyCrystals [2].TakeDamage (damage);
			}
		} else {
			if (enemySpots [unitIteration].row == 1) {
				playerCrystals [0].TakeDamage (damage);
			} else if (playerSpots [unitIteration].row == 2) {
				playerCrystals [1].TakeDamage (damage);
			} else if (playerSpots [unitIteration].row == 3) {
				playerCrystals [2].TakeDamage (damage);
			}
		}
	}

	public void EndTurn() {
		selectedCard = null;
		if (playerTurn) {
			//Player attack
			StartCoroutine(UnitsAttack(playerSpots, enemySpots));
		} else {
			//Enemy Attack
			StartCoroutine(UnitsAttack(enemySpots, playerSpots));
		}
	}

	//Causes units to attack starting from front top row, front collumn, to bottom row, back collumn
	IEnumerator UnitsAttack(SpotStats[] attackingSpots, SpotStats[] defendingSpots) {
		bool startedPlayerTurn = playerTurn;
		if (startedPlayerTurn) {
			endTurnButton.SetActive (false);
			playerTurn = false;
		}
		for (int i = 0; i < attackingSpots.Length; i++) {
			if (attackingSpots [i].hasUnit && attackingSpots[i].damage > 0) {
				attackingSpots [i].unitAnimation.Play ("Attack");
				yield return new WaitForSeconds (0.7f);
				if (attackingSpots [i].collumn == 1) {
					if (defendingSpots [i].hasUnit) {
						if (attackingSpots [i].damage > defendingSpots [i].health && attackingSpots[i].relentless) {
							if (attackingSpots [i].damage > (defendingSpots [i+3].health + defendingSpots [i].health) 
								&& attackingSpots[i].relentless) {
								HitCrystal (i, startedPlayerTurn, 
									attackingSpots[i].damage - defendingSpots[i+3].health - defendingSpots[i].health);
							}
							defendingSpots [i+3].TakeDamage (attackingSpots [i].damage - defendingSpots [i].health);
						}
						attackingSpots [i].Attack (defendingSpots [i]);
					} else if (defendingSpots [i + 3].hasUnit) {
						if (attackingSpots [i].damage > defendingSpots [i+3].health && attackingSpots[i].relentless) {
							HitCrystal (i, startedPlayerTurn, attackingSpots[i].damage - defendingSpots[i+3].health);
						}
						attackingSpots [i].Attack (defendingSpots [i + 3]);
					} else {
						HitCrystal (i, startedPlayerTurn);
					}
				} else {
					if (defendingSpots [i - 3].hasUnit) {
						if (attackingSpots [i].damage > defendingSpots [i-3].health && attackingSpots[i].relentless) {
							if (attackingSpots [i].damage > (defendingSpots [i-3].health + defendingSpots [i].health) 
								&& attackingSpots[i].relentless) {
								HitCrystal (i, startedPlayerTurn, 
									attackingSpots[i].damage - defendingSpots[i-3].health - defendingSpots[i].health);
							}
							defendingSpots [i].TakeDamage (attackingSpots [i].damage - defendingSpots [i-3].health);
						}
						attackingSpots [i].Attack (defendingSpots [i - 3]);
					} else if (defendingSpots [i].hasUnit) {
						if (attackingSpots [i].damage > defendingSpots [i].health && attackingSpots[i].relentless) {
							HitCrystal (i, startedPlayerTurn, attackingSpots[i].damage - defendingSpots[i].health);
						}
						attackingSpots [i].Attack (defendingSpots [i]);
					} else {
						HitCrystal (i, startedPlayerTurn);
					}
				}
			}
		}
		CheckGameEnded ();
		if (startedPlayerTurn) {
			enemy.NewTurn ();
		} else {
			endTurnButton.SetActive (true);
			playerTurn = true;
			player.NewTurn ();
		}
	}
}
