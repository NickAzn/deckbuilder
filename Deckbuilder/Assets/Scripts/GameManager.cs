using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public SpotStats[] playerSpots;		// Set in editor, all player spots (6)
	public SpotStats[] enemySpots;		// Set in editor, all enemy spots (6)
	public Crystal[] playerCrystals;	// Set in editor, all player crystals (3)
	public Crystal[] enemyCrystals;		// Set in editor, all enemy crystals (3)
	public GameObject[] enemyAIs;		// Set in editor, all enemies for different stages.
	public GameObject[] bossEnemyAIs;	// Set in editor, all enemy bosses for different stages

	public Player player;		// Set in editor to the object with Player script
	public EnemyAI enemy;		// Object running EnemyAI script

	public Card selectedCard;
	public GameObject zoomCard;		// Set in editor to a large Card for readability purposes
	public GameObject[] zoomCardEffects;	// Set in editor to effect tabs on large Card for clarity of '*' stats
	Card zoomCardStats;
	public GameObject endTurnButton;	// Set in editor to button with GameManager.EndTurn()

	bool playerTurn = true;

	public Text endText;				// Set in editor to UI Text to display game over text
	public GameObject restartButton;	// Set in editor to button with GameManager.Restart()

	public GameObject rewardScreen;
	public GameObject rewardCard1;
	public GameObject rewardCard2;
	bool gameEnded = false;

	void Start() {
		// Set player crystal health to the saved value
		int[] crystalHealth = SaveLoad.LoadCrystalHealth ();
		playerCrystals [0].SetHealth (crystalHealth[0]);
		playerCrystals [1].SetHealth (crystalHealth[1]);
		playerCrystals [2].SetHealth (crystalHealth[2]);

		// Hide the zoomCard
		zoomCardStats = zoomCard.GetComponent<Card> ();
		HideZoomCard ();

		int[] stageLoadout = SaveLoad.LoadStage ();
		if (stageLoadout != null) {
			if (stageLoadout [3] == stageLoadout [1]) {
				enemy = Instantiate (bossEnemyAIs [stageLoadout [0]]).GetComponent<EnemyAI> ();
			} else {
				enemy = Instantiate (enemyAIs [stageLoadout [0]]).GetComponent<EnemyAI> ();
			}
			if (stageLoadout [0] == 0) {
				enemyCrystals [0].SetHealth (6);
				enemyCrystals [1].SetHealth (6);
				enemyCrystals [2].SetHealth (6);
			} else if (stageLoadout [0] == 1) {
				enemyCrystals [0].SetHealth (7);
				enemyCrystals [1].SetHealth (10);
				enemyCrystals [2].SetHealth (7);
			} else if (stageLoadout [0] == 2) {
				enemyCrystals [0].SetHealth (10);
				enemyCrystals [1].SetHealth (7);
				enemyCrystals [2].SetHealth (10);
			}
		} else {
			enemy = Instantiate (enemyAIs [0]).GetComponent<EnemyAI> ();
			enemyCrystals [0].SetHealth (6);
			enemyCrystals [1].SetHealth (6);
			enemyCrystals [2].SetHealth (6);
		}
		// Sets enemy AI aggro to a random value to determine how aggressive the AI will play
		enemy.aggroMeter = Random.Range (0, 4);


		// Hides the restart button
		restartButton.SetActive (false);
		rewardScreen.SetActive (false);
	}

	//loads a new scene
	public void LoadScene(string name) {
		Time.timeScale = 1.0f;
		SceneManager.LoadScene (name);
	}

	//Hide the zoom card
	public void HideZoomCard() {
		zoomCard.SetActive (false);
		foreach (GameObject gm in zoomCardEffects) {
			gm.SetActive (false);
		}
	}

	//Show the zoom card as the given card
	public void ShowZoomCard(Card card) {
		if (!gameEnded) {
			zoomCardStats.CopyStats (card);
			int effectTab = 0;
			if (zoomCardStats.unitRelentless) {
				zoomCardEffects [effectTab].SetActive (true);
				zoomCardEffects [effectTab].GetComponentInChildren<Text> ().text =
					"Relentless - Excess damage is dealt to the next available target.";
				effectTab++;
			}
			if (zoomCardStats.unitManaFont > 0) {
				zoomCardEffects [effectTab].SetActive (true);
				zoomCardEffects [effectTab].GetComponentInChildren<Text> ().text =
					"Mana Font X- When this unit is in play, increase max mana by X.";
				effectTab++;
			}
			if (zoomCardStats.unitFury > 0) {
				zoomCardEffects [effectTab].SetActive (true);
				zoomCardEffects [effectTab].GetComponentInChildren<Text> ().text =
					"Fury X- This unit attacks X additional times.";
				effectTab++;
			}
			if (zoomCardStats.armor > 0) {
				zoomCardEffects [effectTab].SetActive (true);
				zoomCardEffects [effectTab].GetComponentInChildren<Text> ().text =
					"Armor X- Damage taken from units is reduced by X";
				effectTab++;
			}
			if (zoomCardStats.magicArmor > 0) {
				zoomCardEffects [effectTab].SetActive (true);
				zoomCardEffects [effectTab].GetComponentInChildren<Text> ().text =
					"Magic Armor X- Damage taken from spells is reduced by X";
				effectTab++;
			}
			if (zoomCardStats.crystalPact > 0) {
				zoomCardEffects [effectTab].SetActive (true);
				zoomCardEffects [effectTab].GetComponentInChildren<Text> ().text =
					"Crystal Pact X - Your crystal on the row this card is used on takes X damage.";
				effectTab++;
			}
			zoomCard.SetActive (true);
		}
	}

	//Allows other scripts to see if it is the player's turn
	public bool isPlayerTurn() {
		return playerTurn;
	}

	void WinGame() {
		List<Card> library = SaveLoad.LoadPlayerLibrary ();
		if (library == null) {
			library = new List<Card> ();
		}

		CardRarityHolder cards = Resources.Load<CardRarityHolder> ("Prefabs/CardRarityList");

		int randomCard1 = Random.Range (0, 100);
		int randomCard2 = Random.Range (0, 100);

		if (randomCard1 < 50) {
			rewardCard1.GetComponent<Card> ().CopyStats (cards.commonCards [Random.Range (0, cards.commonCards.Length)]);
		} else if (randomCard1 < 80) {
			rewardCard1.GetComponent<Card> ().CopyStats (cards.uncommonCards [Random.Range (0, cards.uncommonCards.Length)]);
		} else if (randomCard1 < 95) {
			rewardCard1.GetComponent<Card> ().CopyStats (cards.rareCards [Random.Range (0, cards.rareCards.Length)]);
		} else {
			rewardCard1.GetComponent<Card> ().CopyStats (cards.ultimateCards [Random.Range (0, cards.ultimateCards.Length)]);
		}

		if (randomCard2 < 50) {
			rewardCard2.GetComponent<Card> ().CopyStats (cards.commonCards [Random.Range (0, cards.commonCards.Length)]);
		} else if (randomCard2 < 80) {
			rewardCard2.GetComponent<Card> ().CopyStats (cards.uncommonCards [Random.Range (0, cards.uncommonCards.Length)]);
		} else if (randomCard2 < 95) {
			rewardCard2.GetComponent<Card> ().CopyStats (cards.rareCards [Random.Range (0, cards.rareCards.Length)]);
		} else {
			rewardCard2.GetComponent<Card> ().CopyStats (cards.ultimateCards [Random.Range (0, cards.ultimateCards.Length)]);
		}

		library.Add(rewardCard1.GetComponent<Card>().baseCard);
		library.Add(rewardCard2.GetComponent<Card>().baseCard);

		SaveLoad.SavePlayerLibrary(library);

		rewardScreen.SetActive (true);
		gameEnded = true;
		HideZoomCard ();
	}

	//Checks if either side has all crystals destroyed
	public void CheckGameEnded() {
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
			SaveLoad.ResetPlayerLibrary ();
			SaveLoad.ResetStage ();
			endText.text = "Defeat";
			restartButton.SetActive (true);
			gameEnded = true;
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
			WinGame ();
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

		yield return new WaitForSeconds(0.5f);
		for (int i = 0; i < attackingSpots.Length; i++) {
			if (attackingSpots [i].hasUnit && attackingSpots[i].damage > 0) {		//Check if the attackingSpot has a unit with damage > 0
				attackingSpots [i].unitAnimation.Play ("Attack");					// If it does, play attack animation and wait for it to finish
				yield return new WaitForSeconds (0.7f);
				if (attackingSpots [i].collumn == 1) {		// If the unit is in collumn one, attack the enemy at i and i + 3, or the crystal
					if (defendingSpots [i].hasUnit) {
						if (attackingSpots [i].damage > (defendingSpots [i].health + defendingSpots[i].armor) && attackingSpots[i].relentless) {	// If unit has relentless, keep attacking with remaining damage
							if (attackingSpots [i].damage > (defendingSpots [i+3].health + defendingSpots [i].health + defendingSpots[i].armor + defendingSpots[i + 3].armor) 
								&& attackingSpots[i].relentless) {
								HitCrystal (i, startedPlayerTurn, 
									attackingSpots[i].damage - defendingSpots[i+3].health - defendingSpots[i].health - defendingSpots[i].armor - defendingSpots[i + 3].armor);
							}
							defendingSpots [i+3].TakeDamage (attackingSpots [i].damage - defendingSpots [i].health - defendingSpots[i].armor - defendingSpots[i+3].armor);
						}
						defendingSpots [i].TakeDamage(attackingSpots[i].damage - defendingSpots[i].armor);
					} else if (defendingSpots [i + 3].hasUnit) {
						if (attackingSpots [i].damage > (defendingSpots [i+3].health + defendingSpots[i+3].armor) && attackingSpots[i].relentless) {
							HitCrystal (i, startedPlayerTurn, attackingSpots[i].damage - defendingSpots[i+3].health - defendingSpots[i+3].armor);
						}
						defendingSpots [i + 3].TakeDamage (attackingSpots[i].damage - defendingSpots[i+3].armor);
					} else {
						HitCrystal (i, startedPlayerTurn);
					}
				} else {									// If the unit is in collumn 2, attack enemy at i-3 and i, or the crystal
					if (defendingSpots [i - 3].hasUnit) {
						if (attackingSpots [i].damage > (defendingSpots [i-3].health + defendingSpots[i-3].armor) && attackingSpots[i].relentless) {
							if (attackingSpots [i].damage > (defendingSpots [i-3].health + defendingSpots [i].health + defendingSpots[i].armor + defendingSpots[i-3].armor) 
								&& attackingSpots[i].relentless) {
								HitCrystal (i, startedPlayerTurn, 
									attackingSpots[i].damage - defendingSpots[i-3].health - defendingSpots[i].health - defendingSpots[i-3].armor - defendingSpots[i].armor);
							}
							defendingSpots [i].TakeDamage (attackingSpots [i].damage - defendingSpots [i-3].health - defendingSpots[i-3].armor - defendingSpots[i].armor);
						}
						defendingSpots [i - 3].TakeDamage (attackingSpots [i].damage - defendingSpots [i - 3].armor);
					} else if (defendingSpots [i].hasUnit) {
						if (attackingSpots [i].damage > (defendingSpots [i].health + defendingSpots[i].armor) && attackingSpots[i].relentless) {
							HitCrystal (i, startedPlayerTurn, attackingSpots[i].damage - defendingSpots[i].health - defendingSpots[i].armor);
						}
						defendingSpots [i].TakeDamage(attackingSpots[i].damage - defendingSpots[i].armor);
					} else {
						HitCrystal (i, startedPlayerTurn);
					}
				}
				if (attackingSpots [i].fury > 0) {
					attackingSpots [i].fury--;
					i--;
				}
			} else {
				attackingSpots[i].fury = 0;
			}
		} 
		yield return new WaitForSeconds(0.5f);
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
