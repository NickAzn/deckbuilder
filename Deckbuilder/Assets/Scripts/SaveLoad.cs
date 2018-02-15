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
	static string COMMON_SHOP_CARD = "ShopCard1";
	static string UNCOMMON_SHOP_CARD = "ShopCard2";
	static string RARE_SHOP_CARD = "ShopCard3";
	static string ULTIMATE_SHOP_CARD = "ShopCard4";
	static string RANDOM_SHOP_CARD = "ShopCard5";

	static string ENCOUNTERS = "Encounters";
	static string CURRENT_ENCOUNTER = "CurrentEncounter";
	static string SHOP_ENCOUNTER = "ShopEncounter";
	static string STAGE_LEVEL = "Stage";


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

	public static void GenerateNewShop() {
		CardRarityHolder cards = Resources.Load<CardRarityHolder> ("Prefabs/CardRarityList");

		PlayerPrefs.SetString(COMMON_SHOP_CARD, cards.commonCards[Random.Range(0, cards.commonCards.Length)].name);
		PlayerPrefs.SetString(UNCOMMON_SHOP_CARD, cards.uncommonCards[Random.Range(0, cards.uncommonCards.Length)].name);
		PlayerPrefs.SetString(RARE_SHOP_CARD, cards.rareCards[Random.Range(0, cards.rareCards.Length)].name);
		PlayerPrefs.SetString(ULTIMATE_SHOP_CARD, cards.ultimateCards[Random.Range(0, cards.ultimateCards.Length)].name);

		int randomCard = Random.Range (0, 100);
		string randomCardName;
		if (randomCard < 50) {
			randomCardName = cards.commonCards [Random.Range (0, cards.commonCards.Length)].name;
		} else if (randomCard < 80) {
			randomCardName = cards.uncommonCards [Random.Range (0, cards.uncommonCards.Length)].name;
		} else if (randomCard < 95) {
			randomCardName = cards.rareCards [Random.Range (0, cards.rareCards.Length)].name;
		} else {
			randomCardName = cards.ultimateCards [Random.Range (0, cards.ultimateCards.Length)].name;
		}
		PlayerPrefs.SetString (RANDOM_SHOP_CARD, randomCardName);
	}

	public static Card[] LoadShopCards() {
		Card[] shopCards = new Card[5];

		shopCards[0] = Resources.Load<Card>("Cards/" + PlayerPrefs.GetString (COMMON_SHOP_CARD));
		shopCards[1] = Resources.Load<Card>("Cards/" + PlayerPrefs.GetString (UNCOMMON_SHOP_CARD));
		shopCards[2] = Resources.Load<Card>("Cards/" + PlayerPrefs.GetString (RARE_SHOP_CARD));
		shopCards[3] = Resources.Load<Card>("Cards/" + PlayerPrefs.GetString (ULTIMATE_SHOP_CARD));
		shopCards[4] = Resources.Load<Card>("Cards/" + PlayerPrefs.GetString (RANDOM_SHOP_CARD));

		return shopCards;
	}

	public static void RemoveShopCard(int shopCard) {
		if (shopCard == 0) {
			PlayerPrefs.DeleteKey (COMMON_SHOP_CARD);
		} else if (shopCard == 1) {
			PlayerPrefs.DeleteKey (UNCOMMON_SHOP_CARD);
		} else if (shopCard == 2) {
			PlayerPrefs.DeleteKey (RARE_SHOP_CARD);
		} else if (shopCard == 3) {
			PlayerPrefs.DeleteKey (ULTIMATE_SHOP_CARD);
		} else {
			PlayerPrefs.DeleteKey (RANDOM_SHOP_CARD);
		}
	}

	public static void SaveStageLoadout(int stage, int encounters, int shopEncounter) {
		PlayerPrefs.SetInt (STAGE_LEVEL, stage);
		PlayerPrefs.SetInt (ENCOUNTERS, encounters);
		PlayerPrefs.SetInt (SHOP_ENCOUNTER, shopEncounter);
	}

	public static int[] LoadStage() {
		int[] stageLoadout = new int[4];
		stageLoadout[0] = PlayerPrefs.GetInt (STAGE_LEVEL, -1);
		if (stageLoadout [0] == -1) {
			return null;
		}
		stageLoadout [1] = PlayerPrefs.GetInt (ENCOUNTERS);
		stageLoadout [2] = PlayerPrefs.GetInt (SHOP_ENCOUNTER);
		stageLoadout [3] = PlayerPrefs.GetInt (CURRENT_ENCOUNTER);
		return stageLoadout;
	}

	public static void ResetStage() {
		PlayerPrefs.SetInt (STAGE_LEVEL, -1);
		SaveCurrentEncounter (0);
	}

	public static void SaveCurrentEncounter(int encounter) {
		PlayerPrefs.SetInt (CURRENT_ENCOUNTER, encounter);
	}
}
