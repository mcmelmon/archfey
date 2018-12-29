using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defend : MonoBehaviour
{

    public Dictionary<Weapon.DamageType, int> resistances;

    // properties

    public Actor Actor { get; set; }
    public int ArmorRating { get; set; }
    public float ComputedDamage { get; set; }
    public int ConstitutionRating { get; set; }
    public Weapon.DamageType DamageType { get; set; }
    public int DefenseRating { get; set; }
    public int ForceRating { get; set; }
    public Stats Stats { get; set; }



    // Unity


    private void Awake()
    {
        SetComponents();
    }


    private void OnValidate()
    {
        if (ArmorRating > 10) ArmorRating = 10;
        if (ArmorRating < 0) ArmorRating = 0;

        if (ForceRating > 10) ForceRating = 10;
        if (ForceRating < 0) ForceRating = 0;
    }


    // public


    public float DamageAfterDefenses(float _damage, Weapon.DamageType _type)
    {
        // Apply our defense characteristics to an attack and compute damage

        ComputedDamage = _damage;
        DamageType = _type;

        ApplyAgility();
        ApplyArmor();
        ApplyConstitution();
        ApplyForce();
        ApplyIntellect();
        ApplyWill();
        ApplyResistance();

        return ComputedDamage;
    }


    public void SetResistances(Dictionary<Weapon.DamageType, int> _resistances)
    {
        resistances = _resistances;
    }


    // private


    private void ApplyArmor()
    {
        // Armor rating deflects some damage

        if (ComputedDamage <= 0) return;

        switch (DamageType)
        {
            case Weapon.DamageType.Blunt:
                ComputedDamage -= ArmorRating * 1.2f;
                break;
            case Weapon.DamageType.Piercing:
                ComputedDamage -= ArmorRating;
                break;
            case Weapon.DamageType.Slashing:
                ComputedDamage -= ArmorRating * 2f;
                break;
            case Weapon.DamageType.Elemental:
                ComputedDamage -= ArmorRating * 0.5f;
                break;
            default:
                break;
        }
    }


    private void ApplyAgility()
    {
        // Agility rating avoids some damage by shifting location struck

        if (ComputedDamage <= 0) return;

        switch (DamageType) {
            case Weapon.DamageType.Blunt:
                ComputedDamage -= Stats.AgilityRating;
                break;
            case Weapon.DamageType.Piercing:
                ComputedDamage -= Stats.AgilityRating * 1.2f;
                break;
            case Weapon.DamageType.Slashing:
                ComputedDamage -= Stats.AgilityRating * 1.2f;
                break;
            case Weapon.DamageType.Elemental:
                ComputedDamage -= Stats.AgilityRating * 0.5f;
                break;
            default:
                break;
        }
    }


    private void ApplyConstitution()
    {
        // Constitution resists pain and poison

        if (ComputedDamage <= 0) return;

        switch (DamageType)
        {
            case Weapon.DamageType.Blunt:
                ComputedDamage -= ConstitutionRating * 0.5f;
                break;
            case Weapon.DamageType.Piercing:
                ComputedDamage -= ConstitutionRating * 0.5f;
                break;
            case Weapon.DamageType.Poison:
                ComputedDamage -= ConstitutionRating * 1.2f;
                break;
            case Weapon.DamageType.Slashing:
                ComputedDamage -= ConstitutionRating * 0.5f;
                break;
            default:
                break;
        }

    }


    private void ApplyForce()
    {
        // Force rating deflects some damage

        if (ComputedDamage <= 0) return;

        switch (DamageType) {
            case Weapon.DamageType.Blunt:
                ComputedDamage -= ForceRating;
                break;
            case Weapon.DamageType.Piercing:
                ComputedDamage -= ForceRating;
                break;
            case Weapon.DamageType.Slashing:
                ComputedDamage -= ForceRating;
                break;
            case Weapon.DamageType.Elemental:
                ComputedDamage -= ForceRating * 2f;
                break;
            default:
                break;
        }
    }


    private void ApplyIntellect()
    {
        // Intellect protects agains the arcane

        if (ComputedDamage <= 0) return;

        switch (DamageType)
        {
            case Weapon.DamageType.Arcane:
                ComputedDamage -= Stats.IntellectRating;
                break;
            default:
                break;
        }
    }


    private void ApplyWill()
    {
        // Will diminishes suffering

        if (ComputedDamage <= 0) return;

        ComputedDamage -= Stats.WillRating * 0.5f;
    }


    private void ApplyResistance()
    {
        if (ComputedDamage <= 0 || resistances == null) return;

        ComputedDamage -= ComputedDamage * (resistances[DamageType] / 100);
    }


    private void SetComponents()
    {
        Actor = GetComponentInParent<Actor>();
        Stats = GetComponentInParent<Stats>();

        DefenseRating = Stats.AgilityRating + ArmorRating + ConstitutionRating + ForceRating;
    }
}
