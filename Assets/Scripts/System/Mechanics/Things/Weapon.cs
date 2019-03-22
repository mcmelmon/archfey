using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    // Inspector settings

    public Weapons.WeaponName weapon_name;
    public string qualifier;
    public int magic_attack_bonus;
    public int magic_damage_bonus;
    public GameObject projectile_prefab;

    // properties

    public Weapons.DamageType DamageType { get; set; }
    public int DamageBonus { get; set; }
    public int DiceType { get; set; }
    public bool HasAmmunition { get; set; }
    public bool HasReach { get; set; }
    public int HitBonus { get; set; }
    public bool IsFinesse { get; set; }
    public bool IsHeavy { get; set; }
    public bool IsLight { get; set; }
    public bool IsLoaded { get; set; }
    public bool IsMagic { get; set; }
    public bool IsSilvered { get; set; }
    public bool IsThrown { get; set; }
    public bool IsTwoHanded { get; set; }
    public bool IsVersatile { get; set; }
    public Item Item { get; set; }
    public int NumberOfDice { get; set; }
    public float Range { get; set; }
    public Weapons.WeaponType WeaponType { get; set; }


    // Unity


    private void Awake()
    {
        Item = GetComponent<Item>();
        SetStats();
    }


    // private


    private void SetStats()
    {
        DamageBonus = magic_damage_bonus;
        HitBonus = magic_attack_bonus;
        Range = 0;

        switch (weapon_name) {
            case Weapons.WeaponName.Club:
                Item.base_cost = 0.1f; // use Industry.Coin
                Item.base_weight = 2;
                DamageType = Weapons.DamageType.Bludgeoning;
                DiceType = 4;
                NumberOfDice = 1;
                IsLight = true;
                WeaponType = Weapons.WeaponType.Simple_Melee;
                break;
            case Weapons.WeaponName.Dagger:
                Item.base_cost = 2;
                Item.base_weight = 1;
                DamageType = Weapons.DamageType.Piercing;
                DiceType = 4;
                NumberOfDice = 1;
                IsFinesse = true;
                IsLight = true;
                IsThrown = true;
                Range = 8;
                WeaponType = Weapons.WeaponType.Simple_Melee;
                break;
            case Weapons.WeaponName.Greatclub:
                Item.base_cost = 0.2f;
                Item.base_weight = 10;
                DamageType = Weapons.DamageType.Bludgeoning;
                DiceType = 8;
                NumberOfDice = 1;
                IsTwoHanded = true;
                WeaponType = Weapons.WeaponType.Simple_Melee;
                break;
            case Weapons.WeaponName.Handaxe:
                Item.base_cost = 5;
                Item.base_weight = 2;
                DamageType = Weapons.DamageType.Slashing;
                DiceType = 6;
                NumberOfDice = 1;
                IsLight = true;
                IsThrown = true;
                Range = 8;
                WeaponType = Weapons.WeaponType.Simple_Melee;
                break;
            case Weapons.WeaponName.Javelin:
                Item.base_cost = 0.5f;
                Item.base_weight = 2;
                DamageType = Weapons.DamageType.Piercing;
                DiceType = 6;
                NumberOfDice = 1;
                IsThrown = true;
                Range = 10;
                WeaponType = Weapons.WeaponType.Simple_Melee;
                break;
            case Weapons.WeaponName.Light_Hammer:
                Item.base_cost = 2;
                Item.base_weight = 2;
                DamageType = Weapons.DamageType.Bludgeoning;
                DiceType = 4;
                NumberOfDice = 1;
                IsLight = true;
                IsThrown = true;
                Range = 5;
                WeaponType = Weapons.WeaponType.Simple_Melee;
                break;
            case Weapons.WeaponName.Mace:
                Item.base_cost = 5;
                Item.base_weight = 4;
                DamageType = Weapons.DamageType.Bludgeoning;
                DiceType = 6;
                NumberOfDice = 1;
                WeaponType = Weapons.WeaponType.Simple_Melee;
                break;
            case Weapons.WeaponName.Quarterstaff:
                Item.base_cost = 0.2f;
                Item.base_weight = 4;
                DamageType = Weapons.DamageType.Bludgeoning;
                DiceType = 6;
                NumberOfDice = 1;
                IsVersatile = true;
                WeaponType = Weapons.WeaponType.Simple_Melee;
                break;
            case Weapons.WeaponName.Sickle:
                Item.base_cost = 1;
                Item.base_weight = 4;
                DamageType = Weapons.DamageType.Slashing;
                DiceType = 4;
                NumberOfDice = 1;
                IsLight = true;
                WeaponType = Weapons.WeaponType.Simple_Melee;
                break;
            case Weapons.WeaponName.Spear:
                Item.base_cost = 1;
                Item.base_weight = 3;
                DamageType = Weapons.DamageType.Piercing;
                DiceType = 6;
                NumberOfDice = 1;
                IsThrown = true;
                IsVersatile = true;
                Range = 8;
                WeaponType = Weapons.WeaponType.Simple_Melee;
                break;
            case Weapons.WeaponName.Light_Crossbow:
                Item.base_cost = 25;
                Item.base_weight = 5;
                DamageType = Weapons.DamageType.Piercing;
                DiceType = 8;
                NumberOfDice = 1;
                HasAmmunition = true;
                IsLoaded = true;
                IsTwoHanded = true;
                Range = 15;
                WeaponType = Weapons.WeaponType.Simple_Ranged;
                break;
            case Weapons.WeaponName.Dart:
                Item.base_cost = 0.05f;
                Item.base_weight = 0.25f;
                DamageType = Weapons.DamageType.Piercing;
                DiceType = 4;
                NumberOfDice = 1;
                IsFinesse = true;
                IsThrown = true;
                Range = 6;
                WeaponType = Weapons.WeaponType.Simple_Ranged;
                break;
            case Weapons.WeaponName.Shortbow:
                Item.base_cost = 25;
                Item.base_weight = 2;
                DamageType = Weapons.DamageType.Piercing;
                DiceType = 6;
                NumberOfDice = 1;
                HasAmmunition = true;
                IsTwoHanded = true;
                Range = 17;
                WeaponType = Weapons.WeaponType.Simple_Ranged;
                break;
            case Weapons.WeaponName.Sling:
                Item.base_cost = 0.1f;
                Item.base_weight = 0.01f;
                DamageType = Weapons.DamageType.Bludgeoning;
                DiceType = 4;
                NumberOfDice = 1;
                HasAmmunition = true;
                Range = 8;
                WeaponType = Weapons.WeaponType.Simple_Ranged;
                break;
            case Weapons.WeaponName.Battleaxe:
                Item.base_cost = 10;
                Item.base_weight = 4;
                DamageType = Weapons.DamageType.Slashing;
                DiceType = 8;
                NumberOfDice = 1;
                IsVersatile = true;
                WeaponType = Weapons.WeaponType.Martial_Melee;
                break;
            case Weapons.WeaponName.Flail:
                Item.base_cost = 10;
                Item.base_weight = 2;
                DamageType = Weapons.DamageType.Bludgeoning;
                DiceType = 8;
                NumberOfDice = 1;
                WeaponType = Weapons.WeaponType.Martial_Melee;
                break;
            case Weapons.WeaponName.Glaive:
                Item.base_cost = 20;
                Item.base_weight = 6;
                DamageType = Weapons.DamageType.Slashing;
                DiceType = 10;
                NumberOfDice = 1;
                HasReach = true;
                IsHeavy = true;
                IsTwoHanded = true;
                WeaponType = Weapons.WeaponType.Martial_Melee;
                break;
            case Weapons.WeaponName.Greataxe:
                Item.base_cost = 30;
                Item.base_weight = 7;
                DamageType = Weapons.DamageType.Slashing;
                DiceType = 12;
                NumberOfDice = 1;
                IsHeavy = true;
                IsTwoHanded = true;
                WeaponType = Weapons.WeaponType.Martial_Melee;
                break;
            case Weapons.WeaponName.Halberd:
                Item.base_cost = 20;
                Item.base_weight = 6;
                DamageType = Weapons.DamageType.Slashing;
                DiceType = 10;
                NumberOfDice = 1;
                HasReach = true;
                IsHeavy = true;
                IsTwoHanded = true;
                WeaponType = Weapons.WeaponType.Martial_Melee;
                break;
            case Weapons.WeaponName.Lance:
                Item.base_cost = 10;
                Item.base_weight = 6;
                DamageType = Weapons.DamageType.Piercing;
                DiceType = 12;
                NumberOfDice = 1;
                HasReach = true;
                WeaponType = Weapons.WeaponType.Martial_Melee;
                break;
            case Weapons.WeaponName.Longsword:
                Item.base_cost = 15;
                Item.base_weight = 3;
                DamageType = Weapons.DamageType.Slashing;
                DiceType = 8;
                NumberOfDice = 1;
                IsVersatile = true;
                WeaponType = Weapons.WeaponType.Martial_Melee;
                break;
            case Weapons.WeaponName.Maul:
                Item.base_cost = 10;
                Item.base_weight = 10;
                DamageType = Weapons.DamageType.Bludgeoning;
                DiceType = 6;
                NumberOfDice = 2;
                IsHeavy = true;
                IsTwoHanded = true;
                WeaponType = Weapons.WeaponType.Martial_Melee;
                break;
            case Weapons.WeaponName.Morningstar:
                Item.base_cost = 15;
                Item.base_weight = 4;
                DamageType = Weapons.DamageType.Piercing;
                DiceType = 8;
                NumberOfDice = 1;
                WeaponType = Weapons.WeaponType.Martial_Melee;
                break;
            case Weapons.WeaponName.Pike:
                Item.base_cost = 5;
                Item.base_weight = 18;
                DamageType = Weapons.DamageType.Piercing;
                DiceType = 10;
                NumberOfDice = 1;
                HasReach = true;
                IsHeavy = true;
                IsTwoHanded = true;
                WeaponType = Weapons.WeaponType.Martial_Melee;
                break;
            case Weapons.WeaponName.Rapier:
                Item.base_cost = 25;
                Item.base_weight = 2;
                DamageType = Weapons.DamageType.Piercing;
                DiceType = 8;
                NumberOfDice = 1;
                IsFinesse = true;
                WeaponType = Weapons.WeaponType.Martial_Melee;
                break;
            case Weapons.WeaponName.Scimitar:
                Item.base_cost = 25;
                Item.base_weight = 3;
                DamageType = Weapons.DamageType.Slashing;
                DiceType = 6;
                NumberOfDice = 1;
                IsFinesse = true;
                IsLight = true;
                WeaponType = Weapons.WeaponType.Martial_Melee;
                break;
            case Weapons.WeaponName.Shortsword:
                Item.base_cost = 10;
                Item.base_weight = 2;
                DamageType = Weapons.DamageType.Piercing;
                DiceType = 6;
                NumberOfDice = 1;
                IsFinesse = true;
                IsLight = true;
                WeaponType = Weapons.WeaponType.Martial_Melee;
                break;
            case Weapons.WeaponName.Trident:
                Item.base_cost = 5;
                Item.base_weight = 4;
                DamageType = Weapons.DamageType.Piercing;
                DiceType = 6;
                NumberOfDice = 1;
                IsThrown = true;
                IsVersatile = true;
                Range = 5;
                WeaponType = Weapons.WeaponType.Martial_Melee;
                break;
            case Weapons.WeaponName.War_Pick:
                Item.base_cost = 5;
                Item.base_weight = 2;
                DamageType = Weapons.DamageType.Piercing;
                DiceType = 8;
                NumberOfDice = 1;
                WeaponType = Weapons.WeaponType.Martial_Melee;
                break;
            case Weapons.WeaponName.Warhammer:
                Item.base_cost = 15;
                Item.base_weight = 2;
                DamageType = Weapons.DamageType.Bludgeoning;
                DiceType = 8;
                NumberOfDice = 1;
                IsVersatile = true;
                WeaponType = Weapons.WeaponType.Martial_Melee;
                break;
            case Weapons.WeaponName.Whip:
                Item.base_cost = 2;
                Item.base_weight = 3;
                DamageType = Weapons.DamageType.Slashing;
                DiceType = 4;
                NumberOfDice = 1;
                HasReach = true;
                IsFinesse = true;
                WeaponType = Weapons.WeaponType.Martial_Melee;
                break;
            case Weapons.WeaponName.Blowgun:
                Item.base_cost = 10;
                Item.base_weight = 1;
                DamageType = Weapons.DamageType.Piercing;
                DiceType = 1;
                NumberOfDice = 1;
                HasAmmunition = true;
                IsLoaded = true;
                Range = 5;
                WeaponType = Weapons.WeaponType.Martial_Ranged;
                break;
            case Weapons.WeaponName.Hand_Crossbow:
                Item.base_cost = 75;
                Item.base_weight = 3;
                DamageType = Weapons.DamageType.Piercing;
                DiceType = 6;
                NumberOfDice = 1;
                HasAmmunition = true;
                IsLight = true;
                IsLoaded = true;
                Range = 7;
                WeaponType = Weapons.WeaponType.Martial_Ranged;
                break;
            case Weapons.WeaponName.Heavy_Crossbow:
                Item.base_cost = 50;
                Item.base_weight = 18;
                DamageType = Weapons.DamageType.Piercing;
                DiceType = 10;
                NumberOfDice = 1;
                HasAmmunition = true;
                IsHeavy = true;
                IsLoaded = true;
                IsTwoHanded = true;
                Range = 20;
                WeaponType = Weapons.WeaponType.Martial_Ranged;
                break;
            case Weapons.WeaponName.Longbow:
                Item.base_cost = 50;
                Item.base_weight = 2;
                DamageType = Weapons.DamageType.Piercing;
                DiceType = 8;
                NumberOfDice = 1;
                HasAmmunition = true;
                IsHeavy = true;
                IsTwoHanded = true;
                Range = 20;
                WeaponType = Weapons.WeaponType.Martial_Ranged;
                break;
            case Weapons.WeaponName.Net:
                Item.base_cost = 1;
                Item.base_weight = 3;
                DamageType = Weapons.DamageType.Bludgeoning;
                DiceType = 0;
                NumberOfDice = 0;
                IsThrown = true;
                Range = 3;
                WeaponType = Weapons.WeaponType.Martial_Ranged;
                break;
            case Weapons.WeaponName.Shield:
                DamageType = Weapons.DamageType.Bludgeoning;
                DiceType = 4;
                NumberOfDice = 1;
                WeaponType = Weapons.WeaponType.Simple_Melee;
                break;
        }
    }

}
