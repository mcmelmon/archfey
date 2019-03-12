using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HalfOgre : MonoBehaviour
{
    // properties

    public Actor Me { get; set; }

    // Unity


    private void Start()
    {
        SetStats();
    }


    // public


    public int AdditionalDamage(bool is_ranged)
    {
        return is_ranged ? Me.Actions.RollDie(Me.Actions.Attack.EquippedRangedWeapon.dice_type, 1) : Me.Actions.RollDie(Me.Actions.Attack.EquippedMeleeWeapon.dice_type, 1);
    }


    public void OnBadlyInjured()
    {
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack.AttackEnemiesInRange();
    }


    public void OnFriendsInNeed()
    {
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack.AttackEnemiesInRange();
    }


    public void OnHostileActorsSighted()
    {
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack.AttackEnemiesInRange();
    }


    public void OnHostileStructuresSighted()
    {
        if (Me.Actions.Decider.HostileStructures.Count > 0)
        {
            Structure target = Me.Actions.Decider.HostileStructures[Random.Range(0, Me.Actions.Decider.HostileStructures.Count)];
            Me.Actions.Movement.SetDestination(target.GetInteractionPoint(Me));
        }

        Me.Actions.Attack.AttackEnemiesInRange();
    }


    public void OnIdle()
    {
        Me.Actions.Movement.Agent.speed = Me.Actions.Movement.BaseSpeed;
        Me.Actions.SheathWeapon();

        if (Me.Route.local_stops.Length > 1)
        {
            Me.Route.MoveToNextPosition();
        }
        else
        {
            List<Objective> objectives = FindObjectsOfType<Objective>().Where(objective => objective.Claim == Conflict.Instance.EnemyFaction(Me)).ToList();
            if (objectives.Count > 0)
            {
                Objective next_objective = objectives[Random.Range(0, objectives.Count)];
                Me.Actions.Movement.SetDestination(next_objective.claim_nodes[0].transform);
            }
        }
    }


    public void OnInCombat()
    {
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack.AttackEnemiesInRange();
    }


    public void OnMovingToGoal()
    {
        Me.Actions.SheathWeapon();
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
        Me.Actions.Attack.AttackEnemiesInRange();
        Me.RestCounter = 0;
    }


    public void OnWatch()
    {
        Me.Actions.Movement.ResetPath();
        Me.Actions.Attack.AttackEnemiesInRange();
    }


    // private


    private void SetStats()
    {
        Me = GetComponent<Actor>();
        StartCoroutine(Me.GetStatsFromServer("Half Ogre"));
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
        Me.Actions.OnReachedGoal = OnReachedGoal;
        Me.Actions.OnUnderAttack = OnUnderAttack;
        Me.Actions.OnWatch = OnWatch;
        Me.Actions.Movement.AddDestination(Movement.CommonDestination.Home, transform.position);
    }


    private void SetAdditionalStats()
    {

        Me.Actions.Attack.AvailableWeapons = new List<Weapon>() { Weapons.Instance.GetWeaponNamed("greatclub"), Weapons.Instance.GetWeaponNamed("javelin") };
        Me.Stats.Resistances = Characters.resistances[Characters.Template.Base];
        Me.Actions.Attack.CalculateAdditionalDamage = AdditionalDamage;
    }
}
