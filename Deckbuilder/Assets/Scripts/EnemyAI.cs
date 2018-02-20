using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

	int turnCounter = 0;
	public Card[] earlyUnits;
	public Card[] midUnits;
	public Card[] lateUnits;
	public Card[] offensiveSpells;
	public Card[] enchantSpells;
	public GameManager gm;
	public int aggroMeter;
	public int earlyCardTurns;
	public int midCardTurns;

	int actionCounter = 0;

	int playerR1Atk = 0;
	int playerR2Atk = 0;
	int playerR3Atk = 0;
	int playerR1FrontHp = 0;
	int playerR2FrontHp = 0;
	int playerR3FrontHp = 0;

	bool pRow1CrystalUp = false;
	bool pRow2CrystalUp = false;
	bool pRow3CrystalUp = false;

	bool row1CrystalUp = false;
	bool row2CrystalUp = false;
	bool row3CrystalUp = false;

	int extraActionMult = 8;
	int cardCount = 4;

	//Summons a unit at the start of the game, but does not attack with it
	void Start() {
		gm = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		int summonSpot = SelectRandomOpenSpot ();
		int summonCard = Random.Range (0, earlyUnits.Length);
		SummonUnit (gm.enemySpots [summonSpot], earlyUnits [summonCard]);
	}

	public void NewTurn() {
		turnCounter++;

		actionCounter = 0;
		if (turnCounter < midCardTurns) {
			DrawCards (1);
		} else {
			DrawCards (2);
		}

		StartCoroutine (DecideAction ());
	}

	public void DrawCards(int amount) {
		cardCount += amount;
		if (cardCount > 5) {
			cardCount = 5;
		}
	}

    // Summons a unit card at spot
	void SummonUnit(SpotStats spot, Card card) {
		spot.AddUnit (card);
	}

    // Casts a spell card at spot
	void CastSpell(SpotStats spot, Card card) {
		spot.UseSpell (card, false);
	}
		
    // Selects a random open spot on enemy side
	int SelectRandomOpenSpot() {
		List<int> openSpots = new List<int> ();
		for (int i = 0; i < gm.enemySpots.Length; i++) {
			if (!gm.enemySpots [i].hasUnit) {
				openSpots.Add (i);
			}
		}
		if (openSpots.Count > 0) {
			int randomSelect = Random.Range (0, openSpots.Count);
			return openSpots [randomSelect];
		} else {
			return -1;
		}
	}

	// Shows card being played so player knows what the AI is using
	IEnumerator ShowCard(Card card, float time) {
		gm.ShowZoomCard (card);
		yield return new WaitForSeconds (time);
		gm.HideZoomCard ();
	}

	void GetPlayerUnits() {
		playerR1Atk = 0;
		playerR2Atk = 0;
		playerR3Atk = 0;
		playerR1FrontHp = 0;
		playerR2FrontHp = 0;
		playerR3FrontHp = 0;

		// Records the damage that all player rows deal
		// Records the health of the front most unit in that row
		for (int i = 0; i < gm.playerSpots.Length; i++) {
			SpotStats playerSpot = gm.playerSpots [i];
			if (playerSpot.row == 1) {
				playerR1Atk += playerSpot.damage;
				if (playerR1FrontHp == 0) {
					playerR1FrontHp = playerSpot.health;
				}
			} else if (playerSpot.row == 2) {
				playerR2Atk += playerSpot.damage;
				if (playerR2FrontHp == 0) {
					playerR2FrontHp = playerSpot.health;
				}
			} else if (playerSpot.row == 3) {
				playerR3Atk += playerSpot.damage;
				if (playerR3FrontHp == 0) {
					playerR3FrontHp = playerSpot.health;
				}
			}
		}
	}

	void GetPlayerCrystals() {
		pRow1CrystalUp = false;
		pRow2CrystalUp = false;
		pRow3CrystalUp = false;

		//Check which player crystals are still alive
		for (int i = 0; i < gm.playerCrystals.Length; i++) {
			if (gm.playerCrystals [i].isAlive ()) {
				if (i == 0) {
					pRow1CrystalUp = true;
				} else if (i == 1) {
					pRow2CrystalUp = true;
				} else if (i == 2) {
					pRow3CrystalUp = true;
				}
			}
		}
	}

	void GetCrystals() {
		row1CrystalUp = false;
		row2CrystalUp = false;
		row3CrystalUp = false;

		// Determine which crystals on enemy side are still alive
		for (int i = 0; i < gm.enemyCrystals.Length; i++) {
			if (gm.enemyCrystals [i].isAlive ()) {
				if (i == 0) {
					row1CrystalUp = true;
				} else if (i == 1) {
					row2CrystalUp = true;
				} else if (i == 2) {
					row3CrystalUp = true;
				}
			}
		}
	}

	IEnumerator DecideAction() {
		actionCounter++;
		cardCount--;

		if (turnCounter < 6) {
			extraActionMult = 13 - turnCounter;
		}

		yield return new WaitForSeconds (0.7f);

		//Chance to cast a spell before summoning units
		if (cardCount > 0 && Random.Range (0, actionCounter * extraActionMult) == 0) {
			actionCounter++;
			List<int> possibleSpells = OffensiveSpell ();
			if (possibleSpells.Count > 0) {
				int castSpot = possibleSpells [Random.Range (0, possibleSpells.Count)];
				int spellCard = Random.Range (0, offensiveSpells.Length);
				CastSpell (gm.playerSpots [castSpot], offensiveSpells [spellCard]);
				yield return ShowCard (offensiveSpells [spellCard], 1.5f);
				if (offensiveSpells [spellCard].spellCardDraw > 0) {
					DrawCards(offensiveSpells [spellCard].spellCardDraw);
				}

				cardCount--;
			} else {
				actionCounter--;
			}
		}

		Card summonCard;
		List<int> possibleSpots = new List<int>();
		GetPlayerUnits ();
		GetPlayerCrystals ();
		GetCrystals ();

		if (aggroMeter >= 3) {
			possibleSpots = AggroPlay ();
		} else if (aggroMeter <= 0) {
			possibleSpots = DefensivePlay ();
		} else if (aggroMeter == 1) {
			if (Random.Range (0, 3) == 0) {
				possibleSpots = AggroPlay ();
			} else {
				possibleSpots = DefensivePlay ();
			}
		} else if (aggroMeter == 2) {
			if (Random.Range (0, 3) == 0) {
				possibleSpots = DefensivePlay ();
			} else {
				possibleSpots = AggroPlay ();
			}
		} else {
			possibleSpots.Add (SelectRandomOpenSpot ());
		}

		bool manaBoosted = false;
		if (possibleSpots.Count > 0 && possibleSpots [0] > -1) {
			int summonSpot = possibleSpots[Random.Range (0, possibleSpots.Count)];
			if (turnCounter < earlyCardTurns) {
				summonCard = earlyUnits[Random.Range (0, earlyUnits.Length)];
			} else if (turnCounter < midCardTurns) {
				summonCard = midUnits[Random.Range (0, midUnits.Length)];
			} else {
				summonCard = lateUnits[Random.Range (0, lateUnits.Length)];
			}
			if (summonCard.manaBoost > 0) {
				manaBoosted = true;
			}
			SummonUnit (gm.enemySpots [summonSpot], summonCard);
		}
		yield return new WaitForSeconds (0.7f);

		if (cardCount > 0 && Random.Range (0, actionCounter * extraActionMult) == 0) {
			actionCounter++;
			List<int> possibleSpells = Enchant ();
			if (possibleSpells.Count > 0) {
				int castSpot = possibleSpells [Random.Range (0, possibleSpells.Count)];
				int spellCard = Random.Range (0, enchantSpells.Length);
				CastSpell (gm.enemySpots [castSpot], enchantSpells [spellCard]);
				yield return ShowCard (enchantSpells [spellCard], 1.5f);
				cardCount--;
			} else {
				actionCounter--;
			}
		}

		if ((cardCount > 0 && Random.Range (0, actionCounter * extraActionMult) == 0) || (manaBoosted && actionCounter < 6)) {
			StartCoroutine (DecideAction ());
		} else {
			gm.EndTurn ();
		}
	}

	//Attempts to guard crystals that are threatened by player units
	List<int> DefensivePlay() {
		List<int> possibleSummons = new List<int> ();
		if (row1CrystalUp && playerR1Atk > 0) {
			if (!gm.enemySpots [0].hasUnit) {
				possibleSummons.Add (0);
			} else if (!gm.enemySpots [3].hasUnit) {
				possibleSummons.Add (3);
			}
		}
		if (row2CrystalUp && playerR2Atk > 0) {
			if (!gm.enemySpots [1].hasUnit) {
				possibleSummons.Add (1);
			} else if (!gm.enemySpots [4].hasUnit) {
				possibleSummons.Add (4);
			}
		}
		if (row3CrystalUp && playerR3Atk > 0) {
			if (!gm.enemySpots [2].hasUnit) {
				possibleSummons.Add(2);
			} else if (!gm.enemySpots [5].hasUnit) {
				possibleSummons.Add(5);
			}
		}
		// If no crystals are threatened, make an aggressive play instead
		if (possibleSummons.Count == 0) {
			possibleSummons = AggroPlay ();
		}
		return possibleSummons;
	}
		
	List<int> AggroPlay() {
		List<int> possibleSummons = new List<int>();

		//Attempts to attack player rows with no units guarding the crystal
		if (pRow1CrystalUp && playerR1FrontHp == 0) {
			if (!gm.enemySpots [0].hasUnit) {
				possibleSummons.Add(0);
			} else if (!gm.enemySpots [3].hasUnit) {
				possibleSummons.Add(3);
			}
		}
		if (pRow2CrystalUp && playerR2FrontHp == 0) {
			if (!gm.enemySpots [1].hasUnit) {
				possibleSummons.Add(1);
			} else if (!gm.enemySpots [4].hasUnit) {
				possibleSummons.Add(4);
			}
		}
		if (pRow3CrystalUp && playerR3FrontHp == 0) {
			if (!gm.enemySpots [2].hasUnit) {
				possibleSummons.Add(2);
			} else if (!gm.enemySpots [5].hasUnit) {
				possibleSummons.Add(5);
			}
		}

		//If all rows are guarded, attack any row that has a crystal alive
		if (possibleSummons.Count == 0) {
			if (pRow1CrystalUp) {
				if (!gm.enemySpots [0].hasUnit) {
					possibleSummons.Add(0);
				} else if (!gm.enemySpots [3].hasUnit) {
					possibleSummons.Add(3);
				}
			}
			if (pRow2CrystalUp) {
				if (!gm.enemySpots [1].hasUnit) {
					possibleSummons.Add(1);
				} else if (!gm.enemySpots [4].hasUnit) {
					possibleSummons.Add(4);
				}
			}
			if (pRow3CrystalUp) {
				if (!gm.enemySpots [2].hasUnit) {
					possibleSummons.Add(2);
				} else if (!gm.enemySpots [5].hasUnit) {
					possibleSummons.Add(5);
				}
			}
		}
		return possibleSummons;
	}

	// Finds all spots an offensive spell can be played
	List<int> OffensiveSpell() {
		List<int> possibleSpells = new List<int> ();
		for (int i = 0; i < gm.playerSpots.Length; i++) {
			if (gm.playerSpots [i].hasUnit) {
				possibleSpells.Add (i);
			}
		}
		return possibleSpells;
	}

	// Finds all spots an enchantment spell can be played
	List<int> Enchant() {
		List<int> possibleSpells = new List<int> ();
		for (int i = 0; i < gm.enemySpots.Length; i++) {
			if (gm.enemySpots [i].hasUnit && gm.enemySpots [i].enchantment.origCard == null) {
				possibleSpells.Add (i);
			}
		}
		return possibleSpells;
	}
}
