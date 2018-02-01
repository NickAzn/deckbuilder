﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckBuilder : MonoBehaviour {
	
	List<Card> deck = new List<Card>();
	List<Card> library = new List<Card>();
	public Card zoomCard;
	public GameObject deckCard;
	public Transform deckUI;
	public Transform libraryUI;

	public CardHolder selCardHolder;

	public GameObject addCardButton;
	public GameObject removeCardButton;

	public Text deckCardCount;
	public Text libraryCardCount;

	List<GameObject> deckCards = new List<GameObject>();
	List<GameObject> libraryCards = new List<GameObject>();

	void Start () {
		// load the player deck and library
		deck = SaveLoad.LoadPlayerDeck ();
		library = SaveLoad.LoadPlayerLibrary ();
		zoomCard.CopyStats (deck [0]);
		addCardButton.SetActive (false);
		removeCardButton.SetActive (false);
		RefreshUI ();
	}

	void RefreshUI() {
		if (deck != null) {
			// Sort the deck by mana cost
			deck.Sort (Card.SortCardsByCost);

			// Removes all previous display cards
			foreach (GameObject dc in deckCards) {
				Destroy (dc);
			}

			// Creates UI for all new cards, and expands the deck section if needed
			deckCards = new List<GameObject> ();
			for (int i = 0; i < deck.Count; i++) {
				GameObject dc = Instantiate (deckCard) as GameObject;
				deckCards.Add (dc);
				dc.GetComponent<CardHolder> ().origCard = deck [i];
				dc.GetComponent<CardHolder> ().position = i;
				dc.GetComponent<CardHolder> ().deckCard = true;
				dc.GetComponent<CardHolder> ().UpdateUI ();
				dc.transform.SetParent (deckUI);
				dc.GetComponent<RectTransform> ().anchorMax = new Vector2 (0f, 0.5f);
				dc.GetComponent<RectTransform> ().anchorMin = new Vector2 (0f, 0.5f);
				dc.GetComponent<RectTransform> ().anchoredPosition = new Vector3 ((150f * (i + 1) - 40f), 0f, 0f);
				dc.transform.localScale = new Vector3 (2f, 2f, 2f);
			}
			if (deck.Count > 7) {
				deckUI.GetComponent<RectTransform> ().sizeDelta = new Vector2 ((150f * (deck.Count - 7)) - 80f, 0f);
				deckUI.GetComponent<RectTransform> ().anchoredPosition = new Vector2 ((75f * (deck.Count - 7)) - 40f, 0f);
			} else {
				deckUI.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, 0f);
				deckUI.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f, 0f);
			}
		} else {
			deck = new List<Card> ();
		}

		if (library != null) {
			// Sorts the library by mana cost
			library.Sort (Card.SortCardsByCost);

			// Removes all previous display cards
			foreach (GameObject dc in libraryCards) {
				Destroy (dc);
			}

			// Creates UI for all new cards, and expands the library section if needed
			libraryCards = new List<GameObject> ();
			for (int i = 0; i < library.Count; i++) {
				GameObject dc = Instantiate (deckCard) as GameObject;
				deckCards.Add (dc);
				dc.GetComponent<CardHolder> ().origCard = library [i];
				dc.GetComponent<CardHolder> ().position = i;
				dc.GetComponent<CardHolder> ().deckCard = false;
				dc.GetComponent<CardHolder> ().UpdateUI ();
				dc.transform.SetParent (libraryUI);
				dc.GetComponent<RectTransform> ().anchorMax = new Vector2 (0f, 1f);
				dc.GetComponent<RectTransform> ().anchorMin = new Vector2 (0f, 1f);
				dc.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (80f + (150 * (i % 6)), -80f - (150f * (i / 6)), 0f);
				dc.transform.localScale = new Vector3 (2f, 2f, 2f);
			}

			if (library.Count > 12) {
				libraryUI.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, (150f * ((library.Count / 6) - 1)) - 80f);
				libraryUI.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f, -(75f * ((library.Count / 6) - 1)) + 40f);
			} else {
				libraryUI.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, 0f);
				libraryUI.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f, 0f);
			}
				

		} else {
			library = new List<Card> ();
		}
	}

	// Selects card and shows zoomed version with card stats
	public void SelectCard(CardHolder cardHolder) {
		zoomCard.CopyStats (cardHolder.origCard);
		selCardHolder = cardHolder;
		if (cardHolder.deckCard) {
			removeCardButton.SetActive (true);
			addCardButton.SetActive (false);
		} else {
			removeCardButton.SetActive (false);
			addCardButton.SetActive (true);
		}
	}

	// Deselects the card
	void DeselectCard() {
		selCardHolder = null;
		removeCardButton.SetActive (false);
		addCardButton.SetActive (false);
	}

	// Adds selected card from library to the player deck
	public void AddSelectedCard() {
		deck.Add (selCardHolder.GetComponent<CardHolder>().origCard);
		library.RemoveAt(selCardHolder.position);

		DeselectCard ();
		RefreshUI ();
	}

	// Removes the selected card from the deck and places it in the library
	public void RemoveSelectedCard() {
		deck.RemoveAt (selCardHolder.position);
		library.Add (selCardHolder.GetComponent<CardHolder>().origCard);

		DeselectCard ();
		RefreshUI ();
	}

	// Removes all cards from the deck and places them in the library
	public void ResetDeck() {
		for (int i = 0; i < deck.Count; i++) {
			library.Add (deck[i]);
			deck.RemoveAt(i);
			i--;
		}

		DeselectCard ();
		RefreshUI ();
	}

	// If the deck has atleast 20 cards, save the deck
	public void SaveDeck() {
		if (deck.Count >= 20) {
			SaveLoad.SavePlayerDeck (deck);
			SaveLoad.SavePlayerLibrary (library);
		}
	}
}
