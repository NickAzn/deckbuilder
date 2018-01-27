using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	void Start() {
		zoomCardStats = zoomCard.GetComponent<Card> ();
		HideZoomCard ();
		Debug.Log ("Start");
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
	}

	public void EndTurn() {
		StartCoroutine (TurnEnd ());
	}

	IEnumerator TurnEnd() {
		if (playerTurn) {
			endTurnButton.SetActive (false);
			playerTurn = false;
			//Player attack
			for (int i = 0; i < playerSpots.Length; i++) {
				if (playerSpots [i].hasUnit) {
					playerSpots [i].unitAnimation.Play ("Attack");
					yield return new WaitForSeconds (0.7f);
					if (playerSpots [i].collumn == 1) {
						if (enemySpots [i].hasUnit) {
							playerSpots [i].Attack (enemySpots [i]);
						} else if (enemySpots [i + 3].hasUnit) {
							playerSpots [i].Attack (enemySpots [i + 3]);
						} else {
							HitCrystal (i, true);
						}
					} else {
						if (enemySpots [i - 3].hasUnit) {
							playerSpots [i].Attack (enemySpots [i - 3]);
						} else if (enemySpots [i].hasUnit) {
							playerSpots [i].Attack (enemySpots [i]);
						} else {
							HitCrystal (i, true);
						}
					}
				}
			}
			enemy.NewTurn ();
		} else {
			//Enemy Attack
			for (int i = 0; i < enemySpots.Length; i++) {
				if (enemySpots [i].hasUnit) {
					enemySpots [i].unitAnimation.Play ("Attack");
					yield return new WaitForSeconds (0.7f);
					if (enemySpots [i].collumn == 1) {
						if (playerSpots [i].hasUnit) {
							enemySpots [i].Attack (playerSpots [i]);
						} else if (playerSpots [i + 3].hasUnit) {
							enemySpots [i].Attack (playerSpots [i + 3]);
						} else {
							HitCrystal (i, false);
						}
					} else {
						if (playerSpots [i - 3].hasUnit) {
							enemySpots [i].Attack (playerSpots [i - 3]);
						} else if (playerSpots [i].hasUnit) {
							enemySpots [i].Attack (playerSpots [i]);
						} else {
							HitCrystal (i, false);
						}
					}
				}
			}
			endTurnButton.SetActive (true);
			playerTurn = true;
			player.NewTurn ();
		}
	}
}
