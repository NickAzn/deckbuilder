﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardHolder : MonoBehaviour {

	//Class to display cards in the deckbuilder

	public Card origCard;

	public Image cardArt;
	public TextMeshProUGUI manaCost;
	public TextMeshProUGUI cardCount;

	public bool deckCard;
	public int position;

	// Displays a still image of the card and the mana cost of the card
	public void UpdateUI() {
		cardArt.sprite = origCard.cardSprite;
		manaCost.text = origCard.manaCost.ToString ();
	}
}
