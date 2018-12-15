using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Scout : MonoBehaviour {

    public float speed = 6f;
    public float sense_radius = 40f;
    public float sense_perception = 20f;
    public List<Vector3> reports = new List<Vector3>();
    Movement movement;
    Senses senses;
    Actor actor;


    // Unity


    private void Start () {
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