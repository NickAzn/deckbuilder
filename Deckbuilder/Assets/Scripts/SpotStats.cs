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
	public EnchantHolder enchantment;

	public bool playerSide;
	public Player player;

	public Card origCard;

	SpriteRenderer sr;

	public bool relentless = false;
	public int deathDraw = 0;
	public int fury = 0;

	void Start() {
		sr = GetComponent<SpriteRenderer> ();
		spellAnimation.gameObject.SetActive (false);
		summonAnimation.gameObject.SetActive (false);
		UpdateUI ();
	}

	public void NewTurn() {
		if (origCard != null) {
			if (origCard.unitFury > 0) {
				fury += origCard.unitFury;
			}
			if (enchantment.origCard != null) {
				if (enchantment.origCard.unitFury > 0) {
					fury += enchantment.origCard.unitFury;
				}
			}
		}
	}
		
	void Update() {
		// If the player can use selected card here, highlight this spot
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
			fury = card.unitFury;

			if (playerSide) {
				if (card.unitManaFont > 0) {
					player.IncreaseMaxMana (card.unitManaFont);
				}
			}

			unitAnimation.runtimeAnimatorController = card.cardAnimation;
			origCard = card.baseCard;
			UpdateUI ();
		}
	}

	// Runs spell animation are delays spell effect until animation finishes
	public void UseSpell(Card card) {
		if (hasUnit) {
			StartCoroutine(DelaySpell (card.baseCard));
			spellAnimation.runtimeAnimatorController = card.cardAnimation;
			spellAnimation.gameObject.SetActive (true);
		}
	}

	// Delays spell effect to allow animation to finish
	IEnumerator DelaySpell(Card card) {
		//Hide spell animation after it finishes
		float waitTime = card.cardAnimation.animationClips [0].length * 0.95f;
		yield return new WaitForSeconds (waitTime);
		spellAnimation.gameObject.SetActive (false);

		//If the spell is not an enchant, it is an instant spell
		if (!card.spellEnchant) {
			if (card.spellCardDraw > 0) {
				player.DrawCard (card.spellCardDraw);	//Draw cards equal to spellCardDraw
			}
			if (card.attack > 0) {
				TakeDamage (card.attack);	//Take damage based equal to the card's attack
			}
			if (card.spellSacrifice) {
				RemoveUnit ();				//Sacrifices this unit if the spell has spellSacrifice true
			}
			if (card.spellAttackReduce > 0) {
				damage -= card.spellAttackReduce;	// Reduce units attack by spellAttackReduce effect of card
			}
			//Discard to player discards if the player casted this card
			if (!playerSide && card.enemySideCast) {
				player.DiscardCard (card);
			} else if (playerSide && card.playerSideCast) {
				player.DiscardCard (card);
			}
		} else {
			enchantment.DisplayCard (card.baseCard);	//Spell is an enchantent, so it uses the enchant spot
			if (card.health > 0) {
				health += card.health;	// Increase unit's health by enchantment's health
			}
			if (card.attack > 0) {
				damage += card.attack;	// Increase unit's damage by the enchantment's attack
			}
			if (card.spellAttackReduce > 0) {
				damage -= card.spellAttackReduce;	// Reduce units attack by spellAttackReduce effect of card
			}
			if (card.unitRelentless) {
				relentless = true;		// Gives unit relentless, if the enchant has relentless
			}
			if (card.unitFury > 0) {
				fury += card.unitFury;
			}
			UpdateUI ();	// Update with new stats for enchantment
		}
	}

	// Plays the summoning animation then hides it after it finishes
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
		fury = 0;
		if (playerSide) {
			if (deathDraw > 0) {
				player.DrawCard (deathDraw);
				deathDraw = 0;
			}
			if (origCard.unitManaFont > 0) {
				player.IncreaseMaxMana (-origCard.unitManaFont);
			}

			if (enchantment.origCard != null) {
				player.DiscardCard (enchantment.origCard);
			}
				
			player.DiscardCard (origCard);
		}
		enchantment.RemoveCard ();
		UpdateUI ();
	}

	//Places card on spot
	void OnMouseDown() {
		if (CanUseCard()) {
			player.PlayCard (this);
		}
	}

	// Check if the selected card can be used on this spot
	bool CanUseCard() {
		if (gm.selectedCard != null) {
			if (gm.isPlayerTurn ()) {
				Card selCard = gm.selectedCard;
				if (player.CanPlayCard (selCard)) {
					if (selCard.playerSideCast && playerSide) {
						if (selCard.isUnit && !hasUnit) {
							return true;
						} else if (selCard.spellEnchant && hasUnit) {
							if (enchantment.origCard == null) {
								return true;
							} else {
								return false;
							}
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
