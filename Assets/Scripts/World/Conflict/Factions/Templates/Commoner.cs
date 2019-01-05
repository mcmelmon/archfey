using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commoner : MonoBehaviour {

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
        Actor.Actions.FleeFromEnemies();
    }


    public void OnBadlyInjured()
    {

    }


    public void OnHasObective()
    {
        Actor.Actions.Movement.Advance();
    }


    public void OnInCombat()
    {
        Actor.Actions.FleeFromEnemies();
    }


    public void OnHasObjective()
    {
        Actor.Actions.Movement.Advance();
    }


    public void OnHostilesSighted()
    {
        Actor.Actions.FleeFromEnemies();
    }


    public void OnIdle()
    {
        Actor.Actions.Movement.Agent.speed = Actor.Actions.Movement.Speed;
        Actor.Actions.SheathWeapon();

        List<Objective> objectives = Objectives.HeldByFaction[Actor.Faction];
        Objective next_objective = objectives[Random.Range(0, objectives.Count)];

        if (Actor.Actions.Movement.Route == null) {
            Actor.Actions.Movement.SetRoute(Route.Linear(transform.position, next_objective.control_points[0].transform.position, Actor.Actions.Decider.FinishedRoute));
        }
    }


    public void OnUnderAttack()
    {
        Actor.Actions.FleeFromEnemies();
    }


    public void OnWatch()
    {
        // call for help after running away
    }


    // private


    private void SetStats()
    {
        // can't do in Actor until the Commoner component has been attached
        Actor = GetComponent<Actor>();
        Actor.Mhoddim.SetStats();
        Actor.Actions.Attack.EquipMeleeWeapon();
        Actor.Actions.Attack.EquipRangedWeapon();
        Actor.Actions.OnAlliesUnderAttack = OnAlliesUnderAttack;
        Actor.Actions.OnBadlyInjured = OnBadlyInjured;
        Actor.Actions.OnHasObjective = OnHasObjective;
        Actor.Actions.OnHostilesSighted = OnHostilesSighted;
        Actor.Actions.OnIdle = OnIdle;
        Actor.Actions.OnInCombat = OnInCombat;
        Actor.Actions.OnUnderAttack = OnUnderAttack;
        Actor.Actions.OnWatch = OnWatch;
    }
}
