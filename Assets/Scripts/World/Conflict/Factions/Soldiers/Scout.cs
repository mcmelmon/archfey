using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Scout : MonoBehaviour {

    public float sense_perception = 20f;
    public float sense_radius = 40f;
    public float speed = 15f;

    Actor actor;
    Geography geography;
    Movement movement;
    Senses senses;


    // Unity


    private void Awake () {
        SetComponents();
        SetStats();
        Strategize();
    }


    // public


    public void Restrategize()
    {
        Debug.Log("Finished one circuit");
    }
  

    public void Strategize()
    {
        // move around the map in a circle with a radius equal to my distance from the map center

        float distance_to_center = Vector3.Distance(geography.GetCenter(), transform.position);
        Circle scouting_path = Circle.CreateCircle(geography.GetCenter(), distance_to_center);
        Vector3 nearest_vertex = scouting_path.VertexClosestTo(transform.position);

        movement.SetRoute(Route.Circular(nearest_vertex, scouting_path, false, false, Restrategize));
    }


    // private


    private void SetComponents()
    {
        actor = GetComponent<Actor>();
        actor.SetComponents();
        geography = GetComponentInParent<World>().GetComponentInChildren<Geography>();
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