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

    public void OnIdle()
    {
        List<Objective> objectives = Objectives.HeldByFaction[Conflict.Instance.EnemyFaction(Actor)];

        Actor.Movement.SetRoute(Route.Linear(transform.position, objectives[Random.Range(0, objectives.Count)].control_points[0].transform.position));
    }


    public void OnUnderAttack()
    {
        Debug.Log("Eat!");
    }


    // private


    private void SetStats()
    {
        // can't do in Actor until the Gnoll component has been attached

        Actor = GetComponent<Actor>();
        Actor.Ghaddim.SetStats();
        Actor.Attack.EquipMeleeWeapon();
        Actor.Attack.EquipRangedWeapon();
        Actor.OnIdle = OnIdle;
        Actor.OnUnderAttack = OnUnderAttack;
    }
}
