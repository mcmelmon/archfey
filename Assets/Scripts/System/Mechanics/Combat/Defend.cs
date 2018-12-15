using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defend : MonoBehaviour
{
    public float agility_rating;    // move out of harm's way to some extent
    public float armor_rating;      // percentage reduction of incoming damage
    public float corporeal_rating;  // ignore some damage
    public float counter;            // damage returned to attackers
    public float force_rating;      // percentage reduction of non DoT

    public Dictionary<Weapon.Type, float> resistances;

    float computed_damage;       // final health loss

    GameObject attacker;
    Weapon weapon;


    // Unity


    private void OnValidate()
    {
        if (corporeal_rating > 100f) corporeal_rating = 100f;
        if (corporeal_rating < 0f) corporeal_rating = 0f;

        if (agility_rating > 100f) agility_rating = 100f;
        if (agility_rating < 0f) agility_rating = 0f;

        if (armor_rating > 100f) armor_rating = 100f;
        if (armor_rating < 0f) armor_rating = 0f;

        if (force_rating > 100f) force_rating = 100f;
        if (force_rating < 0f) force_rating = 0f;
    }


    // public


    public float HandleAttack(Weapon _weapon, GameObject _attacker)
    {
        // Apply our defense characteristics to an attack and compute damage
        attacker = _attacker;
        weapon = _weapon;
        computed_damage = weapon.instant_damage;  // TODO: handle damage over time

        ApplyAgility();
        ApplyCorporeal();
        ApplyForce();
        ApplyArmor();
        ApplyResistance();

        return computed_damage;
    }


    public float GetCounterDamage()
    {
        return counter;  // TODO: give counter a "weapon" so that it can be handled by defend
    }


    public void SetAgilityRating(float rate)
    {
        agility_rating = rate;
    }


    public void SetArmorRating(float rate)
    {
        armor_rating = rate;
    }


    public void SetCorporealRating(float rate)
    {
        corporeal_rating = rate;
    }


    public void SetCounter(float rate)
    {
        counter = rate;
    }


    public void SetForceRating(float rate)
    {
        force_rating = rate;
    }


    public void SetResistances(Dictionary<Weapon.Type, float> _resistances)
    {
        resistances = _resistances;
    }


    // private


    private void ApplyArmor()
    {
        if (computed_damage <= 0) return;

        switch (weapon.GetType())
        {
            case Weapon.Type.Blunt:
                computed_damage -= armor_rating * computed_damage;
                break;
            case Weapon.Type.Piercing:
                computed_damage -= (armor_rating * computed_damage) * 0.8f;  // TODO: configure type effect on armor
                break;
            case Weapon.Type.Slashing:
                computed_damage -= (armor_rating * computed_damage) * 1.2f;
                break;
            case Weapon.Type.Elemental:
                computed_damage -= (armor_rating * computed_damage) * 0.4f;
                break;
            default:
                break;
        }
    }


    private void ApplyAgility()
    {
        if (computed_damage <= 0) return;

        switch (weapon.GetType()) {
            case Weapon.Type.Blunt:
                computed_damage -= agility_rating * computed_damage;
                break;
            case Weapon.Type.Piercing:
                computed_damage -= (armor_rating * computed_damage) * 1.5f;
                break;
            case Weapon.Type.Slashing:
                computed_damage -= (armor_rating * computed_damage) * 1.2f;
                break;
            case Weapon.Type.Elemental:
                computed_damage -= (armor_rating * computed_damage) * 0.2f;
                break;
            default:
                break;
        }
    }


    private void ApplyCorporeal()
    {
        if (computed_damage <= 0) return;

        computed_damage -= corporeal_rating * computed_damage;

    }


    private void ApplyForce()
    {
        if (computed_damage <= 0) return;

        switch (weapon.GetType()) {
            case Weapon.Type.Blunt:
                computed_damage -= force_rating * computed_damage;
                break;
            case Weapon.Type.Piercing:
                computed_damage -= (force_rating * computed_damage);
                break;
            case Weapon.Type.Slashing:
                computed_damage -= (force_rating * computed_damage);
                break;
            case Weapon.Type.Elemental:
                computed_damage -= (force_rating * computed_damage);
                break;
            default:
                break;
        }
    }


    private void ApplyResistance()
    {
        if (computed_damage <= 0 || resistances == null) return;

        computed_damage -= resistances[weapon.GetType()] * computed_damage;
    }
}
