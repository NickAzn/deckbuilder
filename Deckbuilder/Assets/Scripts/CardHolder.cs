using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardHolder : MonoBehaviour {

	public Card origCard;

	public Image cardArt;
	public Text manaCost;

	public void UpdateUI() {
		cardArt.sprite = origCard.cardSprite;
		manaCost.text = origCard.manaCost.ToString ();
	}
}
