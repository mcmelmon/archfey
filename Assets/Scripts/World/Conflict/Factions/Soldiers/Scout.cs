using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Scout : MonoBehaviour {

    public float perception_range = 40f;
    public float perception_rating = 0.25f;
    public float speed = 2.5f;
    public float stealth_persistence = 0.15f;
    public float stealth_rating = 0.5f;

    Actor actor;
    Geography geography;
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

        float distance_to_center = Vector3.Distance(geography.GetCenter(), transform.position);
        Circle scouting_path = Circle.CreateCircle(geography.GetCenter(), distance_to_center - 20f);
        Vector3 nearest_vertex = scouting_path.VertexClosestTo(transform.position);

        Route new_route = Route.Circular(nearest_vertex, scouting_path, false, false, Restrategize);
        new_route.AccumulateRoutes(movement.GetRoute());  // store our old routes in the new route in case we want to backtrack
        movement.SetRoute(new_route);
    }
  

    public void Strategize()
    {
        // move around the map in a circle with a radius equal to my distance from the map center

        float distance_to_center = Mathf.Min(Vector3.Distance(geography.GetCenter(), transform.position), geography.GetResolution() - 20f);
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
        senses.perception_rating = perception_rating;
        senses.SetRange(perception_range);
        stealth = gameObject.AddComponent<Stealth>();
        stealth.stealth_rating = stealth_rating;
        stealth.stealh_persistence = stealth_persistence;
        GetComponent<Turn>().SetStealth(stealth);
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