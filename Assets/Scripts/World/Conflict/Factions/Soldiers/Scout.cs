using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Scout : MonoBehaviour {

    // properties

    public Actor Actor { get; set; }
    public float PerceptionRange { get; set; }
    public int PerceptionRating { get; set; }
    public float Speed { get; set; }
    public List<Ruin> SpottedRuins { get; set; }
    public int StealthPersistence { get; set; }
    public int StealthRating { get; set; }


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
        new_route.AccumulateRoutes(Actor.Movement.Route);  // store our old routes in the new route in case we want to backtrack
        Actor.Movement.SetRoute(new_route);
    }
  

    public void Strategize()
    {
        // move around the map in a circle with a radius equal to my distance from the map center

        float distance_to_center = Mathf.Min(Vector3.Distance(Geography.Instance.GetCenter(), transform.position), Geography.Instance.GetResolution() - 20f);
        Circle scouting_path = Circle.CreateCircle(Geography.Instance.GetCenter(), distance_to_center);
        Vector3 nearest_vertex = scouting_path.VertexClosestTo(transform.position);

        Actor.Movement.SetRoute(Route.Circular(nearest_vertex, scouting_path, Restrategize));
    }


    // private


    private void SetComponents()
    {
        PerceptionRange = 40f;
        PerceptionRating = 25;
        Speed = 2.5f;
        StealthPersistence = 15;
        StealthRating = 50;

        Actor = GetComponent<Actor>();
        Actor.RuinControlRating = 20;
        Actor.Movement.Agent.speed = Speed;
        Actor.Senses.PerceptionRating = PerceptionRating;
        Actor.Senses.SetRange(PerceptionRange);
        Actor.Stealth = gameObject.AddComponent<Stealth>();
        Actor.Stealth.stealth_rating = StealthRating;
        Actor.Stealth.stealh_persistence = StealthPersistence;
    }


    private void SetStats()
    {
        if (Actor.Ghaddim != null) {
            Actor.Ghaddim.SetStats();
        } else if (Actor.Mhoddim != null) {
            Actor.Mhoddim.SetStats();
        }
    }
}