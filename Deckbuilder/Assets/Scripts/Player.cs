﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour {

	public GameManager gm;
    public OnlineBoardManager onlineBM = null;

	public List<Card> deck = new List<Card>();
	public List<Card> discards = new List<Card>();

	public GameObject[] hand = new GameObject[5];

	int maxMana = 0;
	int mana = 0;
	int maxSacs = 2;
	int sacs = 0;

	public TextMeshProUGUI manaUI;
	public TextMeshProUGUI deckUI;
	public TextMeshProUGUI discUI;

	public GameObject sacButton;

	void Start() {
		if (SaveLoad.LoadPlayerDeck () != null) {
			deck = SaveLoad.LoadPlayerDeck ();
		}

		SaveLoad.SavePlayerDeck (deck);

		//Update UI and hide all possible cards in hand
		UpdateUI ();
		for (int i = 0; i < hand.Length; i++) {
			hand [i].SetActive (false);
		}

		DrawCard (5);
	}

	void Update() {
		if (gm.selectedCard != null && sacs < maxSacs && gm.IsPlayerTurn()) {
			sacButton.SetActive (true);
		} else {
			sacButton.SetActive (false);
		}
	}
	//Updates player UI
	public void UpdateUI() {
		manaUI.text = mana.ToString () + "/" + maxMana.ToString ();
		deckUI.text = deck.Count.ToString ();
		discUI.text = discards.Count.ToString ();
	}

	//Finds an open spot for card in player hand
	int FindOpenHand() {
		for (int i = 0; i < hand.Length; i++) {
			if (hand [i].activeSelf == false) {
				return i;
			}
		}
		return -1;
	}

	//Draws a card from the deck
	//If the deck is empty, recycle the discards back into deck
	//Do not draw if deck and discards are empty or hand is full
	public void DrawCard(int amount) {
		if (amount > 0) {
			int openIndex = FindOpenHand ();
			if (openIndex >= 0) {
				if (deck.Count == 0) {
					ResetDeck ();
					DrawCard (amount);
				} else {
					int j = Random.Range (0, deck.Count);
					hand [openIndex].GetComponent<Card> ().CopyStats (deck [j]);
					hand [openIndex].SetActive (true);
					deck.RemoveAt (j);
					UpdateUI ();
					DrawCard (amount - 1);
				}
			}
		}
	}

    //Draw a card from discards
    //If the discards is empty, don't draw
    public void DrawFromDiscards(int amount) {
        if (amount > 0) {
            int openIndex = FindOpenHand();
            if (openIndex >= 0) {
                if (discards.Count > 0) {
                    hand[openIndex].GetComponent<Card>().CopyStats(discards[discards.Count - 1]);
                    hand[openIndex].SetActive(true);
                    discards.RemoveAt(discards.Count - 1);
                    UpdateUI();
                    DrawFromDiscards(amount - 1);
                }
            }
        }
    }

    // Place all cards in discards into the deck and clear discards
	bool ResetDeck() {
		if (discards.Count > 0) {
			deck = new List<Card> (discards);
			discards.Clear ();
			return true;
		} else {
			return false;
		}
	}

    // Place card in discards
	public void DiscardCard(Card card) {
		discards.Add (card);
		UpdateUI ();
	}

    //returns true if player has enough mana to play card
	public bool CanPlayCard (Card card) {
		if (card.manaCost <= mana) {
			return true;
		}
		return false;
	}

    //Plays card at given spot
	public bool PlayCard(SpotStats spot) {
		Card card = gm.selectedCard;
		if (CanPlayCard(card)) {
			mana -= card.manaCost;
			if (card.isUnit) {
				spot.AddUnit (card);
                if (onlineBM != null) {
                    onlineBM.NetworkSummonUnit(spot.spotIndex, card.baseCard.name);
                }
			} else if (card.isSpell) {
				spot.UseSpell (card, true);
                if (onlineBM != null) {
                    onlineBM.NetworkCastSpell(spot.playerSide, spot.spotIndex, card.baseCard.name);
                }
            }
			if (card.manaBoost > 0) {
				mana += card.manaBoost;
			}
			if (card.maxManaBoost > 0) {
				IncreaseMaxMana (card.maxManaBoost);
			}
			if (card.crystalPact > 0) {
				gm.playerCrystals [spot.row - 1].TakeDamage (card.crystalPact);
				gm.CheckGameEnded ();
			}
			DisableSelectedCard ();
			UpdateUI ();
            gm.PlayedCard(true);
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
			sacs++;
			int cardNum = DisableSelectedCard ();
			DiscardCard (hand [cardNum].GetComponent<Card> ().baseCard);
			hand [cardNum].SetActive (false);
			UpdateUI ();
		}
	}

	public void IncreaseMaxMana(int amount) {
		maxMana += amount;
	}

	public void IncreaseCurrentMana(int amount) {
		mana += amount;
	}

	public void NewTurn() {
		mana = maxMana;
		sacs = 0;
		DrawCard (2);
		UpdateUI ();
	}
}
