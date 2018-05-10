using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "New Enchant", menuName = "Crystal Enchant")]
public class CrystalEnchant : ScriptableObject {

    public string enchName;
    [TextArea]
    public string description;

    public Color color = Color.white;

    //Play type - When > 0, do effect after playType number of cards on played by the player
    public int playType = 0;
    public int playDiscardDraw = 0; // The player will draw playDiscardDraw cards from discards every playType cards played
    public int playHealthBuff = 0;  // The player will increase health of a random unit on their side by playHealthBuff
    public int playDamageBuff = 0;  // The player will increase damage of a random unit on their side by playDamageBuff
}
