using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultMelee : MonoBehaviour
{
    // properties

    public Actor Actor { get; set; }
    public Attack Attack { get; set; }
    public int AttackModifier { get; set; }
    public float Damage { get; set; }
    public int DamageModifier { get; set; }
    public Resources Resources { get; set; }
    public Actor TargetActor { get; set; }
    public Objective TargetStructure { get; set; }
    public Weapon Weapon { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    public void Strike(GameObject _target)
    {
        if (_target == null) return;

        TargetActor = _target.GetComponent<Actor>() ?? null;
        TargetStructure = _target.GetComponentInParent<Objective>() ?? null;
        Weapon = Attack.EquippedMeleeWeapon;
        Weapon.gameObject.SetActive(true);
        SetModifiers();

        if (Hit()) {
            ApplyDamage();
            DisplayEffects(_target.transform.position);
            AdjustEnergy();
        }
    }


    // private


    private void AdjustEnergy()
    {
        Resources.IncreaseEnergy(Damage * Resources.energy_potency);
        if (TargetActor != null) TargetActor.Actions.Resources.IncreaseEnergy(Damage * TargetActor.Actions.Resources.energy_potency);
    }


    private void ApplyDamage()
    {
        if (TargetActor != null) {
            if (TargetActor.Health != null && TargetActor.Actions.Defend != null && Actor.Actions != null) {
                int damage_roll = Random.Range(0, Weapon.damage_die) + 1;
                Damage = TargetActor.Actions.Defend.DamageAfterDefenses(damage_roll + DamageModifier, Weapon.damage_type);
                TargetActor.Health.LoseHealth(Damage, Actor);
            }
        } else if (TargetStructure != null && TargetStructure.is_structure) {
            int damage_roll = Random.Range(0, Weapon.damage_die) + 1;
            TargetStructure.LoseStructure(damage_roll);
        }
    }


    public void DisplayEffects(Vector3 _location)
    {
        GameObject _impact = Instantiate(SpellEffects.Instance.physical_strike_prefab, _location + new Vector3(1, 4f, 0), SpellEffects.Instance.physical_strike_prefab.transform.rotation);
        _impact.transform.parent = transform;
        _impact.name = "Impact";
        Destroy(_impact, 1f);
    }


    public bool Hit()
    {
        if (TargetActor != null) {
            return Random.Range(1, 21) + AttackModifier > TargetActor.Actions.Defend.ArmorClass;
        } else if (TargetStructure != null && TargetStructure.is_structure) {
            return Random.Range(1, 21) + AttackModifier > TargetStructure.armor_class;
        }

        return false;
    }


    private void SetComponents()
    {
        Actor = GetComponentInParent<Actor>();
        Attack = GetComponent<Attack>();
        Resources = GetComponent<Resources>();
    }


    private void SetModifiers()
    {
        if (Weapon.is_light)
        {
            AttackModifier = Actor.Stats.DexterityProficiency + Weapon.attack_bonus + Actions.SuperiorWeapons[Weapon.damage_type];
            DamageModifier = Actor.Stats.DexterityProficiency + Weapon.damage_bonus + Actions.SuperiorWeapons[Weapon.damage_type];
        }
        else
        {
            AttackModifier = Actor.Stats.StrengthProficiency + Weapon.attack_bonus + Actions.SuperiorWeapons[Weapon.damage_type];
            DamageModifier = Actor.Stats.StrengthProficiency + Weapon.damage_bonus + Actions.SuperiorWeapons[Weapon.damage_type];
        }
    }
}
