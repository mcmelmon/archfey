using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gnoll : MonoBehaviour
{
    // properties

    public Actor Actor { get; set; }

    // Unity


    private void Start()
    {
        SetStats();
    }


    // public

    public void OnAlliesUnderAttack()
    {
        Actor.Actions.CloseWithEnemies();
    }


    public void OnContestingObjective()
    {
        Actor.Actions.Movement.Route = null;
        Actor.Actions.Movement.ResetPath();
    }


    public void OnInCombat()
    {
        Actor.Actions.CloseWithEnemies();
    }


    public void OnBadlyInjured()
    {
        Actor.Actions.CloseWithEnemies();
    }


    public void OnHasObjective()
    {
        Actor.Actions.Movement.Advance();
    }


    public void OnHostilesSighted()
    {
        Actor.Actions.CloseWithEnemies();
    }


    public void OnIdle()
    {
        List<Objective> objectives = Objectives.HeldByFaction[Conflict.Instance.EnemyFaction(Actor)];

        Actor.Actions.Movement.SetRoute(Route.Linear(transform.position, objectives[Random.Range(0, objectives.Count)].control_points[0].transform.position, Actor.Actions.Decider.FinishedRoute));
    }


    public void OnUnderAttack()
    {
        Actor.Actions.CloseWithEnemies();
    }


    public void OnWatch()
    {
        Actor.Actions.CloseWithEnemies();
    }


    // private


    private void SetStats()
    {
        // can't do in Actor until the Gnoll component has been attached

        Actor = GetComponent<Actor>();
        Actor.Ghaddim.SetStats();
        Actor.Actions.Attack.EquipMeleeWeapon();
        Actor.Actions.Attack.EquipRangedWeapon();
        Actor.Actions.OnAlliesUnderAttack = OnAlliesUnderAttack;
        Actor.Actions.OnContestingObjective = OnContestingObjective;
        Actor.Actions.OnBadlyInjured = OnBadlyInjured;
        Actor.Actions.OnHasObjective = OnHasObjective;
        Actor.Actions.OnHostilesSighted = OnHostilesSighted;
        Actor.Actions.OnIdle = OnIdle;
        Actor.Actions.OnInCombat = OnInCombat;
        Actor.Actions.OnUnderAttack = OnUnderAttack;
        Actor.Actions.OnWatch = OnWatch;
    }
}
