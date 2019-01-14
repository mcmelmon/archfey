using System.Collections;
using System.Collections.Generic;
using System.Linq;
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


    public void OnBadlyInjured()
    {

    }


    public void OnFriendsInNeed()
    {
        Me.Actions.Decider.FriendsInNeed.Clear();
    }


    public void OnDamagedFriendlyStructuresSighted()
    {
        Me.Actions.CallForHelp();
    }


    public void OnHostileActorsSighted()
    {
        Me.Actions.FleeFromEnemies();
        Me.Actions.CallForHelp();
    }


    public void OnHostileStructuresSighted()
    {
        Me.Actions.CallForHelp();
    }


    public void OnInCombat()
    {
        Me.Actions.FleeFromEnemies();
    }


    public void OnIdle()
    {
        Me.Actions.Movement.Agent.speed = Me.Actions.Movement.Speed;
        Me.Actions.SheathWeapon();

        var structures = new List<Structure>(FindObjectsOfType<Structure>())
            .Where(structure => structure.owner == Me.Faction)
            //.OrderBy(structure => Vector3.Distance(transform.position, transform.transform.position))
            .ToList();

        Structure _structure = structures[Random.Range(0, structures.Count)];
        Vector3 entrance = _structure.entrances[Random.Range(0, _structure.entrances.Count)].transform.position;

        Me.Actions.Movement.SetDestination(entrance);
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

        Me.Actions.OnBadlyInjured = OnBadlyInjured;
        Me.Actions.OnFriendsInNeed = OnFriendsInNeed;
        Me.Actions.OnDamagedFriendlyStructuresSighted = OnDamagedFriendlyStructuresSighted;
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

        Me.Actions.Attack.AvailableWeapons = Characters.available_weapons[Characters.Template.Commoner];

        Me.Health.HitDice = Characters.hit_dice[Characters.Template.Base];
        Me.Health.HitDiceType = Characters.hit_dice_type[Characters.Template.Base];

        Me.Actions.Movement.Speed = Characters.speed[Characters.Template.Base];
        Me.Actions.Movement.Agent.speed = Characters.speed[Characters.Template.Base];

        Me.Senses.Darkvision = Characters.darkvision_range[Characters.Template.Base];
        Me.Senses.PerceptionRange = Characters.perception_range[Characters.Template.Base];

        Me.Stats.ArmorClass = Characters.armor_class[Characters.Template.Base];
        Me.Stats.CharismaProficiency = Characters.charisma_proficiency[Characters.Template.Base];
        Me.Stats.ConstitutionProficiency = Characters.constituion_proficiency[Characters.Template.Base];
        Me.Stats.DexterityProficiency = Characters.dexterity_proficiency[Characters.Template.Base];
        Me.Stats.IntelligenceProficiency = Characters.intelligence_proficiency[Characters.Template.Base];
        Me.Stats.StrengthProficiency = Characters.strength_proficiency[Characters.Template.Base];
        Me.Stats.WisdomProficiency = Characters.wisdom_proficiency[Characters.Template.Base];

        Me.Stats.Resistances = Characters.resistances[Characters.Template.Base];
        Me.Stats.ProficiencyBonus = Characters.proficiency_bonus[Characters.Template.Base];
    }
}
