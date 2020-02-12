using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Melee : MonoBehaviour
{
    // properties

    public Actor Me { get; set; }
    public bool Advantage { get; set; }
    public int AttackModifier { get; set; }
    public bool Critical { get; set; }
    public float Damage { get; set; }
    public int DamageModifierMain { get; set; }
    public int DamageModifierOff { get; set; }
    public bool Disadvantage { get; set; }
    public bool IsOffhandAttack { get; set; }
    public Weapon MainHand { get; set; }
    public Weapon OffHand { get; set; }
    public GameObject Target { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    public void Strike(GameObject _target, bool offhand = false)
    {
        if (_target == null) return;

        Target = _target;
        MainHand = Me.Actions.Combat.EquippedMeleeWeapon;
        OffHand = Me.Actions.Combat.EquippedOffhand;
        IsOffhandAttack = offhand;
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
                int damage_roll = 0;

                if (OffHand != null && IsOffhandAttack) {
                    damage_roll = (Critical) ? (Me.Actions.RollDie(OffHand.DiceType, OffHand.NumberOfDice) * 2) + 1 : Me.Actions.RollDie(OffHand.DiceType, OffHand.NumberOfDice);
                    damage_roll += DamageModifierOff;
                    Damage = target_actor.Actions.Stats.DamageAfterDefenses(damage_roll, OffHand.DamageType);
                }
                else {
                    damage_roll = (Critical) ? (Me.Actions.RollDie(MainHand.DiceType, MainHand.NumberOfDice) * 2) + 1 : Me.Actions.RollDie(MainHand.DiceType, MainHand.NumberOfDice);
                    damage_roll += DamageModifierMain;
                    Damage = target_actor.Actions.Stats.DamageAfterDefenses(damage_roll, MainHand.DamageType);
                }

                target_actor.Health.LoseHealth(Damage, Me);
            }
        } else if (target_structure != null) {
            int damage_roll = Me.Actions.RollDie(MainHand.DiceType, MainHand.NumberOfDice) + 1;
            damage_roll += DamageModifierMain;
            target_structure.LoseStructure(damage_roll, MainHand.DamageType);
        }
    }


    private void CheckAdvantageAndDisadvantage()
    {
        var friends_in_melee = Me.Senses.Actors
                                 .Where(friend => friend != null && Me.Actions.Decider.IsFriendOrNeutral(friend) && Vector3.Distance(transform.position, friend.transform.position) < 2f)
                                 .ToList();

        Advantage |= friends_in_melee.Count > Me.Actions.Decider.AvailableMeleeTargets.Count;

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
        if (IsOffhandAttack && OffHand == null) return false;

        Actor target_actor = Target.GetComponent<Actor>();
        Structure target_structure = Target.GetComponent<Structure>();
        Critical = false;

        int roll = Me.Actions.RollDie(20, 1, Advantage, Disadvantage);

        if (roll >= Me.Actions.Combat.CriticalRangeStart) Critical = true;

        //Debug.Log(Me.name + " melee attack rolled: " + roll);

        if (target_actor != null) {
            return roll + AttackModifier > target_actor.Actions.Stats.GetArmorClass();
        } else if (target_structure != null) {
            return roll + AttackModifier > target_structure.ArmorClass;
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
        if (MainHand.IsFinesse) {
            AttackModifier = Me.Stats.ProficiencyBonus + Me.Stats.GetAdjustedAttributeScore(Proficiencies.Attribute.Dexterity) + MainHand.DamageBonus;
            DamageModifierMain = Me.Stats.GetAdjustedAttributeScore(Proficiencies.Attribute.Dexterity) + MainHand.DamageBonus + Me.Actions.Combat.CalculateAdditionalDamage(Target, false);
        }
        else {
            AttackModifier = Me.Stats.ProficiencyBonus + Me.Stats.GetAdjustedAttributeScore(Proficiencies.Attribute.Strength) + MainHand.DamageBonus;
            DamageModifierMain = Me.Stats.GetAdjustedAttributeScore(Proficiencies.Attribute.Strength) + MainHand.DamageBonus + Me.Actions.Combat.CalculateAdditionalDamage(Target, false);
        }
        DamageModifierOff = OffHand != null ? OffHand.DamageBonus : 0;
    }
}
