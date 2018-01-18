using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public SpotStats[] playerSpots;
	public SpotStats[] enemySpots;

	public void endTurn() {
		for (int i = 0; i < playerSpots.Length; i++) {
			if (playerSpots [i].hasUnit) {
				if (playerSpots [i].collumn == 1) {
					if (enemySpots [i].hasUnit) {
						playerSpots [i].attack (enemySpots [i]);
					} else if (enemySpots [i + 3].hasUnit) {
						playerSpots [i].attack (enemySpots [i + 3]);
					}
				} else {
					if (enemySpots [i - 3].hasUnit) {
						playerSpots [i].attack (enemySpots [i - 3]);
					} else if (enemySpots [i].hasUnit) {
						playerSpots [i].attack (enemySpots [i]);
					}
				}
			}
		}
	}
}
