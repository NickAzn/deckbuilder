using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Crystal : MonoBehaviour {

	public TextMeshProUGUI hpUI;
	int health = 10;

	public Sprite[] healthStates;
	SpriteRenderer sr;

	void Awake() {
		sr = GetComponent<SpriteRenderer> ();
	}

	void Start() {
		UpdateUI ();

	}

	// Crystal takes given amount of damage
	public void TakeDamage(int amount) {
		health -= amount;
		if (health <= 0) {
			health = 0;
		}
		UpdateUI();
	}

	// Updates the health text UI and sprite to full state, damaged state, or destroyed state
	void UpdateUI() {
		if (health <= 0) {
			sr.sprite = healthStates [2];
		} else if (health <= 5) {
			sr.sprite = healthStates [1];
		} else {
			sr.sprite = healthStates [0];
		}
		hpUI.text = health.ToString ();
	}

	// Checks if the crystal has more than 0 health
	public bool isAlive() {
		if (health > 0) {
			return true;
		}
		return false;
	}

	// Sets the amount of health the crystal has
	public void SetHealth(int amount) {
		health = amount;
		UpdateUI ();
	}

	// Get the amount of health the crystal has
	public int GetHealth() {
		return health;
	}
}