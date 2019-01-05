using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FerociousClaw : MonoBehaviour
{
    // properties

    public Actor Actor { get; set; }
    public float Damage { get; set; }
    public int EnergyCost { get; set; }
    public float Range { get; set; }
    public Resources Resources { get; set; }
    public Stats Stats { get; set; }
    public Actor Target { get; set; }


    // Unity

    private void Awake()
    {
        SetComponents();
    }

    // public


    public void Cast(Actor _target, bool dispel_effect = false)
    {
        if (_target == null) return;
        Target = _target;

        if (Resources.CurrentEnergy >= EnergyCost)
        {
            ApplyDamage();
            DisplayEffects();
            AdjustEnergy();
        }
    }


    // private


    private void AdjustEnergy()
    {
        Resources.DecreaseEnergy(EnergyCost);
        Target.Actions.Resources.IncreaseEnergy(Damage * Target.Actions.Resources.energy_potency);
    }


    public void ApplyDamage()
    {
        Damage = 2 * (Actor.Actions.Attack.EquippedMeleeWeapon.damage_die + Actor.Actions.Attack.AttackRating) * Resources.energy_potency;
        float damage_inflicted = Target.Actions.Defend.DamageAfterDefenses(Damage, Weapon.DamageType.Slashing);
        Target.Health.LoseHealth(damage_inflicted, Actor);
    }


    public void DisplayEffects()
    {
        GameObject _claw1 = Instantiate(SpellEffects.Instance.physical_strike_prefab, Target.transform.position + new Vector3(1, 4f, 0), SpellEffects.Instance.physical_strike_prefab.transform.rotation);
        _claw1.transform.parent = Target.transform;
        _claw1.name = "Claw1";
        Destroy(_claw1, 1f);

        GameObject _claw2 = Instantiate(SpellEffects.Instance.physical_strike_prefab, Target.transform.position + new Vector3(0, 4f, 1), SpellEffects.Instance.physical_strike_prefab.transform.rotation);
        _claw2.name = "Claw2";
        _claw1.transform.parent = Target.transform;
        Destroy(_claw2, 1f);
    }


    private void SetComponents()
    {
        Actor = GetComponentInParent<Actor>();
        Resources = GetComponent<Resources>();
        Stats = GetComponentInParent<Stats>();
        Damage = 0;
        EnergyCost = 33;
        Range = 7f;
    }
}