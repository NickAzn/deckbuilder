using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpotStats : MonoBehaviour {

	public bool hasUnit = false;
	public int row = 0;
	public int collumn = 0;
	public int health = 0;
	public int damage = 0;

	public GameManager gm;
	public Color hoverColor;
	public GameObject highlighter;

	public Text atkUI;
	public Text hpUI;
	public Animator unitAnimation;
	public Animator spellAnimation;
	public Animator summonAnimation;

	public bool playerSide;
	public Player player;

	public Card origCard;

	SpriteRenderer sr;

	public bool relentless = false;
	public int deathDraw = 0;

	void Start() {
		sr = GetComponent<SpriteRenderer> ();
		spellAnimation.gameObject.SetActive (false);
		summonAnimation.gameObject.SetActive (false);
		UpdateUI ();
	}

	void Update() {
		if (CanUseCard()) {
			highlighter.SetActive (true);
		} else {
			highlighter.SetActive (false);
		}
	}

	//Adds unit to spot with given damage, health, and unit sprite
	public void AddUnit(Card card) {
		if (!hasUnit) {
			StartCoroutine (PlaySummonAnimation ());
			hasUnit = true;
			health = card.health;
			damage = card.attack;

			relentless = card.unitRelentless;
			deathDraw = card.unitDeathDraw;

			unitAnimation.runtimeAnimatorController = card.cardAnimation;
			origCard = card.baseCard;
			UpdateUI ();
		}
	}

	public void UseSpell(Card card) {
		if (hasUnit) {
			StartCoroutine(DelaySpell (card.baseCard));
			spellAnimation.runtimeAnimatorController = card.cardAnimation;
			spellAnimation.gameObject.SetActive (true);
		}
	}

	IEnumerator DelaySpell(Card card) {
		float waitTime = card.cardAnimation.animationClips [0].length * 0.95f;
		yield return new WaitForSeconds (waitTime);
		spellAnimation.gameObject.SetActive (false);

		if (card.spellCardDraw > 0) {
			player.DrawCard (card.spellCardDraw);
		}
		if (card.health > 0) {
			TakeDamage (-card.health);
		}

		if (card.attack > 0) {
			TakeDamage (card.attack);
		}
		if (card.spellSacrifice) {
			RemoveUnit ();
		}
	}

	IEnumerator PlaySummonAnimation() {
		summonAnimation.gameObject.SetActive (true);
		summonAnimation.Play ("Summoning");
		yield return new WaitForSeconds (0.3f);
		summonAnimation.gameObject.SetActive (false);

	}

	//Removes unit from spot
	void RemoveUnit() {
		hasUnit = false;
		unitAnimation.runtimeAnimatorController = null;
		unitAnimation.GetComponent<SpriteRenderer> ().sprite = null;
		health = 0;
		damage = 0;
		relentless = false;
		if (playerSide) {
			if (deathDraw > 0) {
				player.DrawCard (deathDraw);
				deathDraw = 0;
			}
				
			player.DiscardCard (origCard);
		}
		UpdateUI ();
	}

	//Places card on spot
	void OnMouseDown() {
		if (CanUseCard()) {
			player.PlayCard (this);
		}
	}

	bool CanUseCard() {
		if (gm.selectedCard != null) {
			if (gm.isPlayerTurn ()) {
				Card selCard = gm.selectedCard;
				if (player.CanPlayCard (selCard)) {
					if (selCard.playerSideCast && playerSide) {
						if (selCard.isUnit && !hasUnit) {
							return true;
						} else if (selCard.isSpell && hasUnit) {
							return true;
						}
					} else if (selCard.enemySideCast && !playerSide) {
						if (selCard.isUnit && !hasUnit) {
							return true;
						} else if (selCard.isSpell && hasUnit) {
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	//Changes color when hovered over
	void OnMouseOver() {
		sr.color = hoverColor;
		if (hasUnit) {
			gm.ShowZoomCard (origCard);
		}
	}

	//Goes back to original color when mouse stops hovering
	void OnMouseExit() {
		sr.color = Color.white;
		gm.HideZoomCard ();
	}

	//Damages another spot
	public void Attack(SpotStats enemy) {
		enemy.TakeDamage (damage);
	}

	//Take damage, if health is 0 or less, remove unit
	public void TakeDamage(int amount) {
		if (hasUnit) {
			health -= amount;
			UpdateUI ();
			if (health <= 0) {
				RemoveUnit ();
			}
		}
	}

	//Updates spot's display attack and health
	void UpdateUI() {
		if (hasUnit) {
			atkUI.text = damage.ToString ();
			hpUI.text = health.ToString ();
		} else {
			atkUI.text = "";
			hpUI.text = "";
		}
	}
}
