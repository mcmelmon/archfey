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
    public Actor Target { get; set; }
    public Weapon Weapon { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    public void Strike(Actor _target)
    {
        if (_target == null) return;
        Target = _target;
        Weapon = Attack.EquippedMeleeWeapon;
        Weapon.gameObject.SetActive(true);
        SetModifiers();

        if (Random.Range(1, 21) + AttackModifier > _target.Defend.ArmorClass) { // Dexterity is already built in to AC
            ApplyDamage();
            DisplayEffects();
            AdjustEnergy();
        }
    }


    // private


    private void AdjustEnergy()
    {
        Resources.IncreaseEnergy(Damage * Resources.energy_potency);
        Target.Resources.IncreaseEnergy(Damage * Target.Resources.energy_potency);
    }


    private void ApplyDamage()
    {
        if (Target.Health != null && Target.Defend != null && Actor != null)
        {
            Damage = Target.Defend.DamageAfterDefenses(Weapon.expected_damage + DamageModifier, Weapon.damage_type);
            Target.Health.LoseHealth(Damage, Actor);
        }
    }


    public void DisplayEffects()
    {
        GameObject _impact = Instantiate(SpellEffects.Instance.physical_strike_prefab, Target.transform.position + new Vector3(1, 4f, 0), SpellEffects.Instance.physical_strike_prefab.transform.rotation);
        _impact.transform.parent = Target.transform;
        _impact.name = "Impact";
        Destroy(_impact, 1f);
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
            AttackModifier = Actor.Stats.DexterityProficiency + Weapon.attack_bonus + Actor.SuperiorWeapons[Weapon.damage_type];
            DamageModifier = Actor.Stats.DexterityProficiency + Weapon.damage_bonus + Actor.SuperiorWeapons[Weapon.damage_type];
        }
        else
        {
            AttackModifier = Actor.Stats.StrengthProficiency + Weapon.attack_bonus + Actor.SuperiorWeapons[Weapon.damage_type];
            DamageModifier = Actor.Stats.StrengthProficiency + Weapon.damage_bonus + Actor.SuperiorWeapons[Weapon.damage_type];
        }
    }
}
