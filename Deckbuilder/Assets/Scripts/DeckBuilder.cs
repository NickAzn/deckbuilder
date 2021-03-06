﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeckBuilder : MonoBehaviour {
	
	List<Card> deck = new List<Card>();
	List<Card> library = new List<Card>();
	public Card zoomCard;
	public GameObject deckCard;
	public Transform deckUI;
	public Transform libraryUI;

	public CardHolder selCardHolder;

	public GameObject addCardButton;
	public GameObject addAllCardButton;
	public GameObject removeCardButton;
	public GameObject removeAllCardButton;

	public Text deckCardCount;
	public GameObject feedback;
	public Text feedbackText;

	List<GameObject> deckCards = new List<GameObject>();
	List<GameObject> libraryCards = new List<GameObject>();

	void Start () {
		// load the player deck and library
		deck = SaveLoad.LoadPlayerDeck ();
		library = SaveLoad.LoadPlayerLibrary ();
		zoomCard.CopyStats (deck [0]);
		addCardButton.SetActive (false);
		addAllCardButton.SetActive (false);
		removeCardButton.SetActive (false);
		removeAllCardButton.SetActive (false);
		feedback.SetActive (false);
		RefreshUI ();
	}

	int CardCounter(string name, List<Card> list) {
		int counter = 0;
		foreach (Card card in list) {
			if (card.name.Equals (name)) {
				counter++;
			}
		}
		return counter;
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
				CardHolder cd = dc.GetComponent<CardHolder> ();
				deckCards.Add (dc);
				dc.GetComponent<Button> ().onClick.AddListener(delegate{SelectCard(cd);});
				cd.origCard = deck [i];
				cd.position = i;
				cd.deckCard = true;
				cd.UpdateUI ();
				dc.transform.SetParent (deckUI);
				dc.GetComponent<RectTransform> ().anchorMax = new Vector2 (0f, 0.5f);
				dc.GetComponent<RectTransform> ().anchorMin = new Vector2 (0f, 0.5f);
				dc.GetComponent<RectTransform> ().anchoredPosition = new Vector3 ((150f * (deckCards.Count) - 40f), 0f, 0f);
				dc.transform.localScale = new Vector3 (2f, 2f, 2f);
				int cardCount = CardCounter (deck [i].name, deck);
				cd.cardCount.text = cardCount.ToString () + "x";
				i += (cardCount - 1);

			}
			if (deckCards.Count > 7) {
				float sizeScale = 1.0f;
				if (Camera.main.aspect < 1.7) {
					sizeScale = 1.1f;
				}
				deckUI.GetComponent<RectTransform> ().sizeDelta = new Vector2 ((150f * (deckCards.Count - 7) * sizeScale) - 80f, 0f);
				deckUI.GetComponent<RectTransform> ().anchoredPosition = new Vector2 ((75f * (deckCards.Count - 7) * sizeScale) - 40f, 0f);
			} else {
				deckUI.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, 0f);
				deckUI.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f, 0f);
			}
			deckCardCount.text = deck.Count.ToString ();
		} else {
			deck = new List<Card> ();
			deckCardCount.text = "0";
		}

		if (library != null) {
			// Sorts the library by mana cost
			library.Sort (Card.SortCardsByCost);

			// Removes all previous display cards
			foreach (GameObject dc in libraryCards) {
				Destroy (dc);
			}

			int rowCount = 6;
			if (Camera.main.aspect < 1.7) {
				rowCount = 5;
			}

			// Creates UI for all new cards, and expands the library section if needed
			libraryCards = new List<GameObject> ();
			for (int i = 0; i < library.Count; i++) {
				GameObject dc = Instantiate (deckCard) as GameObject;
				CardHolder cd = dc.GetComponent<CardHolder> ();
				libraryCards.Add (dc);
				dc.GetComponent<Button> ().onClick.AddListener(delegate{SelectCard(cd);});
				cd.origCard = library [i];
				cd.position = i;
				cd.deckCard = false;
				cd.UpdateUI ();
				dc.transform.SetParent (libraryUI);
				dc.GetComponent<RectTransform> ().anchorMax = new Vector2 (0f, 1f);
				dc.GetComponent<RectTransform> ().anchorMin = new Vector2 (0f, 1f);
				dc.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (80f + (150 * ((libraryCards.Count - 1) % rowCount)), -80f - (150f * ((libraryCards.Count - 1)/ rowCount)), 0f);
				dc.transform.localScale = new Vector3 (2f, 2f, 2f);
				int cardCount = CardCounter (library [i].name, library);
				cd.cardCount.text = cardCount.ToString () + "x";
				i += (cardCount - 1);
			}

			if (libraryCards.Count > (rowCount * 2)) {
				libraryUI.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, (150f * ((libraryCards.Count / rowCount) - 1)) - 80f);
				libraryUI.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f, -(75f * ((libraryCards.Count / rowCount) - 1)) + 40f);
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
			removeAllCardButton.SetActive (true);
			addCardButton.SetActive (false);
			addAllCardButton.SetActive (false);
		} else {
			removeCardButton.SetActive (false);
			removeAllCardButton.SetActive (false);
			addCardButton.SetActive (true);
			addAllCardButton.SetActive (true);
		}
	}

	// Deselects the card
	void DeselectCard() {
		selCardHolder = null;
		removeCardButton.SetActive (false);
		removeAllCardButton.SetActive (false);
		addCardButton.SetActive (false);
		addAllCardButton.SetActive (false);
	}

	// Adds selected card from library to the player deck
	public void AddSelectedCard() {
		int cardCount = CardCounter (selCardHolder.GetComponent<CardHolder> ().origCard.name, deck);
		if (cardCount < 3) {
			deck.Add (selCardHolder.GetComponent<CardHolder> ().origCard);
			library.RemoveAt (selCardHolder.position);
			DeselectCard ();
			RefreshUI ();
		} else {
			feedbackText.text = "Cannot add card.\nOnly 3 copies of a card may be in a deck.";
			feedback.SetActive (true);
		}
	}

	// Adds all copies of the selected card from library to the deck
	public void AddAllSelectedCard() {
		int cardCountDeck = CardCounter (selCardHolder.GetComponent<CardHolder> ().origCard.name, deck);
		if (cardCountDeck < 3) {
			int cardCountLibrary = CardCounter (selCardHolder.GetComponent<CardHolder> ().origCard.name, library);
			if (cardCountLibrary > (3 - cardCountDeck)) {
				cardCountLibrary = 3 - cardCountDeck;
			}
			for (int i = 0; i < cardCountLibrary; i++) {
				deck.Add (selCardHolder.GetComponent<CardHolder> ().origCard);
				library.RemoveAt (selCardHolder.position);
			}
			DeselectCard ();
			RefreshUI ();
		} else {
			feedbackText.text = "Cannot add card.\nOnly 3 copies of a card may be in a deck.";
			feedback.SetActive (true);
		}
	}

	// Removes the selected card from the deck and places it in the library
	public void RemoveSelectedCard() {
		deck.RemoveAt (selCardHolder.position);
		library.Add (selCardHolder.GetComponent<CardHolder>().origCard);

		DeselectCard ();
		RefreshUI ();
	}

	// Removes all copies of the selected card from the deck to the library
	public void RemoveAllSelectedCard() {
		int cardCount = CardCounter (selCardHolder.GetComponent<CardHolder> ().origCard.name, deck);
		for (int i = 0; i < cardCount; i++) {
			deck.RemoveAt (selCardHolder.position);
			library.Add (selCardHolder.GetComponent<CardHolder> ().origCard);
		}

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

	// If the deck has atleast 25 cards, save the deck
	public void SaveDeck() {
		if (deck.Count >= 25) {
			SaveLoad.SavePlayerDeck (deck);
			SaveLoad.SavePlayerLibrary (library);
			feedbackText.text = "Deck saved.";
		} else {
			feedbackText.text = "Could not save deck.\nDeck requires atleast 25 cards.\nYour deck has " + deck.Count.ToString() + " cards.";
		}
		feedback.SetActive (true);
	}

	public void CloseFeedback() {
		feedback.SetActive (false);
	}

	public void ExitToGame() {
		SceneManager.LoadScene ("StageMap");
	}
}
