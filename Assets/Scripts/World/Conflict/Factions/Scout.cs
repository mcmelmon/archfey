using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Scout : MonoBehaviour {

    public float speed = 6f;
    public float sense_radius = 40f;
    public float sense_perception = 20f;
    public List<Vector3> reports = new List<Vector3>();
    Geography geography;
    Actor actor;
    Movement movement;
    Senses senses;



    // Unity

    private void Awake()
    {
        geography = GetComponentInParent<World>().GetComponentInChildren<Geography>();
    }


    private void Start () {
        ConfigureRoleSpecificProperties();
        Strategize();
    }


    private void Update () 
    {
        ReportSightings();
    }


    // public


    public void Restrategize()
    {
        Route previous_route = movement.GetRoute();

        if (actor.attacker != null && previous_route != null) {
            Debug.Log("Offense scout is scouting");
        } else {
            Debug.Log("Defense scout is scouting");
        }
    }
  

    public void Strategize()
    {
        // TODO: differentiate between Mhoddim and Ghaddim approaches
        
        if (actor.attacker != null) {
            Debug.Log("Offense scout is scouting");
        }
        else {
            Debug.Log("Defense scout is scouting");
        }
    }


    // private


    private Vector3 AverageSightings()
    {
        Vector3 average = Vector3.zero;

        foreach (var sighting in senses.sightings)
        {
            if (sighting != null) {
                average += sighting.transform.position;
            }
        }

        return average;
    }


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


    private void ReportSightings()
    {
        if (senses.GetSightings().Count > 0){
            Vector3 average = AverageSightings();
            if (!reports.Contains(average)) {
                reports.Add(average);
            }
        } else {
            reports.Clear();
        }
    }
}