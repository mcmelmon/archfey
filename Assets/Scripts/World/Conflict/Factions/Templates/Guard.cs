using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

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


    public void OnBadlyInjured()
    {

    }


    public void OnFriendsInNeed()
    {
        List<Actor> friends_in_need = Me.Actions.Decider.FriendsInNeed;

        if (friends_in_need.Count > 0) {
            Me.Actions.Movement.Agent.speed = 2 * Me.Actions.Movement.Speed;
            Me.Actions.Movement.SetDestination(friends_in_need[Random.Range(0, friends_in_need.Count)].gameObject);
            Me.Actions.Decider.FriendsInNeed.Clear();
        }
    }


    public void OnDamagedFriendlyStructuresSighted()
    {

    }


    public void OnInCombat()
    {
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack.AttackEnemiesInRange();
    }


    public void OnHostileActorsSighted()
    {
        Me.Actions.CallForHelp();
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack.AttackEnemiesInRange();
    }


    public void OnHostileStructuresSighted()
    {

    }


    public void OnIdle()
    {
        Me.Senses.Sight();
    }


    public void OnMovingToGoal()
    {
        Me.Actions.Movement.Agent.speed = Me.Actions.Movement.Speed;
        Me.Actions.SheathWeapon();
        Me.Senses.Sight();
    }


    public void OnPerformingTask()
    {

    }


    public void OnReachedGoal()
    {
        Me.Actions.Movement.ResetPath();

        Route _route = GetComponent<Route>();

        if (_route != null && !_route.Completed()) {
            Me.Actions.Movement.SetDestination(_route.SetNext());
        } else {
            OnIdle();
        }
    }


    public void OnUnderAttack()
    {
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack.AttackEnemiesInRange();
    }


    public void OnWatch()
    {
        Me.Actions.Movement.Route = null;
        Me.Actions.Movement.ResetPath();
        Me.Actions.Attack.AttackEnemiesInRange();
    }


    // private


    private void SetStats()
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
        Me.Actions.OnReachedGoal = OnReachedGoal;
        Me.Actions.OnUnderAttack = OnUnderAttack;
        Me.Actions.OnWatch = OnWatch;

        Me.Health.SetCurrentAndMaxHitPoints();
    }


    private void SetBaseStats()
    {
        Me.Actions.ActionsPerRound = Characters.actions_per_round[Characters.Template.Base];

        Me.Actions.Attack.AvailableWeapons = Characters.available_weapons[Characters.Template.Guard];

        Me.Health.HitDice = (Characters.hit_dice[Characters.Template.Guard]);
        Me.Health.HitDiceType = (Characters.hit_dice_type[Characters.Template.Base]);

        Me.Actions.Movement.Speed = Characters.speed[Characters.Template.Base];
        Me.Actions.Movement.Agent.speed = Characters.speed[Characters.Template.Base];

        Me.Senses.Darkvision = Characters.darkvision_range[Characters.Template.Base];
        Me.Senses.PerceptionRange = Characters.perception_range[Characters.Template.Guard];

        Me.Stats.ArmorClass = Characters.armor_class[Characters.Template.Guard];
        Me.Stats.CharismaProficiency = Characters.charisma_proficiency[Characters.Template.Base];
        Me.Stats.ConstitutionProficiency = Characters.constituion_proficiency[Characters.Template.Guard];
        Me.Stats.DexterityProficiency = Characters.dexterity_proficiency[Characters.Template.Guard];
        Me.Stats.IntelligenceProficiency = Characters.intelligence_proficiency[Characters.Template.Base];
        Me.Stats.StrengthProficiency = Characters.strength_proficiency[Characters.Template.Guard];
        Me.Stats.WisdomProficiency = Characters.wisdom_proficiency[Characters.Template.Base];

        Me.Stats.Resistances = Characters.resistances[Characters.Template.Base];
        Me.Stats.ProficiencyBonus = Characters.proficiency_bonus[Characters.Template.Base];
    }
}