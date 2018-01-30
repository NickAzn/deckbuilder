using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveLoad {

	//Saves the health of player crystals
	public static void SaveCrystalHealth(int crystal1, int crystal2, int crystal3) {
		PlayerPrefs.SetInt ("Crystal1HP", crystal1);
		PlayerPrefs.SetInt ("Crystal2HP", crystal2);
		PlayerPrefs.SetInt ("Crystal3HP", crystal3);
	}

	//Loads the health of player crystals and returns as an array of int
	public static int[] LoadCrystalHealth() {
		int[] crystalHealth = new int[3];
		crystalHealth[0] = (PlayerPrefs.GetInt ("Crystal1HP"));
		crystalHealth[1] = (PlayerPrefs.GetInt ("Crystal2HP"));
		crystalHealth[2] = (PlayerPrefs.GetInt ("Crystal3HP"));
		return crystalHealth;
	}

	//Resets the health of player crystals to 10
	public static void ResetCrystalHealth() {
		PlayerPrefs.SetInt ("Crystal1HP", 10);
		PlayerPrefs.SetInt ("Crystal2HP", 10);
		PlayerPrefs.SetInt ("Crystal3HP", 10);
	}

	//Saves the player deck
	public static void SavePlayerDeck(List<Card> deck) {
		string deckCards = "";

		//Saves all card names as strings with spaces between cards
		for (int i = 0; i < deck.Count; i++) {
			deckCards = deckCards + deck[i].name + " ";
		}

		PlayerPrefs.SetString ("PlayerDeck", deckCards);
	}

	//Loads the saved player deck and returns as a list of cards
	public static List<Card> LoadPlayerDeck() {

		//If there is a deck to load, load the deck, else, return null
		if (PlayerPrefs.GetString ("PlayerDeck") != "") {
			List<Card> playerDeck = new List<Card> ();

			string deckCards = PlayerPrefs.GetString ("PlayerDeck");

			//Split the string by the whitespace between card names to get card names individually
			string[] cardStrings = deckCards.Split (null);

			//Load prefab associated with the card name and add it to the deck
			for (int i = 0; i < cardStrings.Length - 1; i++) {
				playerDeck.Add ((Resources.Load ("Cards/" + cardStrings [i]) as GameObject).GetComponent<Card>());
			}
			return playerDeck;
		}

		return null;
	}

	//Resets the player deck
	public static void ResetPlayerDeck() {
		PlayerPrefs.SetString ("PlayerDeck", "");
	}
}
