using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : MonoBehaviour
{
    // Inspector settings

    public Armors.ArmorName armor_name;
    public int magical_bonus;
    public List<string> additional_properties;

    // properties

    public int ArmorClassEnhancement { get; set; }
    public Armors.ArmorType ArmorWeightClass { get; set; }
    public int BaseArmorClass { get; set; }
    public Item Item { get; set; }
    public int MaximumDexterityBonus { get; set; }
    public int MinimumDexterityBonus { get; set; }
    public int MinimumStrengthProficiency { get; set; }
    public bool StealthDisadvantage { get; set; }


    // Unity


    private void Awake()
    {
        Item = GetComponent<Item>();
        SetStats();
    }


    // public


    public int ArmorClass(int dexterity_proficiency)
    {
        // TODO: shields
        return BaseArmorClass + magical_bonus + Mathf.Clamp(dexterity_proficiency, MinimumDexterityBonus, MaximumDexterityBonus);
    }


    public string DisplayName()
    {
        return armor_name.ToString().Replace("_", " ").ToLower();
    }


    // private


    private void SetStats()
    {
        ArmorClassEnhancement = magical_bonus;

        switch (armor_name) {
            case Armors.ArmorName.Padded:
                Item.base_cost = 5;
                Item.base_weight = 8;
                ArmorWeightClass = Armors.ArmorType.Light;
                BaseArmorClass = 11;
                MaximumDexterityBonus = 10;
                MinimumDexterityBonus = -10;
                StealthDisadvantage = true;
                break;
            case Armors.ArmorName.Leather:
                Item.base_cost = 10;
                Item.base_weight = 10;
                ArmorWeightClass = Armors.ArmorType.Light;
                BaseArmorClass = 11;
                MaximumDexterityBonus = 10;
                MinimumDexterityBonus = -10;
                StealthDisadvantage = false;
                break;
            case Armors.ArmorName.Studded_Leather:
                Item.base_cost = 45;
                Item.base_weight = 13;
                ArmorWeightClass = Armors.ArmorType.Light;
                BaseArmorClass = 12;
                MaximumDexterityBonus = 10;
                MinimumDexterityBonus = -10;
                StealthDisadvantage = false;
                break;
            case Armors.ArmorName.Hide:
                Item.base_cost = 10;
                Item.base_weight = 12;
                ArmorWeightClass = Armors.ArmorType.Medium;
                BaseArmorClass = 12;
                MaximumDexterityBonus = 2;
                MinimumDexterityBonus = -10;
                StealthDisadvantage = false;
                break;
            case Armors.ArmorName.Chain_Shirt:
                Item.base_cost = 50;
                Item.base_weight = 20;
                ArmorWeightClass = Armors.ArmorType.Medium;
                BaseArmorClass = 13;
                MaximumDexterityBonus = 2;
                MinimumDexterityBonus = -10;
                StealthDisadvantage = false;
                break;
            case Armors.ArmorName.Scale_Mail:
                Item.base_cost = 50;
                Item.base_weight = 45;
                ArmorWeightClass = Armors.ArmorType.Medium;
                BaseArmorClass = 14;
                MaximumDexterityBonus = 2;
                StealthDisadvantage = true;
                break;
            case Armors.ArmorName.Breastplate:
                Item.base_cost = 400;
                Item.base_weight = 20;
                ArmorWeightClass = Armors.ArmorType.Medium;
                BaseArmorClass = 14;
                MaximumDexterityBonus = 2;
                MinimumDexterityBonus = -10;
                StealthDisadvantage = false;
                break;
            case Armors.ArmorName.Half_Plate:
                Item.base_cost = 750;
                Item.base_weight = 40;
                ArmorWeightClass = Armors.ArmorType.Medium;
                BaseArmorClass = 15;
                MaximumDexterityBonus = 2;
                MinimumDexterityBonus = -10;
                StealthDisadvantage = true;
                break;
            case Armors.ArmorName.Ring_Mail:
                Item.base_cost = 30;
                Item.base_weight = 40;
                ArmorWeightClass = Armors.ArmorType.Heavy;
                BaseArmorClass = 14;
                MaximumDexterityBonus = 0;
                MinimumDexterityBonus = 0;
                StealthDisadvantage = true;
                break;
            case Armors.ArmorName.Chain_Mail:
                Item.base_cost = 75;
                Item.base_weight = 55;
                ArmorWeightClass = Armors.ArmorType.Heavy;
                BaseArmorClass = 16;
                MaximumDexterityBonus = 0;
                MinimumDexterityBonus = 0;
                StealthDisadvantage = true;
                break;
            case Armors.ArmorName.Splint:
                Item.base_cost = 200;
                Item.base_weight = 60;
                ArmorWeightClass = Armors.ArmorType.Heavy;
                BaseArmorClass = 17;
                MaximumDexterityBonus = 0;
                MinimumDexterityBonus = 0;
                StealthDisadvantage = true;
                break;
            case Armors.ArmorName.Plate:
                Item.base_cost = 1500;
                Item.base_weight = 65;
                ArmorWeightClass = Armors.ArmorType.Heavy;
                BaseArmorClass = 18;
                MaximumDexterityBonus = 0;
                MinimumDexterityBonus = 0;
                StealthDisadvantage = true;
                break;
            case Armors.ArmorName.Shield:
                Item.base_cost = 10;
                Item.base_weight = 6;
                ArmorClassEnhancement += 2;
                break;
            case Armors.ArmorName.None:
                Item.base_cost = 0;
                Item.base_weight = 0;
                BaseArmorClass = 10;
                MaximumDexterityBonus = 10;
                MinimumDexterityBonus = -10;
                StealthDisadvantage = false;
                break;
        }
    }
}
