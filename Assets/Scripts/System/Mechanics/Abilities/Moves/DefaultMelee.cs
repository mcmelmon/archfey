using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultMelee : MonoBehaviour
{
    // Inspector settings

    // properties

    public Actor Actor { get; set; }
    public Attack Attack { get; set; }
    public int AttackModifier { get; set; }
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
            Weapon.Impact();
            ApplyDamage();
        }
    }


    // private


    private void ApplyDamage()
    {
        if (Target.Health != null && Target.Defend != null && Actor != null)
        {
            float damage_inflicted = Target.Defend.DamageAfterDefenses(Random.Range(1, Weapon.damage_maximum) + DamageModifier, Weapon.damage_type);
            Target.Health.LoseHealth(damage_inflicted, Actor);
        }
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
