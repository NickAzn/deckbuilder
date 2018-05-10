using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchantManager : MonoBehaviour {

    public Crystal[] crystals;
    CrystalEnchant selEnchant;
    CrystalEnchant[] activeEnchants;
    public CrystalEnchantHolder[] activeEnchHold;
    List<CrystalEnchant> libraryEnchants;

	void Start () {
        //Load Enchants
        activeEnchants = SaveLoad.LoadActiveCrystalEnchants();
        libraryEnchants = SaveLoad.LoadCrystalEnchantLibrary();
        if (libraryEnchants == null) {
            libraryEnchants = new List<CrystalEnchant>();
        }

        //Set crystals to have current enchants
        if (activeEnchants[0] != null) {
            selEnchant = activeEnchants[0];
            EnchantCrystal(0);
        }
        if (activeEnchants[1] != null) {
            selEnchant = activeEnchants[1];
            EnchantCrystal(1);
        }
        if (activeEnchants[2] != null) {
            selEnchant = activeEnchants[2];
            EnchantCrystal(2);
        }
        selEnchant = null;
    }

    //Enchants crystal at index i with selected enchant
    public void EnchantCrystal(int i) {
        if (crystals[i] != null) {
            libraryEnchants.Add(crystals[i].enchant);
        }

        crystals[i].AddEnchant(selEnchant);
        libraryEnchants.Remove(selEnchant);
        activeEnchHold[i].ChangeEnchant(selEnchant);
        activeEnchHold[i].gameObject.SetActive(true);
        SaveEnchants();
    }

    //Removes enchant from crystal at index i and adds it to library
    public void RemoveEnchant(int i) {
        if (crystals[i] != null) {
            libraryEnchants.Add(crystals[i].enchant);
            crystals[i].RemoveEnchant();
            activeEnchHold[i].gameObject.SetActive(false);
            SaveEnchants();
        }
    }

    public void SelectEnchant(CrystalEnchant ench) {
        selEnchant = ench;
    }

    //Save active and library enchants
    void SaveEnchants() {
        SaveLoad.SaveActiveCrystalEnchants(crystals[0].enchant, crystals[1].enchant, crystals[2].enchant);
        SaveLoad.SaveCrystalEnchantLibrary(libraryEnchants);
    }
}
