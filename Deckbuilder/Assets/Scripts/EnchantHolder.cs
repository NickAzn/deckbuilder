using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnchantHolder : MonoBehaviour {

	public Card origCard;

	public Animator cardAnimation;

	GameManager gm;

	void Start() {
		gm = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		cardAnimation.gameObject.SetActive (false);
	}

	// Shows card animation of given card
	public void DisplayCard(Card card) {
		origCard = card.baseCard;
		cardAnimation.runtimeAnimatorController = origCard.cardAnimation;
		cardAnimation.gameObject.SetActive (true);
	}

	// Removes card and disables the display
	public void RemoveCard() {
		if (origCard != null) {
			cardAnimation.gameObject.SetActive (false);
			origCard = null;
		}
	}

	// When mouse is hovering, show a zoomed version of the card with stats
	void OnMouseEnter() {
		if (origCard != null) {
			gm.ShowZoomCard (origCard);
		}
	}

	// When mouse stops hovering, hide the zoomed card
	void OnMouseExit() {
		gm.HideZoomCard ();
	}
}
