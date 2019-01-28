using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Acolyte : MonoBehaviour
{
    // properties

    public Actor Me { get; set; }
    public CureWounds CureWounds { get; set; }
    public Magic Magic { get; set; }
    public SacredFlame SacredFlame { get; set; }


    // Unity


    private void Start()
    {
        SetComponents();
    }


    // public


    public void OnBadlyInjured()
    {
        if (Magic.HaveSpellSlot(Magic.Level.First)) {
            Magic.UseSpellSlot(Magic.Level.First);
            CureWounds.Cast(Me);
        }
    }


    public void OnFriendsInNeed()
    {
        Me.Actions.CloseWithEnemies();
        AttackWithSpell();
        Me.Actions.Decider.FriendsInNeed.Clear();
    }


    public void OnHostileActorsSighted()
    {
        Me.Actions.CloseWithEnemies();
        AttackWithSpell();
        Me.Actions.CallForHelp();
    }


    public void OnHostileStructuresSighted()
    {
    }


    public void OnInCombat()
    {
        Me.Actions.Movement.Agent.speed = Me.Actions.Movement.Speed;
        AttackWithSpell();
    }


    public void OnIdle()
    {
        Me.Actions.Movement.Agent.speed = Me.Actions.Movement.Speed;
        Me.Actions.SheathWeapon();
        Me.Actions.Movement.SetDestination(Me.Actions.Movement.Destinations[Movement.CommonDestination.Home]);
    }


    public void OnMedic()
    {
        Actor wounded = Me.Senses.Actors
                          .Where(friend => Me.Actions.Decider.IsFriendOrNeutral(friend) && friend.Health.BadlyInjured() == true)
                          .ToList()
                          .First();

        Me.Actions.Movement.SetDestination(wounded.transform.position);

        TreatWounded();
    }


    public void OnMovingToGoal()
    {
        Me.Actions.Movement.Agent.speed = Me.Actions.Movement.Speed;
        Me.Senses.Sight();
    }


    public void OnReachedGoal()
    {
        Me.Actions.Movement.ResetPath();
        Me.Actions.Movement.Agent.speed = Me.Actions.Movement.Speed;
        Me.Actions.Decider.FriendsInNeed.Clear();
    }


    public void OnUnderAttack()
    {
        Me.Actions.Movement.Agent.speed = Me.Actions.Movement.Speed;
        AttackWithSpell();
    }


    public void OnWatch()
    {
        // call for help after running away
    }


    // private


    private void AttackWithSpell()
    {
        Actor target = Me.Actions.Decider.Threat.PrimaryThreat();

        if (Vector3.Distance(target.transform.position, transform.position) < SacredFlame.Range) {
            SacredFlame.Cast(target);
        }
    }


    private void SetComponents()
    {
        Me = GetComponent<Actor>();
        StartCoroutine(Me.GetStatsFromServer(this.GetType().Name));
        SetAdditionalStats();

        Me.Actions.Attack.EquipMeleeWeapon();
        Me.Actions.Attack.EquipRangedWeapon();

        Me.Actions.OnBadlyInjured = OnBadlyInjured;
        Me.Actions.OnFriendsInNeed = OnFriendsInNeed;
        Me.Actions.OnHostileActorsSighted = OnHostileActorsSighted;
        Me.Actions.OnHostileStructuresSighted = OnHostileStructuresSighted;
        Me.Actions.OnIdle = OnIdle;
        Me.Actions.OnInCombat = OnInCombat;
        Me.Actions.OnMedic = OnMedic;
        Me.Actions.OnMovingToGoal = OnMovingToGoal;
        Me.Actions.OnReachedGoal = OnReachedGoal;
        Me.Actions.OnUnderAttack = OnUnderAttack;
        Me.Actions.OnWatch = OnWatch;
        Me.Actions.Movement.AddDestination(Movement.CommonDestination.Home, transform.position);
    }


    private void SetAdditionalStats()
    {
        Me.Actions.Attack.AvailableWeapons = Characters.available_weapons[Characters.Template.Commoner];
        Me.Senses.Darkvision = Characters.darkvision_range[Characters.Template.Base];
        Me.Senses.PerceptionRange = Characters.perception_range[Characters.Template.Base];
        Me.Stats.Resistances = Characters.resistances[Characters.Template.Base];

        CureWounds = gameObject.AddComponent<CureWounds>();
        SacredFlame = gameObject.AddComponent<SacredFlame>();
        Magic = gameObject.AddComponent<Magic>();
        Magic.SpellSlots[Magic.Level.First] = 3;
        Magic.SpellsLeft[Magic.Level.First] = 3;
    }


    private void TreatWounded()
    {
        var nearby_wounded = Me.Senses.Actors
                               .Where(f => Me.Actions.Decider.IsFriendOrNeutral(f) && f.Health.BadlyInjured() && Vector3.Distance(transform.position, f.transform.position) < 2f + Me.Size)
                               .ToList();

        if (nearby_wounded.Count > 0) {
            Actor wounded = nearby_wounded.OrderBy(f => f.Health.CurrentHealthPercentage()).Reverse().ToList().First();

            if (wounded != null) {
                Magic.UseSpellSlot(Magic.Level.First);
                CureWounds.Cast(wounded);
            }
        }
    }
}