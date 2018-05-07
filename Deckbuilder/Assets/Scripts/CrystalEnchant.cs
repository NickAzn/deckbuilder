using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "New Enchant", menuName = "Crystal Enchant")]
public class CrystalEnchant : ScriptableObject {

    public new string name;
    [TextArea]
    public string description;

    public Color color = Color.white;

    //Play type - When > 0, do effect after var number of cards on played by the player
    public int playDiscardDraw = 0; // The player will draw from the discard pile after playing playDiscardDraw
    public int playHealthBuff = 0;  // The player will increase health of a random unit on their side by 1
    public int playDamageBuff = 0;  // The player will increase damage of a random unit on their side by 1
}
