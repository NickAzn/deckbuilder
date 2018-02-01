﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public SpotStats[] playerSpots;		// Set in editor, all player spots (6)
	public SpotStats[] enemySpots;		// Set in editor, all enemy spots (6)
	public Crystal[] playerCrystals;	// Set in editor, all player crystals (3)
	public Crystal[] enemyCrystals;		// Set in editor, all enemy crystals (3)

	public Player player;		// Set in editor to the object with Player script
	public EnemyAI enemy;		// Set in editor to the object running EnemyAI script

	public Card selectedCard;
	public GameObject zoomCard;		// Set in editor to a large Card for readability purposes
	Card zoomCardStats;
	public GameObject endTurnButton;	// Set in editor to button with GameManager.EndTurn()

	bool playerTurn = true;

	public Text endText;				// Set in editor to UI Text to display game over text
	public GameObject restartButton;	// Set in editor to button with GameManager.Restart()

	void Start() {
		// Set player crystal health to the saved value
		int[] crystalHealth = SaveLoad.LoadCrystalHealth ();
		playerCrystals [0].SetHealth (crystalHealth[0]);
		playerCrystals [1].SetHealth (crystalHealth[1]);
		playerCrystals [2].SetHealth (crystalHealth[2]);

		// Set timeScale to 1
		Time.timeScale = 1.0f;
		// Hide the zoomCard
		zoomCardStats = zoomCard.GetComponent<Card> ();
		HideZoomCard ();
		// Sets enemy AI aggro to a random value to determine how aggressive the AI will play
		enemy.aggroMeter = Random.Range (0, 4);
		// Hides the restart button
		restartButton.SetActive (false);
	}

	//Reload the scene
	public void Restart() {
		SceneManager.LoadScene ("GameBoard");
	}

	//Hide the zoom card
	public void HideZoomCard() {
		zoomCard.SetActive (false);
	}

	//Show the zoom card as the given card
	public void ShowZoomCard(Card card) {
		zoomCardStats.CopyStats (card);
		zoomCard.SetActive (true);
	}

	//Allows other scripts to see if it is the player's turn
	public bool isPlayerTurn() {
		return playerTurn;
	}

	//Checks if either side has all crystals destroyed
	void CheckGameEnded() {
		bool playerAlive = false;
		for (int i = 0; i < playerCrystals.Length; i++) {
			if (playerCrystals [i].isAlive ()) {
				playerAlive = true;
			}
		}
		//If all player crystals are destroyed, the player loses and game resets
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
		//If all enemy crystals are destroyed, the player wins and continues
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
		//Check if the game ended after crystals take damage
		CheckGameEnded ();
	}

	//Hit crystal with parameter for specific amount of damage
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
		CheckGameEnded ();
	}

	// Ends turn
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
			if (attackingSpots [i].hasUnit && attackingSpots[i].damage > 0) {		//Check if the attackingSpot has a unit with damage > 0
				attackingSpots [i].unitAnimation.Play ("Attack");					// If it does, play attack animation and wait for it to finish
				yield return new WaitForSeconds (0.7f);
				if (attackingSpots [i].collumn == 1) {		// If the unit is in collumn one, attack the enemy at i and i + 3, or the crystal
					if (defendingSpots [i].hasUnit) {
						if (attackingSpots [i].damage > defendingSpots [i].health && attackingSpots[i].relentless) {	// If unit has relentless, keep attacking with remaining damage
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
				} else {									// If the unit is in collumn 2, attack enemy at i-3 and i, or the crystal
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
				if (attackingSpots [i].fury > 0) {
					attackingSpots [i].fury--;
					i--;
				}
			}
		}
		// Start the new turn after attacking
		if (startedPlayerTurn) {
			foreach (SpotStats spot in enemySpots) {
				spot.NewTurn ();
			}
			enemy.NewTurn ();
		} else {
			foreach (SpotStats spot in playerSpots) {
				spot.NewTurn ();
			}

			endTurnButton.SetActive (true);
			playerTurn = true;
			player.NewTurn ();
		}
	}
}
