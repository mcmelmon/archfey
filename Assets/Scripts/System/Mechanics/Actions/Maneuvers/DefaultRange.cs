﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DefaultRange : MonoBehaviour
{
    // properties

    public Actor Me { get; set; }
    public bool Advantage { get; set; }
    public int AttackModifier { get; set; }
    public bool Critical { get; set; }
    public float Damage { get; set; }
    public int DamageModifier { get; set; }
    public bool Disadvantage { get; set; }
    public GameObject Projectile { get; set; }
    public GameObject Target { get; set; }
    public Weapon Weapon { get; set; }


    // Unity

    private void Awake()
    {
        SetComponents();
    }


    // public


    public void Strike(GameObject _target) {
        // TODO: allow ranged attacks against structure

        if (_target == null) return;

        Target = _target;
        Weapon = Me.Actions.Attack.EquippedRangedWeapon;
        Weapon.gameObject.SetActive(true);
        SetModifiers();
        Projectile = Instantiate(Weapon.projectile_prefab, transform.Find("AttackOrigin").transform.position, transform.rotation);
        StartCoroutine(Seek());
        CheckAdvantageAndDisadvantage();

        if (Hit()) {
            ApplyDamage();
            DisplayEffects(_target.transform.position);
        }
    }


    // private


    private void ApplyDamage()
    {
        Actor target_actor = Target.GetComponent<Actor>();
        Structure target_structure = Target.GetComponent<Structure>();

        if (target_actor != null) {
            if (target_actor.Health != null && target_actor.Actions.Stats != null && Me.Actions != null) {
                int damage_roll = (Critical) ? Random.Range(0, Weapon.damage_die * 2) + 1 : Random.Range(0, Weapon.damage_die) + 1;
                Damage = target_actor.Actions.Stats.DamageAfterDefenses(damage_roll + DamageModifier, Weapon.damage_type);
                target_actor.Health.LoseHealth(Damage, Me);
            }
        } else if (target_structure != null) {
            int damage_roll = (Critical) ? Random.Range(0, Weapon.damage_die * 2) + 1 : Random.Range(0, Weapon.damage_die) + 1;
            target_structure.LoseStructure(damage_roll, Weapon.damage_type);
        }
    }


    private void CheckAdvantageAndDisadvantage()
    {
        var friends_in_melee = Me.Actions.Decider.Friends.Where(f => Vector3.Distance(transform.position, f.transform.position) < 2f).ToList();

        if (Me.Actions.Attack.AvailableMeleeTargets.Count > 0) {
            Disadvantage = true;
        } 

        Advantage |= friends_in_melee.Count > Me.Actions.Attack.AvailableMeleeTargets.Count;
    }


    private void DisplayEffects(Vector3 _location)
    {
        GameObject _impact = Instantiate(SpellEffects.Instance.physical_strike_prefab, _location + new Vector3(1, 4f, 0), SpellEffects.Instance.physical_strike_prefab.transform.rotation);
        _impact.transform.parent = transform;
        _impact.name = "Impact";
        Destroy(_impact, 1f);
    }


    private bool Hit()
    {
        Actor target_actor = Target.GetComponent<Actor>();
        Structure target_structure = Target.GetComponent<Structure>();

        int roll = Advantage && !Disadvantage
            ? Mathf.Max(Random.Range(1, 21), Random.Range(1, 21))
            : !Advantage && Disadvantage ? Mathf.Min(Random.Range(1, 21), Random.Range(1, 21)) : Random.Range(1, 21);

        if (roll == 20) Critical = true;

        if (target_actor != null) {
            return  roll + AttackModifier > target_actor.Actions.Stats.ArmorClass;
        } else if (target_structure != null) {
            return roll + AttackModifier > target_structure.armor_class;
        }

        return false;
    }


    private void SetComponents()
    {
        Advantage = false;
        Critical = false;
        Disadvantage = false;
        Me = GetComponentInParent<Actor>();
    }


    private void SetModifiers()
    {
        AttackModifier = Me.Stats.DexterityProficiency + Weapon.attack_bonus + Actions.SuperiorWeapons[Weapon.damage_type];
        DamageModifier = Me.Stats.DexterityProficiency + Weapon.damage_bonus + Actions.SuperiorWeapons[Weapon.damage_type];

    }


    private IEnumerator Seek()
    {
        while (true) {
            if (Target == null && Projectile != null) {
                Destroy(Projectile);
                yield return null;
            } else if (Projectile != null) {
                float separation = float.MaxValue;
                Vector3 direction = Target.transform.position - transform.position;
                float distance = 10f * Time.deltaTime;

                Projectile.transform.position += distance * direction;
                separation = Vector3.Distance(Target.transform.position, Projectile.transform.position);

                if (separation <= .5f) Destroy(Projectile);
            }

            yield return null;
        }
    }
}