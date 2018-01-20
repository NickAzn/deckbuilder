using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpotStats : MonoBehaviour {

	public bool hasUnit = false;
	public int row = 0;
	public int collumn = 0;
	public int health = 0;
	public int damage = 0;

	public Text atkUI;
	public Text hpUI;

	public void AddUnit(int health, int damage) {
		if (!hasUnit) {
			health = this.health;
			damage = this.damage;
			UpdateUI ();
		}
	}

	void RemoveUnit() {
		hasUnit = false;
		atkUI.text = "";
		hpUI.text = "";
		health = 0;
		damage = 0;
	}

	public void Attack(SpotStats enemy) {
		enemy.TakeDamage (damage);
	}

	public void TakeDamage(int amount) {
		health -= amount;
		UpdateUI();
		if (health <= 0) {
			RemoveUnit ();
		}
	}

	void UpdateUI() {
		atkUI.text = damage.ToString ();
		hpUI.text = health.ToString ();
	}
		
}
