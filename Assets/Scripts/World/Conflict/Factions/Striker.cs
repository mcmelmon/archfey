using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Striker : MonoBehaviour {

    public float speed = 5f;
    public float sense_radius = 30f;
    public float sense_perception = 15f;
    Actor actor;
    Movement movement;
    Senses senses;
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
        if (attacker != null) {

        }
        else {

        }
    }


    public void Strategize()
    {
        if (true)
        {
            // If we are not part of a formation, come up with an individual strategy.

            if (attacker != null) {
            }
            else {
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