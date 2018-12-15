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


    // Unity


    private void Start()
    {
        SetComponents();
        SetStats();
        Strategize();
    }


    // public


    public void Restrategize()
    {

    }


    public void Strategize()
    {

    }


    // private


    private void SetComponents()
    {
        actor = GetComponent<Actor>();
        actor.SetComponents();
        movement = GetComponent<Movement>();
        movement.GetAgent().speed = speed;
        senses = GetComponent<Senses>();
        senses.SetRange(sense_radius);
        senses.SetPerception(sense_perception);
    }


    private void SetStats()
    {
        if (actor.ghaddim != null) {
            actor.ghaddim.SetStats();
        } else if (actor.mhoddim != null) {
            actor.mhoddim.SetStats();
        }
    }
}