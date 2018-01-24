using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public SpotStats[] playerSpots;
	public SpotStats[] enemySpots;

	public Player player;

	public Card selectedCard;
	public GameObject zoomCard;
	Card zoomCardStats;

	void Start() {
		zoomCardStats = zoomCard.GetComponent<Card> ();
		HideZoomCard ();
	}

	public void HideZoomCard() {
		zoomCard.SetActive (false);
	}

	public void ShowZoomCard(Card card) {
		zoomCardStats.CopyStats (card);
		zoomCard.SetActive (true);
	}

	public void EndTurn() {
		//Player attack
		player.ResetMana();
		for (int i = 0; i < playerSpots.Length; i++) {
			if (playerSpots [i].hasUnit) {
				if (playerSpots [i].collumn == 1) {
					if (enemySpots [i].hasUnit) {
						playerSpots [i].Attack (enemySpots [i]);
					} else if (enemySpots [i + 3].hasUnit) {
						playerSpots [i].Attack (enemySpots [i + 3]);
					}
				} else {
					if (enemySpots [i - 3].hasUnit) {
						playerSpots [i].Attack (enemySpots [i - 3]);
					} else if (enemySpots [i].hasUnit) {
						playerSpots [i].Attack (enemySpots [i]);
					}
				}
			}
		}

		//Enemy Attack
		for (int i = 0; i < enemySpots.Length; i++) {
			if (enemySpots [i].hasUnit) {
				if (enemySpots [i].collumn == 1) {
					if (playerSpots [i].hasUnit) {
						enemySpots [i].Attack (playerSpots [i]);
					} else if (playerSpots [i + 3].hasUnit) {
						enemySpots [i].Attack (playerSpots [i + 3]);
					}
				} else {
					if (playerSpots [i - 3].hasUnit) {
						enemySpots [i].Attack (playerSpots [i - 3]);
					} else if (playerSpots [i].hasUnit) {
						enemySpots [i].Attack (playerSpots [i]);
					}
				}
			}
		}
	}
}
