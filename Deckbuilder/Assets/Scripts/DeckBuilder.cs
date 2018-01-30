using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckBuilder : MonoBehaviour {
	
	List<Card> deck = new List<Card>();
	public Card zoomCard;
	public GameObject deckCard;
	public Transform deckUI;

	void Start () {
		// load the player deck and sort by the cost of the cards
		deck = SaveLoad.LoadPlayerDeck ();
		deck.Sort (Card.SortCardsByCost);

		zoomCard.CopyStats (deck [0]);
		for (int i = 0; i < deck.Count; i++) {
			GameObject dc = Instantiate (deckCard) as GameObject;
			dc.GetComponent<CardHolder> ().origCard = deck[i];
			dc.GetComponent<CardHolder> ().UpdateUI ();
			dc.transform.SetParent (deckUI);
			dc.GetComponent<RectTransform> ().anchorMax = new Vector2 (0f, 0.5f);
			dc.GetComponent<RectTransform> ().anchorMin = new Vector2 (0f, 0.5f);
			dc.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (150f * (i + 1), 0f, 0f);
			dc.transform.localScale = new Vector3 (2f, 2f, 2f);
		}

		deckUI.GetComponent<RectTransform> ().sizeDelta = new Vector2(150f * (deck.Count - 7), 0f);
		deckUI.GetComponent<RectTransform> ().anchoredPosition = new Vector2(75f * (deck.Count - 7), 0f);
	}

	public void ShowZoomCard(CardHolder cardHolder) {
		zoomCard.CopyStats (cardHolder.origCard);
	}
}
