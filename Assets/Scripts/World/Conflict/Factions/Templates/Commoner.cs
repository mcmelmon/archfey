﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commoner : MonoBehaviour {

    // properties

    public Actor Me { get; set; }


    // Unity


    private void Start()
    {
        SetComponents();
    }


    // public


    public void OnAlliesUnderAttack()
    {
        Me.Actions.FleeFromEnemies();
    }


    public void OnBadlyInjured()
    {

    }


    public void OnHostileActorsSighted()
    {
        Me.Actions.FleeFromEnemies();
    }


    public void OnHostileStructuresSighted()
    {
        if (Me.Actions.Decider.Structures.Count > 0) {
            Collider _collider = Me.Actions.Decider.Structures[Random.Range(0, Me.Actions.Decider.Structures.Count)].GetComponent<Collider>();
            Vector3 destination = _collider.ClosestPointOnBounds(transform.position);

            Me.Actions.Movement.SetDestination(destination);
        }
    }


    public void OnInCombat()
    {
        Me.Actions.FleeFromEnemies();
    }


    public void OnIdle()
    {
        Me.Actions.Movement.Agent.speed = Me.Actions.Movement.Speed;
        Me.Actions.SheathWeapon();

        Structure[] structures = FindObjectsOfType<Structure>();

        if (Me.Actions.Movement.Route == null) {
            Me.Actions.Movement.SetDestination(structures[Random.Range(0,structures.Length)].transform.position);
        }
    }


    public void OnMovingToGoal()
    {
        Me.Senses.Sight();
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
        Me.Actions.FleeFromEnemies();
    }


    public void OnWatch()
    {
        // call for help after running away
    }


    // private


    private void SetComponents()
    {
        // can't do in Actor until the Commoner component has been attached
        Me = GetComponent<Actor>();
        SetBaseStats();

        Me.Actions.Attack.EquipMeleeWeapon();
        Me.Actions.Attack.EquipRangedWeapon();
        Me.Actions.OnAlliesUnderAttack = OnAlliesUnderAttack;
        Me.Actions.OnBadlyInjured = OnBadlyInjured;
        Me.Actions.OnHostileActorsSighted = OnHostileActorsSighted;
        Me.Actions.OnHostileStructuresSighted = OnHostileStructuresSighted;
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
        Me.Actions.ClaimRating = Characters.claim_rating[Characters.Template.Commoner];

        Me.Actions.Attack.AvailableWeapons = Characters.available_weapons[Characters.Template.Commoner];

        Me.Actions.Stats.ArmorClass = Characters.armor_class[Characters.Template.Base];
        Me.Actions.Stats.SetResistances(Characters.resistances[Characters.Template.Base]);

        Me.Health.HitDice = Characters.hit_dice[Characters.Template.Base];
        Me.Health.HitDiceType = Characters.hit_dice_type[Characters.Template.Base];

        Me.Actions.Movement.Speed = Characters.speed[Characters.Template.Base];
        Me.Actions.Movement.Agent.speed = Characters.speed[Characters.Template.Base];

        Me.Senses.Darkvision = Characters.darkvision_range[Characters.Template.Base];
        Me.Senses.PerceptionRange = Characters.perception_range[Characters.Template.Base];

        Me.Stats.CharismaProficiency = Characters.charisma_proficiency[Characters.Template.Base];
        Me.Stats.ConstitutionProficiency = Characters.constituion_proficiency[Characters.Template.Base];
        Me.Stats.DexterityProficiency = Characters.dexterity_proficiency[Characters.Template.Base];
        Me.Stats.IntelligenceProficiency = Characters.intelligence_proficiency[Characters.Template.Base];
        Me.Stats.StrengthProficiency = Characters.strength_proficiency[Characters.Template.Base];
        Me.Stats.WisdomProficiency = Characters.wisdom_proficiency[Characters.Template.Base];
    }
}
