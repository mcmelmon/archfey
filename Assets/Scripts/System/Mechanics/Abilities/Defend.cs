using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defend : MonoBehaviour
{
    public int agility_rating;    // move out of harm's way
    public int armor_rating;      // deflect incoming attacks
    public int constitution_rating;  // walk it off
    public int force_rating;      // deflect incoming attacks

    public Dictionary<Weapon.DamageType, int> resistances;

    float computed_damage;       // final health loss

    Attack attacker;
    Weapon weapon;


    // Unity


    private void OnValidate()
    {
        if (agility_rating > 10) agility_rating = 10;
        if (agility_rating < 0) agility_rating = 0;

        if (armor_rating > 10) armor_rating = 10;
        if (armor_rating < 0) armor_rating = 0;

        if (force_rating > 10) force_rating = 10;
        if (force_rating < 0) force_rating = 0;
    }


    // public


    public float HandleAttack(Weapon _weapon, Attack _attacker)
    {
        // Apply our defense characteristics to an attack and compute damage
        attacker = _attacker;
        weapon = _weapon;
        computed_damage = weapon.instant_damage + attacker.AgilityRating + attacker.StrengthRating;  // TODO: handle damage over time

        ApplyAgility();
        ApplyConstitution();
        ApplyForce();
        ApplyArmor();
        ApplyResistance();

        return computed_damage;
    }


    public void SetResistances(Dictionary<Weapon.DamageType, int> _resistances)
    {
        resistances = _resistances;
    }


    // private


    private void ApplyArmor()
    {
        // Armor rating deflects some damage

        if (computed_damage <= 0) return;

        switch (weapon.damage_type)
        {
            case Weapon.DamageType.Blunt:
                computed_damage -= armor_rating * 1.2f;
                break;
            case Weapon.DamageType.Piercing:
                computed_damage -= armor_rating;
                break;
            case Weapon.DamageType.Slashing:
                computed_damage -= armor_rating * 2f;
                break;
            case Weapon.DamageType.Elemental:
                computed_damage -= armor_rating * 0.5f;
                break;
            default:
                break;
        }
    }


    private void ApplyAgility()
    {
        // Agility rating avoids some damage by shifting location struck

        if (computed_damage <= 0) return;

        switch (weapon.damage_type) {
            case Weapon.DamageType.Blunt:
                computed_damage -= agility_rating;
                break;
            case Weapon.DamageType.Piercing:
                computed_damage -= agility_rating * 1.2f;
                break;
            case Weapon.DamageType.Slashing:
                computed_damage -= agility_rating * 1.2f;
                break;
            case Weapon.DamageType.Elemental:
                computed_damage -= agility_rating * 0.5f;
                break;
            default:
                break;
        }
    }


    private void ApplyConstitution()
    {
        // Constitution resists pain and poison

        if (computed_damage <= 0) return;

        switch (weapon.damage_type)
        {
            case Weapon.DamageType.Blunt:
                computed_damage -= constitution_rating * 0.5f;
                break;
            case Weapon.DamageType.Piercing:
                computed_damage -= constitution_rating * 0.5f;
                break;
            case Weapon.DamageType.Poison:
                computed_damage -= constitution_rating * 1.2f;
                break;
            case Weapon.DamageType.Slashing:
                computed_damage -= constitution_rating * 0.5f;
                break;
            default:
                break;
        }

    }


    private void ApplyForce()
    {
        // Force rating deflects some damage

        if (computed_damage <= 0) return;

        switch (weapon.damage_type) {
            case Weapon.DamageType.Blunt:
                computed_damage -= force_rating;
                break;
            case Weapon.DamageType.Piercing:
                computed_damage -= force_rating;
                break;
            case Weapon.DamageType.Slashing:
                computed_damage -= force_rating;
                break;
            case Weapon.DamageType.Elemental:
                computed_damage -= force_rating * 2f;
                break;
            default:
                break;
        }
    }


    private void ApplyResistance()
    {
        if (computed_damage <= 0 || resistances == null) return;

        computed_damage -= computed_damage * (resistances[weapon.damage_type] / 100);
    }
}
