using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heavy : MonoBehaviour {

    public float speed = 3.5f;
    public float sense_radius = 20f;
    public float sense_perception = 10f;
    Actor actor;
    Movement movement;
    Senses senses;
    Formation formation;
    Attacker attacker;
    Defender defender;

    // Unity

    private void Awake()
    {
        actor = GetComponent<Actor>();
        attacker = GetComponent<Attacker>();
        defender = GetComponent<Defender>();
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
        if (true)
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

            if (true)
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
        senses = GetComponent<Senses>();
        senses.SetRange(sense_radius);
        senses.SetPerception(sense_perception);
        actor = GetComponent<Actor>();
        actor.SetComponents();
        actor.SetStats();
        movement = GetComponent<Movement>();
        movement.GetAgent().speed = speed;
    }
}
