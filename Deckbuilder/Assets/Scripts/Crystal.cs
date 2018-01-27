using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crystal : MonoBehaviour {

	public GameManager gm;
	public Text hpUI;
	int health = 10;

	public Sprite[] healthStates;
	SpriteRenderer sr;

	void Start() {
		sr = GetComponent<SpriteRenderer> ();
		UpdateUI ();
	}

	public void TakeDamage(int amount) {
		health -= amount;
		if (health <= 0) {
			health = 0;
		}
		UpdateUI();
	}

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

	public bool isAlive() {
		if (health > 0) {
			return true;
		}
		return false;
	}
}
