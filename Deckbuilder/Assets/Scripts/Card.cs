using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {

	GameManager gm;

	public bool playerSideCast;
	public bool enemySideCast;

	public int attack;
	public int health;

	void Start() {
		gm = GameObject.Find ("GameManager").GetComponent<GameManager> ();
	}

	void OnMouseDown() {
		gm.selectedCard = this;
	}
}
