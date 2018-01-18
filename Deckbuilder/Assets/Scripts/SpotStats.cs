using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotStats : MonoBehaviour {

	public bool hasUnit = false;
	public int row = 0;
	public int collumn = 0;
	public int health = 0;
	public int damage = 0;

	public bool addUnit(int health, int damage) {
		if (!hasUnit) {
			health = this.health;
			damage = this.damage;
			return true;
		} else {
			return false;
		}
	}

	public void attack(SpotStats enemy) {
		enemy.takeDamage (damage);
	}

	public void takeDamage(int amount) {
		health -= amount;
		if (health <= 0) {
			hasUnit = false;
			health = 0;
			damage = 0;
		}
	}
}
