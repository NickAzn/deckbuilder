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
	public SpriteRenderer unitSprite;

	public bool playerSide;

	SpriteRenderer sr;

	void Start() {
		sr = GetComponent<SpriteRenderer> ();
	}

	//Adds unit to spot with given damage, health, and unit sprite
	public void AddUnit(int damage, int health, Sprite unit) {
		if (!hasUnit) {
			hasUnit = true;
			this.health = health;
			this.damage = damage;
			unitSprite.sprite = unit;
			UpdateUI ();
		}
	}

	//Removes unit from spot
	void RemoveUnit() {
		hasUnit = false;
		atkUI.text = "";
		hpUI.text = "";
		unitSprite.sprite = null;
		health = 0;
		damage = 0;
	}

	//Places card on spot
	void OnMouseDown() {
		if (!hasUnit) {
			if (gm.selectedCard != null) {
				Card selCard = gm.selectedCard;
				if (selCard.playerSideCast && playerSide) {
					AddUnit (selCard.attack, selCard.health, selCard.unit);
					gm.selectedCard = null;
				} else if (selCard.enemySideCast && !playerSide) {
					AddUnit (selCard.attack, selCard.health, selCard.unit);
					gm.selectedCard = null;
				}
			}
		}
	}

	//Changes color when hovered over
	void OnMouseOver() {
		sr.color = hoverColor;
	}

	//Goes back to original color when mouse stops hovering
	void OnMouseExit() {
		sr.color = Color.white;
	}

	//Damages another spot
	public void Attack(SpotStats enemy) {
		enemy.TakeDamage (damage);
	}

	//Take damage, if health is 0 or less, remove unit
	public void TakeDamage(int amount) {
		health -= amount;
		UpdateUI();
		if (health <= 0) {
			RemoveUnit ();
		}
	}

	//Updates spot's display attack and health
	void UpdateUI() {
		atkUI.text = damage.ToString ();
		hpUI.text = health.ToString ();
	}
		
}
