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

	public GameManager gm;
	public Color hoverColor;

	public Text atkUI;
	public Text hpUI;

	public bool playerSide;

	SpriteRenderer sr;

	void Start() {
		sr = GetComponent<SpriteRenderer> ();
	}

	public void AddUnit(int health, int damage) {
		if (!hasUnit) {
			hasUnit = true;
			this.health = health;
			this.damage = damage;
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

	void OnMouseDown() {
		if (!hasUnit) {
			if (gm.selectedCard != null) {
				Card selCard = gm.selectedCard;
				if (selCard.playerSideCast && playerSide) {
					AddUnit (selCard.attack, selCard.health);
					gm.selectedCard = null;
				} else if (selCard.enemySideCast && !playerSide) {
					AddUnit (selCard.attack, selCard.health);
					gm.selectedCard = null;
				}
			}
		}
	}

	void OnMouseOver() {
		sr.color = hoverColor;
	}

	void OnMouseExit() {
		sr.color = Color.white;
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
