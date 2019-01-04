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

        if (Actor.Movement.Route == null) {
            Actor.Movement.SetRoute(Route.Linear(transform.position, next_objective.control_points[0].transform.position, Actor.Decider.FinishedRoute));
        }
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
        Actor.Attack.EquipMeleeWeapon();
        Actor.Attack.EquipRangedWeapon();
        Actor.OnIdle = OnIdle;
        Actor.OnUnderAttack = OnUnderAttack;
    }
}
