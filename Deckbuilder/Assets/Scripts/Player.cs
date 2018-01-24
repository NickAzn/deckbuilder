using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	public GameManager gm;

	public List<Card> deck = new List<Card>();
	List<Card> discards = new List<Card>();
	List<Card> inPlay = new List<Card>();

	public GameObject[] hand = new GameObject[5];

	int maxMana = 0;
	int mana = 0;

	public Text manaUI;

	void Start() {
		UpdateUI ();
		for (int i = 0; i < hand.Length; i++) {
			hand [i].SetActive (false);
		}
	}

	public void UpdateUI() {
		manaUI.text = mana.ToString () + "/" + maxMana.ToString ();
	}

	int FindOpenHand() {
		for (int i = 0; i < hand.Length; i++) {
			if (hand [i].activeSelf == false) {
				return i;
			}
		}
		return -1;
	}

	public void DrawCard() {
		int openIndex = FindOpenHand ();
		if (openIndex >= 0) {
			if (deck.Count == 0) {
				if (ResetDeck ()) {
					DrawCard ();
				}
			} else {
				int i = Random.Range (0, deck.Count);
				hand[openIndex].GetComponent<Card>().CopyStats(deck[i]);
				hand [openIndex].SetActive (true);
				deck.RemoveAt (i);
			}
		}
	}

	bool ResetDeck() {
		if (discards.Count > 0) {
			deck = new List<Card> (discards);
			discards.Clear ();
			return true;
		} else {
			return false;
		}
	}

	public void DiscardCard(Card card) {
		discards.Add (card);
	}

	public bool PlayCard(SpotStats spot) {
		Card card = gm.selectedCard;
		if (card.manaCost <= mana) {
			mana -= card.manaCost;
			inPlay.Add (card);
			spot.AddUnit (card);
			DisableSelectedCard ();
			UpdateUI ();
			return true;
		}
		return false;
	}

	int DisableSelectedCard() {
		if (gm.selectedCard != null) {
			int cardNum = -1;
			string cardTag = gm.selectedCard.tag;
			if (cardTag.Equals ("Card1")) {
				cardNum = 0;
			} else if (cardTag.Equals ("Card2")) {
				cardNum = 1;
			} else if (cardTag.Equals ("Card3")) {
				cardNum = 2;
			} else if (cardTag.Equals ("Card4")) {
				cardNum = 3;
			} else if (cardTag.Equals ("Card5")) {
				cardNum = 4;
			}
			hand [cardNum].SetActive (false);
			gm.selectedCard = null;
			return cardNum;
		}
		return -1;
	}

	public void SacrificeCard() {
		if (gm.selectedCard != null) {
			maxMana++;
			mana++;
			int cardNum = DisableSelectedCard ();
			DiscardCard (hand [cardNum].GetComponent<Card> ());
			hand [cardNum].SetActive (false);
			UpdateUI ();
		}
	}

	public void ResetMana() {
		mana = maxMana;
		UpdateUI ();
	}
}
