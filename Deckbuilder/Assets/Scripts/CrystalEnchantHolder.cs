using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CrystalEnchantHolder : MonoBehaviour {

    public CrystalEnchant enchant;
    public TextMeshProUGUI title;
    public TextMeshProUGUI desc;
    public Image enchGem;

	// Use this for initialization
	void Start () {
        if (enchant != null) {
            enchGem.color = enchant.color;
            title.text = enchant.enchName;
            desc.text = enchant.description;
        }
	}

    public void ChangeEnchant(CrystalEnchant ench) {
        enchant = ench;
        enchGem.color = enchant.color;
        title.text = enchant.enchName;
        desc.text = enchant.description;
    }
}
