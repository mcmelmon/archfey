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
    public Stats Stats { get; set; }
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

        Weapon.gameObject.SetActive(true);

        if (Random.Range(1, 21) + AttackModifier > Conflict.ToHitBase + Target.Defend.DefenseRating) {
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
            Damage = Target.Defend.DamageAfterDefenses(Random.Range(1, Weapon.damage_maximum) + DamageModifier, Weapon.damage_type);
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
        Resources = GetComponent<Resources>();
        Actor = GetComponentInParent<Actor>();
        Stats = GetComponentInParent<Stats>();
        Attack = GetComponent<Attack>();
        Weapon = Attack.EquippedMeleeWeapon;

        AttackModifier = Stats.AgilityRating + Stats.StrengthRating + Attack.SuperiorWeapons[Weapon.damage_type];
        DamageModifier = Stats.AgilityRating + Stats.StrengthRating + Attack.SuperiorWeapons[Weapon.damage_type];
    }
}
