using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour {

    public bool onlineMode = false;
    public OnlineBoardManager onlineBM = null;

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

	public bool playerTurn = true;

	public TextMeshProUGUI endText;				// Set in editor to UI Text to display game over text
	public GameObject restartButton;	// Set in editor to button with GameManager.Restart()

	public GameObject rewardScreen;
	public GameObject rewardCard1;
	public GameObject rewardCard2;
	bool gameEnded = false;

    public GameObject enchantedParticle;
    int playerCardsPlayed = -1;
    int enemyCardsPlayed = 0;
    int crystalEnchParticleEmit = 20;

	void Start() {
		// Set player crystal health to the saved value
		int[] crystalHealth = SaveLoad.LoadCrystalHealth ();
		playerCrystals [0].SetHealth (crystalHealth[0]);
		playerCrystals [1].SetHealth (crystalHealth[1]);
		playerCrystals [2].SetHealth (crystalHealth[2]);

		// Hide the zoomCard
		zoomCardStats = zoomCard.GetComponent<Card> ();
		HideZoomCard ();

        if (!onlineMode) {
            int[] stageLoadout = SaveLoad.LoadStage();
            float crystalHeatlhMult = 1f;
            if (stageLoadout != null) {
                if (stageLoadout[3] == stageLoadout[1]) {
                    enemy = Instantiate(bossEnemyAIs[stageLoadout[0]]).GetComponent<EnemyAI>();
                    crystalHeatlhMult = 1.5f;
                } else {
                    enemy = Instantiate(enemyAIs[stageLoadout[0]]).GetComponent<EnemyAI>();
                }
                if (stageLoadout[0] == 0) {
                    enemyCrystals[0].SetHealth((int)(6 * crystalHeatlhMult));
                    enemyCrystals[1].SetHealth((int)(6 * crystalHeatlhMult));
                    enemyCrystals[2].SetHealth((int)(6 * crystalHeatlhMult));
                } else if (stageLoadout[0] == 1) {
                    enemyCrystals[0].SetHealth((int)(7 * crystalHeatlhMult));
                    enemyCrystals[1].SetHealth((int)(10 * crystalHeatlhMult));
                    enemyCrystals[2].SetHealth((int)(7 * crystalHeatlhMult));
                } else if (stageLoadout[0] == 2) {
                    enemyCrystals[0].SetHealth((int)(10 * crystalHeatlhMult));
                    enemyCrystals[1].SetHealth((int)(7 * crystalHeatlhMult));
                    enemyCrystals[2].SetHealth((int)(10 * crystalHeatlhMult));
                }
            } else {
                enemy = Instantiate(enemyAIs[0]).GetComponent<EnemyAI>();
                enemyCrystals[0].SetHealth(6);
                enemyCrystals[1].SetHealth(6);
                enemyCrystals[2].SetHealth(6);
            }
            // Sets enemy AI aggro to a random value to determine how aggressive the AI will play
            enemy.aggroMeter = Random.Range(0, 4);
        }

		// Hides the restart button
		restartButton.SetActive (false);
		rewardScreen.SetActive (false);

        SaveLoad.ResetCrystalEnchants();
        CrystalEnchant[] enchants = SaveLoad.LoadActiveCrystalEnchants();
        if (enchants[0] != null) {
            playerCrystals[0].AddEnchant(enchants[0]);
        }
        if (enchants[1] != null) {
            playerCrystals[1].AddEnchant(enchants[1]);
        }
        if (enchants[2] != null) {
            playerCrystals[2].AddEnchant(enchants[2]);
        }
        SaveLoad.SaveActiveCrystalEnchants(playerCrystals[0].enchant, playerCrystals[1].enchant, playerCrystals[2].enchant);
        UpdateCrystalEnch();

        for (int i = 0; i < playerSpots.Length; i++) {
            playerSpots[i].spotIndex = i;
        }
        for (int i = 0; i < enemySpots.Length; i++) {
            enemySpots[i].spotIndex = i;
        }
    }

    public void PlayedCard(bool isPlayer) {
        if (isPlayer) {
            playerCardsPlayed++;
            if (playerCardsPlayed == 0)
                playerCardsPlayed++;
        } else {
            enemyCardsPlayed++;
        }
        UpdateCrystalEnch();
    }

    void UpdateCrystalEnch() {
        foreach (Crystal i in playerCrystals) {
            if (i.isAlive()) {
                CrystalEnchant enchant = i.enchant;
                if (enchant != null) {
                    int playValue = 0;
                    if (enchant.playType > 0) {
                        playValue = playerCardsPlayed % enchant.playType;
                        if (playValue == 0 && IsPlayerTurn()) {
                            if (enchant.playDiscardDraw > 0)
                                player.DrawFromDiscards(enchant.playDiscardDraw);
                            if (enchant.playDamageBuff > 0)
                                BuffRandomUnit(playerSpots, enchant.playDamageBuff, 0, i.enchant.color);
                            if (enchant.playHealthBuff > 0)
                                BuffRandomUnit(playerSpots, 0, enchant.playHealthBuff, i.enchant.color);
                            i.enchantParticles.Emit(crystalEnchParticleEmit);
                        }
                        if (playerCardsPlayed > 0) {
                            i.enchantUIText.text = (enchant.playType - playValue).ToString();
                        } else {
                            i.enchantUIText.text = enchant.playType.ToString();
                        }
                    }
                }
            }
        }

        foreach (Crystal i in enemyCrystals) {
            if (i.isAlive()) {
                CrystalEnchant enchant = i.enchant;
                if (enchant != null) {
                    int playValue = 0;
                    if (enchant.playType > 0) {
                        playValue = enemyCardsPlayed % enchant.playType;
                        if (playValue == 0 && !IsPlayerTurn()) {
                            if (enchant.playDiscardDraw > 0)
                                enemy.DrawCards(enchant.playDiscardDraw);
                            if (enchant.playDamageBuff > 0)
                                BuffRandomUnit(enemySpots, enchant.playDamageBuff, 0, i.enchant.color);
                            if (enchant.playHealthBuff > 0)
                                BuffRandomUnit(enemySpots, 0, enchant.playHealthBuff, i.enchant.color);
                            i.enchantParticles.Emit(crystalEnchParticleEmit);
                        }
                        i.enchantUIText.text = (enchant.playType - playValue).ToString();
                    }
                }
            }
        }
    }

    void BuffRandomUnit(SpotStats[] spotList, int damageBuff, int healthBuff, Color enchColor) {
        List<int> availableSpots = new List<int>();
        for (int i = 0; i < spotList.Length; i++) {
            if (spotList[i].hasUnit) {
                availableSpots.Add(i);
            }
        }
        if (availableSpots.Count > 0) {
            int index = Random.Range(0, availableSpots.Count);
            SpotStats spot = spotList[availableSpots[index]];

            GameObject particles = Instantiate(enchantedParticle, new Vector3(spot.transform.position.x, spot.transform.position.y, -1f), Quaternion.identity);
            ParticleSystem.MainModule partSettings = particles.GetComponent<ParticleSystem>().main;
            partSettings.startColor = enchColor;

            spot.damage += damageBuff;
            spot.health += healthBuff;
            spot.UpdateUI();
        }
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
			int x = 0;
			if (zoomCardStats.unitRelentless) {
				zoomCardEffects [effectTab].SetActive (true);
				zoomCardEffects [effectTab].GetComponentInChildren<TextMeshProUGUI> ().text =
					"Relentless - Excess damage is dealt to the next available target.";
				effectTab++;
			}
			if (zoomCardStats.unitManaFont > 0) {
				x = zoomCardStats.unitManaFont;
				zoomCardEffects [effectTab].SetActive (true);
				zoomCardEffects [effectTab].GetComponentInChildren<TextMeshProUGUI> ().text =
					"Mana Font " + x + " - When this unit is in play, increase max mana by " + x + ".";
				effectTab++;
			}
			if (zoomCardStats.unitFury > 0) {
				x = zoomCardStats.unitFury;
				zoomCardEffects [effectTab].SetActive (true);
				zoomCardEffects [effectTab].GetComponentInChildren<TextMeshProUGUI> ().text =
					"Fury "+x+" - This unit attacks "+x+" additional time(s).";
				effectTab++;
			}
			if (zoomCardStats.armor > 0) {
				x = zoomCardStats.armor;
				zoomCardEffects [effectTab].SetActive (true);
				zoomCardEffects [effectTab].GetComponentInChildren<TextMeshProUGUI> ().text =
					"Armor "+x+" - Damage taken from units is reduced by "+x+".";
				effectTab++;
			}
			if (zoomCardStats.magicArmor > 0) {
				x = zoomCardStats.magicArmor;
				zoomCardEffects [effectTab].SetActive (true);
				zoomCardEffects [effectTab].GetComponentInChildren<TextMeshProUGUI> ().text =
					"Magic Armor "+x+" - Damage taken from spells is reduced by "+x+".";
				effectTab++;
			}
			if (zoomCardStats.crystalPact > 0) {
				x = zoomCardStats.crystalPact;
				zoomCardEffects [effectTab].SetActive (true);
				zoomCardEffects [effectTab].GetComponentInChildren<TextMeshProUGUI> ().text =
					"Crystal Pact "+x+" - Your crystal on the row this card is used on takes "+x+" damage.";
				effectTab++;
			}
			zoomCard.SetActive (true);
		}
	}

	//Allows other scripts to see if it is the player's turn
	public bool IsPlayerTurn() {
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
            if (onlineMode && onlineBM != null) {
                onlineBM.NetworkEndTurn();
            }
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
            if (!onlineMode)
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
