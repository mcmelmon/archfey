using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DefaultMelee : MonoBehaviour
{
    // properties

    public Actor Me { get; set; }
    public bool Advantage { get; set; }
    public int AttackModifier { get; set; }
    public bool Critical { get; set; }
    public float Damage { get; set; }
    public int DamageModifier { get; set; }
    public bool Disadvantage { get; set; }
    public GameObject Target { get; set; }
    public Weapon Weapon { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    public void Strike(GameObject _target)
    {
        if (_target == null) return;

        Target = _target;
        Weapon = Me.Actions.Attack.EquippedMeleeWeapon;
        SetModifiers();

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
                Damage = target_actor.Actions.Stats.DamageAfterDefenses(damage_roll, Weapon.DamageType);
                target_actor.Health.LoseHealth(Damage, Me);
            }
        } else if (target_structure != null) {
            int damage_roll = Me.Actions.RollDie(Weapon.DiceType, Weapon.NumberOfDice) + 1;
            damage_roll += DamageModifier;
            target_structure.LoseStructure(damage_roll, Weapon.DamageType);
        }

        Critical = false;
    }


    private void CheckAdvantageAndDisadvantage()
    {
        var friends_in_melee = Me.Senses.Actors
                                 .Where(friend => friend != null && Me.Actions.Decider.IsFriendOrNeutral(friend) && Vector3.Distance(transform.position, friend.transform.position) < 2f)
                                 .ToList();

        Advantage |= friends_in_melee.Count > Me.Actions.Attack.AvailableMeleeTargets.Count;

        // TODO: calculate disadvantage (e.g. can't see, restrained, etc)
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

        int roll = Me.Actions.RollDie(20, 1, Advantage, Disadvantage);

        if (roll >= Me.Actions.Attack.CriticalRangeStart) Critical = true;

        Debug.Log(Me.name + " rolled: " + roll);

        if (target_actor != null) {
            return roll + AttackModifier > target_actor.Actions.Stats.GetArmorClass();
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
        if (Weapon.IsFinesse) {
            AttackModifier = Me.Stats.ProficiencyBonus + Me.Stats.GetAdjustedAttributeScore(Proficiencies.Attribute.Dexterity) + Weapon.DamageBonus;
            DamageModifier = Me.Stats.GetAdjustedAttributeScore(Proficiencies.Attribute.Dexterity) + Weapon.DamageBonus + Me.Actions.Attack.CalculateAdditionalDamage(false);
        } else {
            AttackModifier = Me.Stats.ProficiencyBonus + Me.Stats.GetAdjustedAttributeScore(Proficiencies.Attribute.Strength) + Weapon.DamageBonus;
            DamageModifier = Me.Stats.GetAdjustedAttributeScore(Proficiencies.Attribute.Strength) + Weapon.DamageBonus + Me.Actions.Attack.CalculateAdditionalDamage(false);
        }
    }
}
