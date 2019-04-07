using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Range : MonoBehaviour
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
        Weapon = Me.Actions.Combat.EquippedRangedWeapon;
        SetModifiers();
        Projectile = Instantiate(Weapon.projectile_prefab, Me.weapon_transform.position, transform.rotation);
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
                int damage_roll = (Critical) ? (Me.Actions.RollDie(Weapon.DiceType, Weapon.NumberOfDice) * 2) + 1 : Me.Actions.RollDie(Weapon.DiceType, Weapon.NumberOfDice);
                damage_roll += DamageModifier;
                Damage = target_actor.Actions.Stats.DamageAfterDefenses(damage_roll + DamageModifier, Weapon.DamageType);
                target_actor.Health.LoseHealth(Damage, Me);
            }
        } else if (target_structure != null) {
            int damage_roll = Me.Actions.RollDie(Weapon.DiceType, Weapon.NumberOfDice) + 1;
            damage_roll += DamageModifier; target_structure.LoseStructure(damage_roll, Weapon.DamageType);
        }
    }


    private void CheckAdvantageAndDisadvantage()
    {
        var friends_in_melee = Me.Senses.Actors
                                 .Where(friend => friend != null && Me.Actions.Decider.IsFriendOrNeutral(friend) && Vector3.Distance(transform.position, friend.transform.position) < 2f)
                                 .ToList();
                                 
        if (Me.Actions.Decider.AvailableMeleeTargets.Count > 0) {
            Disadvantage = true;
        } 

        Advantage |= friends_in_melee.Count > Me.Actions.Decider.AvailableMeleeTargets.Count;
    }


    private void DisplayEffects(Vector3 _location)
    {
        GameObject _impact = Instantiate(SpellEffects.Instance.physical_strike_prefab, _location, SpellEffects.Instance.physical_strike_prefab.transform.rotation);
        _impact.name = "Impact";
        Destroy(_impact, 3f);
    }


    private bool Hit()
    {
        Actor target_actor = Target.GetComponent<Actor>();
        Structure target_structure = Target.GetComponent<Structure>();
        Critical = false;

        int roll = Me.Actions.RollDie(20, 1, Advantage, Disadvantage);

        //Debug.Log(Me.name + " ranged attack rolled: " + roll);

        if (roll >= Me.Actions.Combat.CriticalRangeStart) Critical = true;

        if (target_actor != null) {
            return  roll + AttackModifier > target_actor.Actions.Stats.GetArmorClass();
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
        AttackModifier = Me.Stats.ProficiencyBonus + Me.Stats.GetAdjustedAttributeScore(Proficiencies.Attribute.Dexterity) + Weapon.DamageBonus;
        DamageModifier = Me.Stats.GetAdjustedAttributeScore(Proficiencies.Attribute.Dexterity) + Weapon.DamageBonus + Me.Actions.Combat.CalculateAdditionalDamage(Target, true);
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
                float distance = Projectile.GetComponent<Projectile>().speed * Time.deltaTime;

                Projectile.transform.position += distance * direction;
                separation = Vector3.Distance(Target.transform.position, Projectile.transform.position);

                if (separation <= .5f) Destroy(Projectile);
            }

            yield return null;
        }
    }
}
