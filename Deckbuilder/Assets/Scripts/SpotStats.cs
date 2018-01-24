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
	public GameObject highlighter;

	public Text atkUI;
	public Text hpUI;
	public SpriteRenderer unitSprite;

	public bool playerSide;
	public Player player;

	public Card origCard;

	SpriteRenderer sr;

	void Start() {
		sr = GetComponent<SpriteRenderer> ();
		UpdateUI ();
	}

	void Update() {
		if (gm.selectedCard != null) {
			Card card = gm.selectedCard;
			if (player.CanPlayCard(card)) {
				if (card.playerSideCast && playerSide && !hasUnit) {
					highlighter.SetActive (true);
				} else if (card.enemySideCast && !playerSide && !hasUnit) {
					highlighter.SetActive (true);
				}
			}
		} else {
			highlighter.SetActive (false);
		}
	}

	//Adds unit to spot with given damage, health, and unit sprite
	public void AddUnit(Card card) {
		if (!hasUnit) {
			hasUnit = true;
			health = card.health;
			damage = card.attack;
			unitSprite.sprite = card.unit;
			origCard = card;
			UpdateUI ();
		}
	}

	//Removes unit from spot
	void RemoveUnit() {
		hasUnit = false;
		unitSprite.sprite = null;
		health = 0;
		damage = 0;
		if (playerSide) {
			player.DiscardCard (origCard);
		}
		origCard = null;
		UpdateUI ();
	}

	//Places card on spot
	void OnMouseDown() {
		if (!hasUnit) {
			if (gm.selectedCard != null) {
				Card selCard = gm.selectedCard;
				if (selCard.playerSideCast && playerSide) {
					player.PlayCard (this);
					gm.selectedCard = null;
				} else if (selCard.enemySideCast && !playerSide) {
					player.PlayCard (this);
					gm.selectedCard = null;
				}
			}
		}
	}

	//Changes color when hovered over
	void OnMouseOver() {
		sr.color = hoverColor;
		if (hasUnit) {
			gm.ShowZoomCard (origCard);
		}
	}

	//Goes back to original color when mouse stops hovering
	void OnMouseExit() {
		sr.color = Color.white;
		gm.HideZoomCard ();
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
		if (hasUnit) {
			atkUI.text = damage.ToString ();
			hpUI.text = health.ToString ();
		} else {
			atkUI.text = "";
			hpUI.text = "";
		}
	}
}
