using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defend : MonoBehaviour
{

    public Dictionary<Weapon.DamageType, int> resistances;

    // properties

    public Actor Actor { get; set; }
    public int ArmorClass { get; set; }
    public float ComputedDamage { get; set; }
    public Weapon.DamageType DamageType { get; set; }
    public int DefenseRating { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    private void OnValidate()
    {
        if (ArmorClass > 30) ArmorClass = 30;
        if (ArmorClass < 1) ArmorClass = 1;

    }


    // public


    public float DamageAfterDefenses(float _damage, Weapon.DamageType _type)
    {
        // Apply our defense characteristics to an attack and compute damage

        ComputedDamage = _damage;
        DamageType = _type;
        ApplyResistance();

        return ComputedDamage;
    }


    public void SetResistances(Dictionary<Weapon.DamageType, int> _resistances)
    {
        resistances = _resistances;
    }


    // private


    private void ApplyResistance()
    {
        if (ComputedDamage <= 0 || resistances == null) return;

        ComputedDamage -= ComputedDamage * (resistances[DamageType] / 100);
    }


    private void SetComponents()
    {
        Actor = GetComponentInParent<Actor>();

        DefenseRating = ArmorClass + Actor.Stats.DexterityProficiency;
    }
}
