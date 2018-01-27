using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour {

	GameManager gm;

	public Card baseCard;

	public RuntimeAnimatorController cardAnimation;

	public int manaCost;

	public bool isUnit;
	public bool isSpell;

	public bool playerSideCast;
	public bool enemySideCast;

	public int attack;
	public int health;

	public string cardName;
	public string description;

	public Text nameUI;
	public Text descUI;
	public Text atkUI;
	public Text hpUI;
	public Text manaUI;
	public Animator cardArt;

	public Color clickColor;

	SpriteRenderer sr;

	void Start() {
		gm = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		sr = GetComponent<SpriteRenderer> ();
		UpdateUI ();
	}

	void UpdateUI() {
		cardArt.runtimeAnimatorController = cardAnimation;
		nameUI.text = cardName;
		descUI.text = description;
		if (attack > -1) {
			atkUI.text = attack.ToString ();
		} else {
			atkUI.text = "";
		}
		if (health > -1) {
			hpUI.text = health.ToString ();
		} else {
			hpUI.text = "";
		}
		manaUI.text = manaCost.ToString ();
	}

	public void CopyStats(Card otherCard) {
		isUnit = otherCard.isUnit;
		isSpell = otherCard.isSpell;
		cardAnimation = otherCard.cardAnimation;
		playerSideCast = otherCard.playerSideCast;
		enemySideCast = otherCard.enemySideCast;
		attack = otherCard.attack;
		health = otherCard.health;
		description = otherCard.description;
		cardName = otherCard.cardName;
		manaCost = otherCard.manaCost;
		UpdateUI ();
	}

	void OnMouseDown() {
		gm.selectedCard = this;
		sr.color = clickColor;
	}

	void OnMouseUp() {
		sr.color = Color.white;
	}

	void OnMouseOver() {
		gm.ShowZoomCard (this);
	}

	void OnMouseExit() {
		gm.HideZoomCard ();
	}
}
