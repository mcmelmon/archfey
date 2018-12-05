using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Striker : MonoBehaviour {

    Geography geography;
    Actor actor;

    // Unity

    private void Awake()
    {
        geography = GetComponentInParent<World>().GetComponentInChildren<Geography>();
        actor = GetComponent<Actor>();
    }


    private void Start()
    {
        ConfigureRoleSpecificProperties();
        Strategize();
    }


    private void Update()
    {

    }


    // public


    private void Restrategize()
    {
        Route previous_route = actor.movement.GetRoute();
        Route new_route;

        if (actor.attack != null && previous_route != null)
        {

        }
        else
        {

        }

        //actor.movement.SetRoute(new_route);
    }


    private void Strategize()
    {
        // TODO: differentiate between Mhoddim and Ghaddim approaches

        Route _route;

        if (actor.attack != null)
        {

        }
        else
        {
        }

        //actor.movement.SetRoute(_route);
    }


    // private


    private void ConfigureRoleSpecificProperties()
    {
        actor = GetComponent<Actor>();
        actor.SetComponents();
        actor.SetStats();
    }


    private void SetFormation()
    {

    }
}
