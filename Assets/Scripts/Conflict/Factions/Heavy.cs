using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heavy : MonoBehaviour {

    public float speed = 3.5f;
    Actor actor;
    Formation formation;

    // Unity

    private void Awake()
    {
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


    public void Restrategize()
    {
        if (actor.attack != null)
        {

        }
        else
        {

        }
    }


    public void SetFormation(Formation _formation)
    {
        formation = _formation;
    }


    public void Strategize()
    {
        if (formation == null)
        {
            // If we are not part of a formation, come up with an individual strategy.

            if (actor.attack != null)
            {
            }
            else
            {
            }
        }
    }


    // private


    private void ConfigureRoleSpecificProperties()
    {
        actor = GetComponent<Actor>();
        actor.SetComponents();
        actor.SetStats();
        actor.movement.GetAgent().speed = speed;
    }
}
