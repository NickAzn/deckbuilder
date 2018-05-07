using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Crystal : MonoBehaviour {

	public TextMeshProUGUI hpUI;
    public Image enchantUI;
    public TextMeshProUGUI enchantUIText;
    public ParticleSystem enchantParticles;
    public CrystalEnchant enchant;
	int health = 10;

	public Sprite[] healthStates;
	SpriteRenderer sr;

	void Awake() {
		sr = GetComponent<SpriteRenderer> ();
	}

	void Start() {
        if (enchant != null) {
            AddEnchant(enchant);
        }
        UpdateUI();
    }

    //Adds enchant to crystal
    public void AddEnchant(CrystalEnchant newEnchant) {
        enchant = newEnchant;
        ParticleSystem.MainModule particleSettings = enchantParticles.main;
        particleSettings.startColor = enchant.color;
        enchantUI.color = enchant.color;
        enchantParticles.gameObject.SetActive(true);
        enchantUI.gameObject.SetActive(true);
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
            if (enchant != null)
                enchantParticles.gameObject.SetActive(false);
		} else if (health <= 5) {
			sr.sprite = healthStates [1];
		} else {
			sr.sprite = healthStates [0];
		}
        hpUI.text = health.ToString();
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