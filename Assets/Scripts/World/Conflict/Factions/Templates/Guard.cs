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
        Me.Actions.Movement.ResetPath();
        Me.Actions.Decider.FriendsInNeed.Clear();
        FindShrine();
    }


    public void OnFriendsInNeed()
    {
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack.AttackEnemiesInRange();
        Me.Actions.Decider.FriendsInNeed.Clear();
    }


    public void OnInCombat()
    {
        Me.Actions.CallForHelp();
        Me.Actions.Decider.FriendsInNeed.Clear();
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack.AttackEnemiesInRange();
    }


    public void OnHostileActorsSighted()
    {
        Me.Actions.Decider.FriendsInNeed.Clear();
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack.AttackEnemiesInRange();
    }


    public void OnHostileStructuresSighted()
    {

    }


    public void OnIdle()
    {
        Me.Senses.Sight();
        Me.Actions.SheathWeapon();
        Me.Actions.Movement.Home();
    }


    public void OnMovingToGoal()
    {
        Me.Actions.Movement.Agent.speed = Me.Actions.Movement.Speed;
        Me.Actions.SheathWeapon();
        Me.Senses.Sight();
    }


    public void OnNeedsRest()
    {
        Me.Actions.Movement.Agent.speed = Me.Actions.Movement.Speed;
        Me.Actions.SheathWeapon();
        Me.Actions.Movement.SetDestination(Me.Actions.Movement.Destinations[Movement.CommonDestination.Home]);
    }


    public void OnReachedGoal()
    {
        Me.Actions.Movement.ResetPath();
        Me.Actions.Decider.FriendsInNeed.Clear();

        Route _route = GetComponent<Route>();

        if (_route != null && !_route.Completed()) {
            Me.Actions.Movement.SetDestination(_route.SetNext());
        } else {
            OnIdle();
        }
    }


    public void OnUnderAttack()
    {
        Me.Actions.Decider.FriendsInNeed.Clear();
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack.AttackEnemiesInRange();
        Me.RestCounter = 0;
    }


    public void OnWatch()
    {
        Me.Actions.Movement.Route = null;
        Me.Actions.Movement.ResetPath();
        Me.Actions.Attack.AttackEnemiesInRange();
    }


    // private


    private void FindShrine()
    {
        Structure nearest_sacred_structure = new List<Structure>(FindObjectsOfType<Structure>())
            .Where(s => s.owner == Me.Faction && s.purpose == Structure.Purpose.Sacred)
            .OrderBy(s => Vector3.Distance(transform.position, s.transform.position))
            .ToList()
            .First();

        Me.Actions.Movement.SetDestination(nearest_sacred_structure.transform);
    }


    private void SetStats()
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
        Me.Actions.OnMovingToGoal = OnMovingToGoal;
        Me.Actions.OnNeedsRest = OnNeedsRest;
        Me.Actions.OnReachedGoal = OnReachedGoal;
        Me.Actions.OnUnderAttack = OnUnderAttack;
        Me.Actions.OnWatch = OnWatch;
        Me.Actions.Movement.AddDestination(Movement.CommonDestination.Home, transform.position);
    }


    private void SetAdditionalStats()
    {
        Me.Actions.Attack.AvailableWeapons = Characters.available_weapons[Characters.Template.Guard];
        Me.Senses.Darkvision = Characters.darkvision_range[Characters.Template.Base];
        Me.Senses.PerceptionRange = Characters.perception_range[Characters.Template.Guard];
        Me.Stats.Resistances = Characters.resistances[Characters.Template.Base];
    }
}