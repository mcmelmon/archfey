using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{

    // properties

    public Actor Me { get; set; }


    // Unity


    private void Start()
    {
        SetStats();
    }


    // public


    public void OnAlliesUnderAttack()
    {
        Me.Actions.CloseWithEnemies();
    }


    public void OnBadlyInjured()
    {

    }


    public void OnInCombat()
    {
        Me.Actions.CloseWithEnemies();
    }


    public void OnHostilesSighted()
    {
        Me.Actions.CloseWithEnemies();
    }


    public void OnIdle()
    {
        Me.Actions.Movement.Agent.speed = Me.Actions.Movement.Speed;
        Me.Actions.SheathWeapon();

        List<Objective> objectives = Objectives.HeldByFaction[Me.Faction];
        Objective next_objective = objectives[Random.Range(0, objectives.Count)];

        if (Me.Actions.Movement.Route == null)
        {
            Me.Actions.Movement.SetDestination(next_objective.claim_nodes[0].transform.position);
        }
    }


    public void OnMovingToGoal()
    {
        Me.Actions.Movement.Advance();
    }


    public void OnPerformingTask()
    {

    }


    public void OnReachedGoal()
    {
        Me.Actions.Movement.ResetPath();
        OnIdle();
    }


    public void OnUnderAttack()
    {
        Me.Actions.CloseWithEnemies();
    }


    public void OnWatch()
    {
        Me.Actions.Movement.Route = null;
        Me.Actions.Movement.ResetPath();
    }


    // private


    private void SetStats()
    {
        // can't do in Actor until the Commoner component has been attached
        Me = GetComponent<Actor>();
        SetBaseStats();

        Me.Actions.Attack.EquipMeleeWeapon();
        Me.Actions.Attack.EquipRangedWeapon();
        Me.Actions.OnAlliesUnderAttack = OnAlliesUnderAttack;
        Me.Actions.OnBadlyInjured = OnBadlyInjured;
        Me.Actions.OnHostilesSighted = OnHostilesSighted;
        Me.Actions.OnIdle = OnIdle;
        Me.Actions.OnInCombat = OnInCombat;
        Me.Actions.OnMovingToGoal = OnMovingToGoal;
        Me.Actions.OnPerformingTask = OnPerformingTask;
        Me.Actions.OnReachedGoal = OnReachedGoal;
        Me.Actions.OnUnderAttack = OnUnderAttack;
        Me.Actions.OnWatch = OnWatch;

        Me.Health.SetCurrentAndMaxHitPoints();
    }


    private void SetBaseStats()
    {
        Me.Actions.ActionsPerRound = Characters.actions_per_round[Characters.Template.Base];
        Me.Actions.ClaimRating = Characters.claim_rating[Characters.Template.Guard];

        Me.Actions.Attack.AvailableWeapons = Characters.available_weapons[Characters.Template.Guard];

        Me.Actions.Defend.ArmorClass = Characters.armor_class[Characters.Template.Guard];
        Me.Actions.Defend.SetResistances(Characters.resistances[Characters.Template.Base]);

        Me.Health.HitDice = (Characters.hit_dice[Characters.Template.Guard]);
        Me.Health.HitDiceType = (Characters.hit_dice_type[Characters.Template.Base]);

        Me.Actions.Movement.Speed = Characters.speed[Characters.Template.Base];
        Me.Actions.Movement.Agent.speed = Characters.speed[Characters.Template.Base];

        Me.Senses.Darkvision = Characters.darkvision_range[Characters.Template.Base];
        Me.Senses.PerceptionRange = Characters.perception_range[Characters.Template.Guard];

        Me.Stats.CharismaProficiency = Characters.charisma_proficiency[Characters.Template.Base];
        Me.Stats.ConstitutionProficiency = Characters.constituion_proficiency[Characters.Template.Guard];
        Me.Stats.DexterityProficiency = Characters.dexterity_proficiency[Characters.Template.Guard];
        Me.Stats.IntelligenceProficiency = Characters.intelligence_proficiency[Characters.Template.Base];
        Me.Stats.StrengthProficiency = Characters.strength_proficiency[Characters.Template.Guard];
        Me.Stats.WisdomProficiency = Characters.wisdom_proficiency[Characters.Template.Base];
    }
}