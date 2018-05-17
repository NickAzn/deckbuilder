using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchantManager : MonoBehaviour {

    public GameObject enchantHolder;
    public Crystal[] crystals;
    CrystalEnchant selEnchant;
    CrystalEnchant[] activeEnchants;
    public CrystalEnchantHolder[] activeEnchHold;
    public List<CrystalEnchant> libraryEnchants;
    public Transform libraryUI;
    List<GameObject> enchantHolders;

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
        RefreshUI();
    }

    public void RefreshUI() {
        enchantHolders = new List<GameObject>();
        if (libraryEnchants != null) {
            for (int i = 0; i < libraryEnchants.Count; i++) {
                GameObject holder = Instantiate(enchantHolder);
                RectTransform rt = holder.GetComponent<RectTransform>();
                CrystalEnchantHolder enchHolder = holder.GetComponent<CrystalEnchantHolder>();
                enchHolder.ChangeEnchant(libraryEnchants[i]);
                holder.transform.SetParent(libraryUI);
                rt.anchorMax = new Vector2(0.5f, 1f);
                rt.anchorMin = new Vector2(0.5f, 1f);
                rt.anchoredPosition = new Vector3(0f, -200f * i - 120f, 0f);
                holder.transform.localScale = new Vector3(3f, 3f, 1f);
                enchantHolders.Add(holder);
            }
        }
        if (enchantHolders.Count > 3) {
            RectTransform rt = libraryUI.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(0f, (100f * enchantHolders.Count));
            rt.anchoredPosition = new Vector2(0f, -(50f * enchantHolders.Count));
        }
    }

    //Enchants crystal at index i with selected enchant
    public void EnchantCrystal(int i) {
        if (crystals[i].enchant != null) {
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
            RefreshUI();
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
