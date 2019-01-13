﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smite : MonoBehaviour
{
    // properties

    public Actor Actor { get; set; }
    public float Damage { get; set; }
    public int ManaCost { get; set; }
    public float Range { get; set; }
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

        ApplyDamage();
        DisplayEffects();
    }


    // private


    private void ApplyDamage()
    {
        Damage = 3 * (Actor.Actions.Attack.EquippedMeleeWeapon.damage_die + Actor.Actions.Attack.AttackRating);
        int damage_inflicted = Target.Actions.Stats.DamageAfterDefenses(Mathf.RoundToInt(Damage), Weapon.DamageType.Radiant);
        Target.Health.LoseHealth(damage_inflicted, Actor);
    }


    private void DisplayEffects()
    {
        GameObject flare = Instantiate(SpellEffects.Instance.smite_prefab, Target.transform.position, Target.transform.rotation, Target.transform);
        flare.name = "Smite";
        flare.transform.position += new Vector3(0, 3, 0);
        Destroy(flare, 0.5f);
    }


    private void SetComponents()
    {
        Actor = GetComponentInParent<Actor>();
        Stats = GetComponentInParent<Stats>();
        Damage = 0;
        ManaCost = 40;
        Range = 20f;
    }
}
