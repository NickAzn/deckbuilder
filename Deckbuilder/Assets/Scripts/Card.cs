using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour {

	GameManager gm;

	public Card baseCard;

	public RuntimeAnimatorController cardAnimation;
	public Sprite cardSprite;

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

	public bool spellSacrifice = false;
	public int spellCardDraw = 0;

	public bool unitRelentless = false;
	public int unitDeathDraw = 0;

	SpriteRenderer sr;

	void Start() {
		if (GameObject.Find ("GameManager") != null) {
			gm = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		}
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

		spellSacrifice = otherCard.spellSacrifice;
		spellCardDraw = otherCard.spellCardDraw;

		unitRelentless = otherCard.unitRelentless;
		unitDeathDraw = otherCard.unitDeathDraw;

		UpdateUI ();
	}

	void OnMouseDown() {
		if (gm != null) {
			gm.selectedCard = this;

		}
		sr.color = clickColor;
	}

	void OnMouseUp() {
		sr.color = Color.white;
	}

	void OnMouseOver() {
		if (gm != null) {
			gm.ShowZoomCard (this);
		}
	}

	void OnMouseExit() {
		if (gm != null) {
			gm.HideZoomCard ();
		}
	}

	// Sort cards based on cost
	public static int SortCardsByCost(Card card1, Card card2) {
		if (card1 == null) {
			if (card2 == null) {
				return 0;
			} else {
				return -1;
			}
		} else {
			if (card2 == null) {
				return 1;
			} else {
				int card1Cost = card1.manaCost;
				int card2Cost = card2.manaCost;

				if (card1Cost < card2Cost) {
					return -1;
				} else if (card1Cost > card2Cost) {
					return 1;
				} else {
					return 0;
				}
			}
		}
	}
}
