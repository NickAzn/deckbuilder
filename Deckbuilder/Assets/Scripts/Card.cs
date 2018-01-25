using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour {

	GameManager gm;

	public Card baseCard;

	public Sprite unit;

	public int manaCost;

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
	public SpriteRenderer cardArt;

	public Color clickColor;

	SpriteRenderer sr;

	void Start() {
		gm = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		sr = GetComponent<SpriteRenderer> ();
		UpdateUI ();
	}

	void UpdateUI() {
		cardArt.sprite = unit;
		nameUI.text = cardName;
		descUI.text = description;
		atkUI.text = attack.ToString ();
		hpUI.text = health.ToString ();
		manaUI.text = manaCost.ToString ();
	}

	public void CopyStats(Card otherCard) {
		unit = otherCard.unit;
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
		sr.color = clickColor;
		gm.selectedCard = this;
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
