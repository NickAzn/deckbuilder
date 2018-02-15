using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Shop : MonoBehaviour {

	public Card zoomCard;
	public GameObject deckCard;
	public Transform libraryUI;
	CardHolder selCardHolder;
	int selCardPrice;

	public Crystal[] playerCrystals;
	public Button[] crystalRepairButtons;

	List<Card> library = new List<Card>();
	List<GameObject> libraryCards = new List<GameObject>();

	int tokens = 0;
	public Text tokenUI;

	public GameObject sellButton;
	public GameObject sellAllButton;
	public GameObject buyButton;

	public CardHolder commonCard;
	public CardHolder uncommonCard;
	public CardHolder rareCard;
	public CardHolder ultimateCard;
	public CardHolder randomCard;

	void Start() {
		//SaveLoad.GenerateNewShop ();
		tokens = SaveLoad.LoadPlayerTokens ();
		library = SaveLoad.LoadPlayerLibrary ();
		sellButton.SetActive (false);
		sellAllButton.SetActive (false);
		buyButton.SetActive (false);

		RefreshLibrary ();
		RefreshCrystals ();
		RefreshTokens ();
		RefreshCards ();
	}

	void RefreshLibrary() {
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
				CardHolder cd = dc.GetComponent<CardHolder> ();
				libraryCards.Add (dc);
				dc.GetComponent<Button> ().onClick.AddListener(delegate{SelectCard(cd);});
				cd.origCard = library [i];
				cd.position = i;
				cd.deckCard = true;
				cd.UpdateUI ();
				dc.transform.SetParent (libraryUI);
				dc.GetComponent<RectTransform> ().anchorMax = new Vector2 (0f, 1f);
				dc.GetComponent<RectTransform> ().anchorMin = new Vector2 (0f, 1f);
				dc.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (80f + (150 * ((libraryCards.Count - 1) % 6)), -80f - (150f * ((libraryCards.Count - 1)/ 6)), 0f);
				dc.transform.localScale = new Vector3 (2f, 2f, 2f);
				int cardCount = CardCounter (library [i].name, library);
				cd.cardCount.text = cardCount.ToString () + "x";
				i += (cardCount - 1);
			}

			if (libraryCards.Count > 6) {
				libraryUI.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, (180f * (libraryCards.Count / 6)) - 80f);
				libraryUI.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f, -(90f * (libraryCards.Count / 6)) + 40f);
			} else {
				libraryUI.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, 0f);
				libraryUI.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f, 0f);
			}
		} else {
			library = new List<Card> ();
		}
	}

	void RefreshCrystals() {
		int[] crystalHealth = SaveLoad.LoadCrystalHealth ();

		playerCrystals [0].SetHealth(crystalHealth [0]);
		playerCrystals [1].SetHealth(crystalHealth [1]);
		playerCrystals [2].SetHealth(crystalHealth [2]);


		if (crystalHealth [0] == 10) {
			crystalRepairButtons [0].gameObject.SetActive (false);
		} else if (crystalHealth [0] == 0) {
			crystalRepairButtons [0].onClick = new Button.ButtonClickedEvent ();
			crystalRepairButtons [0].onClick.AddListener(delegate{RestoreCrystal(0);});
			crystalRepairButtons [0].GetComponent<Image> ().sprite = Resources.Load <Sprite>("Textures/CrystalRestoreIcon");
			crystalRepairButtons [0].GetComponentInChildren<Text> ().text = "-30";
		}

		if (crystalHealth [1] == 10) {
			crystalRepairButtons [1].gameObject.SetActive (false);
		} else if (crystalHealth [1] == 0) {
			crystalRepairButtons [1].onClick = new Button.ButtonClickedEvent ();
			crystalRepairButtons [1].onClick.AddListener(delegate{RestoreCrystal(1);});
			crystalRepairButtons [1].GetComponent<Image> ().sprite = Resources.Load <Sprite>("Textures/CrystalRestoreIcon");
			crystalRepairButtons [1].GetComponentInChildren<Text> ().text = "-30";
		}

		if (crystalHealth [2] == 10) {
			crystalRepairButtons [2].gameObject.SetActive (false);
		} else if (crystalHealth [2] == 0) {
			crystalRepairButtons [2].onClick = new Button.ButtonClickedEvent ();
			crystalRepairButtons [2].onClick.AddListener(delegate{RestoreCrystal(2);});
			crystalRepairButtons [2].GetComponent<Image> ().sprite = Resources.Load <Sprite>("Textures/CrystalRestoreIcon");
			crystalRepairButtons [2].GetComponentInChildren<Text> ().text = "-30";
		}
	}

	void RefreshCards() {
		Card[] shopCards = SaveLoad.LoadShopCards ();
		if (shopCards [0] != null) {
			commonCard.origCard = shopCards [0];
			commonCard.UpdateUI ();
		} else {
			commonCard.gameObject.SetActive (false);
		}

		if (shopCards [1] != null) {
			uncommonCard.origCard = shopCards [1];
			uncommonCard.UpdateUI ();
		} else {
			uncommonCard.gameObject.SetActive (false);
		}

		if (shopCards [2] != null) {
			rareCard.origCard = shopCards [2];
			rareCard.UpdateUI ();
		} else {
			rareCard.gameObject.SetActive (false);
		}

		if (shopCards [3] != null) {
			ultimateCard.origCard = shopCards [3];
			ultimateCard.UpdateUI ();
		} else {
			ultimateCard.gameObject.SetActive (false);
		}

		if (shopCards [4] != null) {
			randomCard.origCard = shopCards [4];
			randomCard.UpdateUI ();
		} else {
			randomCard.gameObject.SetActive (false);
		}
	}

	void RefreshTokens() {
		tokenUI.text = tokens.ToString ();
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

	public void SelectCard(CardHolder cd) {
		zoomCard.CopyStats (cd.origCard);
		selCardHolder = cd;
		if (cd.deckCard) {
			sellButton.SetActive (true);
			sellAllButton.SetActive (true);
			buyButton.SetActive (false);
		} else {
			sellButton.SetActive (false);
			sellAllButton.SetActive (false);
			buyButton.SetActive (true);
		}
	}

	void DeselectCard() {
		selCardHolder = null;
		sellButton.SetActive (false);
		sellAllButton.SetActive (false);
		buyButton.SetActive (false);
	}

	public void SellSelectCard() {
		library.RemoveAt (selCardHolder.position);
		SaveLoad.SavePlayerLibrary (library);

		tokens += 5;
		SaveLoad.SavePlayerTokens (tokens);

		DeselectCard ();
		sellButton.SetActive (false);
		sellAllButton.SetActive (false);
		RefreshLibrary ();
		RefreshTokens ();
	}

	public void SellAllSelectCard() {
		int cardCount = CardCounter (selCardHolder.origCard.name, library);
		for (int i = 0; i < cardCount; i++) {
			library.RemoveAt (selCardHolder.position);
			tokens += 5;
		}

		SaveLoad.SavePlayerLibrary (library);
		SaveLoad.SavePlayerTokens (tokens);
		DeselectCard ();
		sellButton.SetActive (false);
		sellAllButton.SetActive (false);
		RefreshLibrary ();
		RefreshTokens ();
	}

	public void SetCardPrice(int price) {
		selCardPrice = price;
	}

	public void BuySelectCard() {
		if (tokens >= selCardPrice) {
			tokens -= selCardPrice;
			library.Add (selCardHolder.origCard);
			selCardHolder.gameObject.SetActive (false);
			if (selCardPrice == 10) {
				SaveLoad.RemoveShopCard (0);
			} else if (selCardPrice == 15) {
				SaveLoad.RemoveShopCard (1);
			} else if (selCardPrice == 25) {
				SaveLoad.RemoveShopCard (2);
			} else if (selCardPrice == 50) {
				SaveLoad.RemoveShopCard (3);
			} else {
				SaveLoad.RemoveShopCard (4);
			}

			SaveLoad.SavePlayerTokens (tokens);
			SaveLoad.SavePlayerLibrary (library);
			RefreshTokens ();
			RefreshLibrary ();
			DeselectCard ();
		}
	}

	public void RepairCrystal(int crystal) {
		if (tokens >= 5) {
			tokens -= 5;
			playerCrystals[crystal].TakeDamage(-1);
			SaveLoad.SaveCrystalHealth (playerCrystals [0].GetHealth(), playerCrystals [1].GetHealth(), playerCrystals [2].GetHealth());
			SaveLoad.SavePlayerTokens (tokens);
			RefreshTokens ();
			RefreshCrystals ();
		}
	}

	public void RestoreCrystal(int crystal) {
		if (tokens >= 30) {
			tokens -= 30;
			playerCrystals [crystal].TakeDamage (-3);
			SaveLoad.SaveCrystalHealth (playerCrystals [0].GetHealth(), playerCrystals [1].GetHealth(), playerCrystals [2].GetHealth());
			SaveLoad.SavePlayerTokens (tokens);
			RefreshTokens ();

			crystalRepairButtons [crystal].GetComponent<Button> ().onClick.RemoveAllListeners ();
			crystalRepairButtons [crystal].GetComponent<Button> ().onClick.AddListener(delegate{RepairCrystal(crystal);});
			crystalRepairButtons [crystal].GetComponent<Image> ().sprite = Resources.Load <Sprite>("Textures/CrystalRepairIcon");
			crystalRepairButtons [crystal].GetComponentInChildren<Text> ().text = "-5";
		}
	}
		
	public void ExitToGame() {
		SceneManager.LoadScene ("StageMap");
	}
}
