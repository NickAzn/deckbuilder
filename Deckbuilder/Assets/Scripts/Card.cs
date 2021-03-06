﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Card : MonoBehaviour {

	GameManager gm;

	public Card baseCard;

	public RuntimeAnimatorController cardAnimation;
	public Sprite cardSprite;

	public int manaCost;

	public bool isUnit;		// Sets card to be a unit
	public bool isSpell;	// Sets card to be a spell

	public bool playerSideCast;	// ALlows player to cast this card on player side
	public bool enemySideCast;	// Allows player to cast this card on enemy side

	public int attack;
	public int health;

	public string cardName;
	[TextArea]
	public string description;

	public TextMeshProUGUI nameUI;
	public TextMeshProUGUI descUI;
	public TextMeshProUGUI atkUI;
	public TextMeshProUGUI hpUI;
	public TextMeshProUGUI manaUI;
	public Animator cardArt;

	public Color clickColor;
	public Color hoverColor;

	public bool spellEnchant = false;	// When true, sets spell as enchant, so it uses a spot's enchant spot
	public bool spellSacrifice = false;	// When true, spell will remove the unit it is casted on
	public int spellCardDraw = 0;		// When > 0, spell will make the player draw that amount of cards
	public int spellAttackReduce = 0;	// When > 0, spell/enchant will reduce the attack of unit it is casted on
	public int spellStartDamage = 0;	// When > 0, deal damage to enemy unit in same row
	public bool spellStartLifeSteal = false;	// When true, damage dealt at start of turn from spellStartDamage increases health.
	public int spellRowHit = 0;			// When > 0, deal damage to enemy units in same row
	public int spellColumnHit = 0;		// When > 0, deal damage to enemy units in same column
	public int spellRowColumnHit = 0;	// When > 0, deal damage to units in same row and column

	public bool unitRelentless = false;	// When true, unit will continue attacking past first target, if it has extra attack
	public int unitDeathDraw = 0;		// When > 0, player will draw that amount of cards on unit death
	public int unitManaFont = 0;		// When > 0, player gains that amount of max mana while unit is alive
	public int unitFury = 0;			// When > 0, unit attack that amount of additional times

	public int manaBoost = 0;			// When > 0, this card grants that amount of current mana the turn it is used
	public int maxManaBoost = 0;		// When > 0, this card grants increased max mana
	public int armor = 0;				// Reduce damage taken from units by armor value
	public int magicArmor = 0;			// Reduce damage taken from spells by magicArmor value
	public int crystalPact = 0;			// Card damages crystal in the row it was used by crystalPact value

	Image cardBack;

	void Start() {
		if (GameObject.Find ("GameManager") != null) {
			gm = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		}
		cardBack = GetComponent<Image> ();
		UpdateUI ();
	}
		
	//Update the card's UI to the card's current stats
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

	//Copies all stats from otherCard
	public void CopyStats(Card otherCard) {
		baseCard = otherCard.baseCard;

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

		spellEnchant = otherCard.spellEnchant;
		spellSacrifice = otherCard.spellSacrifice;
		spellCardDraw = otherCard.spellCardDraw;
		spellAttackReduce = otherCard.spellAttackReduce;
		spellStartDamage = otherCard.spellStartDamage;
		spellStartLifeSteal = otherCard.spellStartLifeSteal;
		spellRowHit = otherCard.spellRowHit;
		spellColumnHit = otherCard.spellColumnHit;
		spellRowColumnHit = otherCard.spellRowColumnHit;

		unitRelentless = otherCard.unitRelentless;
		unitDeathDraw = otherCard.unitDeathDraw;
		unitManaFont = otherCard.unitManaFont;
		unitFury = otherCard.unitFury;

		manaBoost = otherCard.manaBoost;
		maxManaBoost = otherCard.maxManaBoost;
		armor = otherCard.armor;
		magicArmor = otherCard.magicArmor;
		crystalPact = otherCard.crystalPact;

		UpdateUI ();
	}

	//When clicked, set gamemanager's selected card to this card
	void OnMouseDown() {
		if (gm != null) {
			gm.selectedCard = this;

		}
		//Change color to give player feedback that they clicked the card
		cardBack.color = clickColor;
	}

	//When mouse is released from a click, change color back to white from the click color
	void OnMouseUp() {
		cardBack.color = hoverColor;
	}

	//When hovering over the card, show a zoomed version of card for readability
	void OnMouseEnter() {
		if (gm != null) {
			gm.ShowZoomCard (this);
			cardBack.color = hoverColor;
		}
	}

	//When mouse is no longer hovering over the card, stop showing the zoomed card
	void OnMouseExit() {
		if (gm != null) {
			gm.HideZoomCard ();
			cardBack.color = Color.white;
		}
	}

	// Sort cards based on mana cost, if mana cost is the same, sort alphabetically
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
					return SortCardsByName(card1, card2);
				}
			}
		}
	}

	// Sort cards based on type (unit then spells), if same type, sort alphabetically
	public static int SortCardsByType(Card card1, Card card2) {
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
				int card1Type = 0;
				int card2Type = 0;

				if (card1.isSpell)
					card1Type = 1;
				if (card2.isSpell)
					card2Type = 1;

				if (card1Type < card2Type) {
					return -1;
				} else if (card1Type > card2Type) {
					return 1;
				} else {
					return SortCardsByCost (card1, card2);
				}
			}
		}
	}

	// Sorts cards alphabetically
	public static int SortCardsByName(Card card1, Card card2) {
		return (card1.name.CompareTo (card2.name));
	}
}
