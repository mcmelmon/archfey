using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Scout : MonoBehaviour {

    Geography geography;
    Actor actor;
    readonly float sense_radius = 40f;

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

    }


    // public


    public void Restrategize()
    {
        Route previous_route = actor.movement.GetRoute();
        Route new_route;

        if (actor.attack != null && previous_route != null) {
            // Scout a smaller concentric circle
            Circle _circle = Circle.CreateCircle(geography.GetCenter(), previous_route.path.radius * .7f, 18);
            new_route = Route.CircularRoute(_circle.VertexClosestTo(transform.position), _circle, false, Restrategize);
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
            new_route = Route.CircularRoute(next_circle.VertexClosestTo(transform.position), next_circle, false, Restrategize);
            new_route.AccumulateRoutes(previous_route);
        }

        actor.movement.SetRoute(new_route);
    }
  

    public void Strategize()
    {
        // TODO: differentiate between Mhoddim and Ghaddim approaches

        Route _route;

        if (actor.attack != null) {
            Circle _circle = Circle.CreateCircle(geography.GetCenter(), (geography.GetResolution() / 2f) - sense_radius, 18);
            _route = Route.CircularRoute(_circle.VertexClosestTo(transform.position), _circle, false, Restrategize);
        } else {
            Dictionary<Ruins.Category, Circle> ruin_circles = GetComponentInParent<Defense>().GetRuinCircles();
            _route = Route.CircularRoute(ruin_circles[Ruins.Category.Tertiary].VertexClosestTo(transform.position), ruin_circles[Ruins.Category.Tertiary], false, Restrategize);
        }

        actor.movement.SetRoute(_route);
    }


    // private


    private void ConfigureRoleSpecificProperties()
    {
        GetComponent<Senses>().SetRange(sense_radius);
        actor = GetComponent<Actor>();
        actor.SetComponents();
        actor.SetStats();
    }
}