using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveLoad {

	static string PLAYER_DECK = "PlayerDeck";
	static string PLAYER_LIBRARY = "PlayerLibrary";

	static string CRYSTAL_1_HP = "Crystal1HP";
	static string CRYSTAL_2_HP = "Crystal2HP";
	static string CRYSTAL_3_HP = "Crystal3HP";

	static string TOKENS = "Tokens";

	//Saves the health of player crystals
	public static void SaveCrystalHealth(int crystal1, int crystal2, int crystal3) {
		PlayerPrefs.SetInt (CRYSTAL_1_HP, crystal1);
		PlayerPrefs.SetInt (CRYSTAL_2_HP, crystal2);
		PlayerPrefs.SetInt (CRYSTAL_3_HP, crystal3);
	}

	//Loads the health of player crystals and returns as an array of int
	public static int[] LoadCrystalHealth() {
		int[] crystalHealth = new int[3];
		crystalHealth[0] = (PlayerPrefs.GetInt (CRYSTAL_1_HP));
		crystalHealth[1] = (PlayerPrefs.GetInt (CRYSTAL_2_HP));
		crystalHealth[2] = (PlayerPrefs.GetInt (CRYSTAL_3_HP));
		return crystalHealth;
	}

	//Resets the health of player crystals to 10
	public static void ResetCrystalHealth() {
		PlayerPrefs.SetInt (CRYSTAL_1_HP, 10);
		PlayerPrefs.SetInt (CRYSTAL_2_HP, 10);
		PlayerPrefs.SetInt (CRYSTAL_3_HP, 10);
	}

	//Saves the player deck
	public static void SavePlayerDeck(List<Card> deck) {
		string deckCards = "";

		//Saves all card names as strings with spaces between cards
		for (int i = 0; i < deck.Count; i++) {
			deckCards = deckCards + deck[i].name + " ";
		}

		PlayerPrefs.SetString (PLAYER_DECK, deckCards);
	}

	//Loads the saved player deck and returns as a list of cards
	public static List<Card> LoadPlayerDeck() {

		//If there is a deck to load, load the deck, else, return null
		if (PlayerPrefs.GetString (PLAYER_DECK) != "") {
			List<Card> playerDeck = new List<Card> ();

			string deckCards = PlayerPrefs.GetString (PLAYER_DECK);

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
		PlayerPrefs.SetString (PLAYER_DECK, "");
	}

	public static List<Card> LoadPlayerLibrary() {
		//If there is a library to load, load the library, else, return null
		if (PlayerPrefs.GetString (PLAYER_LIBRARY) != "") {
			List<Card> playerLibrary = new List<Card> ();

			string libraryCards = PlayerPrefs.GetString (PLAYER_LIBRARY);

			//Split the string by the whitespace between card names to get card names individually
			string[] cardStrings = libraryCards.Split (null);

			//Load prefab associated with the card name and add it to the library
			for (int i = 0; i < cardStrings.Length - 1; i++) {
				playerLibrary.Add ((Resources.Load ("Cards/" + cardStrings [i]) as GameObject).GetComponent<Card>());
			}
			return playerLibrary;
		}
		return null;
	}

	public static void SavePlayerLibrary(List<Card> library) {
		string libraryCards = "";

		//Saves all card names as strings with spaces between cards
		for (int i = 0; i < library.Count; i++) {
			libraryCards = libraryCards + library[i].name + " ";
		}

		PlayerPrefs.SetString (PLAYER_LIBRARY, libraryCards);

	}

	public static void ResetPlayerLibrary() {
		PlayerPrefs.SetString (PLAYER_LIBRARY, "");
	}

	public static void SavePlayerTokens(int amount) {
		PlayerPrefs.SetInt (TOKENS, amount);
	}

	public static int LoadPlayerTokens() {
		return PlayerPrefs.GetInt (TOKENS, 0);
	}
}
