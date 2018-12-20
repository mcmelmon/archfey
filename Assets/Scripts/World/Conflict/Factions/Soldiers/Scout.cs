using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Scout : MonoBehaviour {

    public float perception_range = 40f;
    public int perception_rating = 25;
    public float speed = 2.5f;
    public int stealth_persistence = 15;
    public int stealth_rating = 50;

    Actor actor;
    Movement movement;
    Senses senses;
    Stealth stealth;

    List<Ruin> spotted_ruins = new List<Ruin>();

    // Unity


    private void Awake () {
        SetComponents();
        SetStats();
        Strategize();
    }


    private void Update()
    {
        // spot ruins
    }


    // public


    public void Restrategize()
    {
        // Create a new path around the center with a shorter radius

        float distance_to_center = Vector3.Distance(Geography.Instance.GetCenter(), transform.position);
        Circle scouting_path = Circle.CreateCircle(Geography.Instance.GetCenter(), distance_to_center - 20f);
        Vector3 nearest_vertex = scouting_path.VertexClosestTo(transform.position);

        Route new_route = Route.Circular(nearest_vertex, scouting_path, Restrategize);
        new_route.AccumulateRoutes(movement.Route);  // store our old routes in the new route in case we want to backtrack
        movement.SetRoute(new_route);
    }
  

    public void Strategize()
    {
        // move around the map in a circle with a radius equal to my distance from the map center

        float distance_to_center = Mathf.Min(Vector3.Distance(Geography.Instance.GetCenter(), transform.position), Geography.Instance.GetResolution() - 20f);
        Circle scouting_path = Circle.CreateCircle(Geography.Instance.GetCenter(), distance_to_center);
        Vector3 nearest_vertex = scouting_path.VertexClosestTo(transform.position);

        movement.SetRoute(Route.Circular(nearest_vertex, scouting_path, Restrategize));
    }


    // private


    private void SetComponents()
    {
        actor = GetComponent<Actor>();
        movement = GetComponent<Movement>();
        movement.Agent.speed = speed;
        senses = GetComponent<Senses>();
        senses.perception_rating = perception_rating;
        senses.SetRange(perception_range);
        stealth = gameObject.AddComponent<Stealth>();
        stealth.stealth_rating = stealth_rating;
        stealth.stealh_persistence = stealth_persistence;
        actor.Stealth = stealth;
    }


    private void SetStats()
    {
        if (actor.Ghaddim != null) {
            actor.Ghaddim.SetStats();
        } else if (actor.Mhoddim != null) {
            actor.Mhoddim.SetStats();
        }
    }
}