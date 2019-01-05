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


    public void OnIdle()
    {
        List<Objective> objectives = Objectives.HeldByFaction[Actor.Faction];
        Objective next_objective = objectives[Random.Range(0, objectives.Count)];

        if (Actor.Actions.Movement.Route == null) {
            Actor.Actions.Movement.SetRoute(Route.Linear(transform.position, next_objective.control_points[0].transform.position, Actor.Actions.Decider.FinishedRoute));
        }
    }


    public void OnHostilesSighted()
    {

    }


    public void OnUnderAttack()
    {
        Debug.Log("Help!");
    }


    // private


    private void SetStats()
    {
        // can't do in Actor until the Commoner component has been attached
        Actor = GetComponent<Actor>();
        Actor.Mhoddim.SetStats();
        Actor.Actions.Attack.EquipMeleeWeapon();
        Actor.Actions.Attack.EquipRangedWeapon();
        Actor.Actions.OnIdle = OnIdle;
        Actor.Actions.OnUnderAttack = OnUnderAttack;
    }
}
