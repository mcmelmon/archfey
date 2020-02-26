using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EldritchBlast : MonoBehaviour
{
   // properties

    public Actor Me { get; set; }
    public bool Advantage { get; set; }
    public Weapons.DamageType DamageType { get; set; }
    public int Die { get; set; }
    public bool Disadvantage { get; set; }
    public Magic.Level Level { get; set; }
    public float Range { get; set; }
    public Magic.School School { get; set; }
    public Spellcaster Spellcaster { get; set; }
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
        CheckAdvantageAndDisadvantage();

        DrawRay();

        if (Hit()) {
            ApplyDamage();
            DisplayEffects();
        }
        // TODO: push or pull from invocations
    }

    public bool IsWithinAttackRange(Transform target)
    {
        float separation = Me.SeparationFrom(target);
        return separation < Range;
    }

    // private

    private void ApplyDamage()
    {
        int damage_roll = Me.Actions.RollDie(Die, NumberOfDice());

        // TODO: agonizing blast invocation 

        int damage_inflicted = Target.Actions.Stats.DamageAfterDefenses(damage_roll, DamageType);
        Target.Health.LoseHealth(damage_inflicted, Me);
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

    private void DisplayEffects()
    {
        GameObject flare = Instantiate(SpellEffects.Instance.sacred_flame_prefab, Target.transform.position, Target.transform.rotation, Target.transform);
        flare.name = "EldritchBlast";
        flare.transform.position += new Vector3(0, 3, 0);
        Destroy(flare, 1.5f);
    }

    private void DrawRay()
    {
        Me.Actions.Magic.DrawRay(Me, Target);
    }

    private bool Hit()
    {
        Actor target_actor = Target.GetComponent<Actor>();

        int roll = Me.Actions.RollDie(20, 1, Advantage, Disadvantage);

        if (target_actor != null) {
            return  roll + Spellcaster.AttackModifier() > target_actor.Actions.Stats.GetArmorClass();
        }

        return false;
    }

    private int NumberOfDice()
    {
        switch (Me.GetComponent<Spellcaster>().Level) {  // TODO: pick the Spellcaster for this spell by casting attribute
            case int n when (n > 0 && n < 5):
                return 1;
            case int n when (n >= 5 && n < 11):
                return 2;
            case int n when (n >= 11 && n < 17):
                return 3;
            case int n when (n >= 17):
                return 4;
            default:
                return 1;
        }
    }

    private void SetComponents()
    {
        DamageType = Weapons.DamageType.Force;
        Die = 10;
        Level = Magic.Level.Cantrip;
        Me = GetComponentInParent<Actor>();
        Range = 12f;
        School = Magic.School.Evocation;
        Spellcaster = Me.GetComponent<Spellcaster>(); // TODO: someday we may have mutliclassing, but not this day
    }
}
