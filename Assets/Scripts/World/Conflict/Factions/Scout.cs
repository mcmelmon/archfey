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
        Route previous_route = actor.movement.GetRoute();
        Route new_route;

        if (actor.attacker != null && previous_route != null) {
            // Scout a smaller concentric circle around the map
            Circle _circle = Circle.CreateCircle(geography.GetCenter(), previous_route.circuitous.path.radius * .7f, 18);
            new_route = Route.Circular(_circle.VertexClosestTo(transform.position), _circle, false, Restrategize);
            new_route.AccumulateRoutes(previous_route);
        } else {
            Dictionary<Ruins.Category, Circle> ruins_by_category = GetComponentInParent<Defense>().GetRuinCircles();
            List<Circle> _ruins = new List<Circle>();

            // Create a list of the ruin circles
            foreach (KeyValuePair<Ruins.Category, Circle> keyValue in ruins_by_category) {
                _ruins.Add(keyValue.Value);
            }

            // Clear the paths traveled if we've covered every ruin circle
            if (_ruins.Count == previous_route.routes_followed.Count) {
                previous_route.routes_followed.Clear();
            }

            // Patrol the first ruin that we haven't traveled (recently)
            Circle next_circle = _ruins[previous_route.routes_followed.Count];
            new_route = Route.Circular(next_circle.VertexClosestTo(transform.position), next_circle, false, Restrategize);
            new_route.AccumulateRoutes(previous_route);
        }

        actor.Move(new_route);
    }
  

    public void Strategize()
    {
        // TODO: differentiate between Mhoddim and Ghaddim approaches

        Route _route;

        if (actor.attacker != null) {
            // Circle the map
            Circle _circle = Circle.CreateCircle(geography.GetCenter(), (geography.GetResolution() / 2f) - sense_radius, 18);
            _route = Route.Circular(_circle.VertexClosestTo(transform.position), _circle, false, Restrategize);
        } else {
            // Move to the tertiary circle and patrol it, other circles handled by Restrategize()
            Dictionary<Ruins.Category, Circle> ruin_circles = GetComponentInParent<Defense>().GetRuinCircles();
            _route = Route.Circular(ruin_circles[Ruins.Category.Tertiary].VertexClosestTo(transform.position), ruin_circles[Ruins.Category.Tertiary], false, Restrategize);
        }

        actor.Move(_route);
    }


    // private


    private Vector3 AverageSightings()
    {
        Vector3 average = Vector3.zero;

        foreach (var sighting in senses.sightings)
        {
            average += sighting.transform.position;
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
        actor.movement.GetAgent().speed = speed;
    }


    private void ReportSightings()
    {
        if (senses.sightings.Count > 0){
            Vector3 average = AverageSightings();
            if (!reports.Contains(average)) {
                reports.Add(average);
            }
        } else {
            reports.Clear();
        }
    }
}