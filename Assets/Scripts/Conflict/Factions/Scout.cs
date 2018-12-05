using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Scout : MonoBehaviour {

    Geography geography;
    Mhoddim mhoddim;
    Ghaddim ghaddim;
    Attack attack;
    Defend defend;
    Movement movement;
    readonly Senses senses;
    readonly float sense_radius = 40f;

    // Unity

    private void Awake()
    {
        geography = GetComponentInParent<World>().GetComponentInChildren<Geography>();
        mhoddim = GetComponent<Mhoddim>();
        ghaddim = GetComponent<Ghaddim>();
        attack = GetComponent<Attack>();
        defend = GetComponent<Defend>();
        movement = GetComponent<Movement>();
        GetComponent<Senses>().radius = sense_radius;
    }


    private void Start () {
        Strategize();
    }


    private void Update () 
    {

    }


    // private


    private void Restrategize()
    {
        Route previous_route = movement.GetRoute();
        Route new_route;

        if (attack != null && previous_route != null) {
            // Scout a smaller concentric circle
            Circle _circle = Circle.CreateCircle(geography.GetCenter(), previous_route.path.radius * .7f, 18);
            new_route = Route.CircularRoute(_circle.VertexClosestTo(transform.position), _circle, false, Restrategize);
            new_route.AccumulateRoutes(previous_route);
        } else {
            Dictionary<string, Circle> ruins_by_category = GetComponentInParent<Defense>().GetRuinCircles();
            List<Circle> _ruins = new List<Circle>();

            // Create a list of the ruin circles
            foreach (KeyValuePair<string, Circle> keyValue in ruins_by_category) {
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

        movement.SetRoute(new_route);
    }
  

    private void Strategize()
    {
        // TODO: differentiate between Mhoddim and Ghaddim approaches

        Route _route;

        if (attack != null) {
            Circle _circle = Circle.CreateCircle(geography.GetCenter(), (geography.GetResolution() / 2f) - sense_radius, 18);
            _route = Route.CircularRoute(_circle.VertexClosestTo(transform.position), _circle, false, Restrategize);
        } else {
            Dictionary<string, Circle> ruin_circles = GetComponentInParent<Defense>().GetRuinCircles();
            _route = Route.CircularRoute(ruin_circles["tertiary"].VertexClosestTo(transform.position), ruin_circles["tertiary"], false, Restrategize);
        }

        movement.SetRoute(_route);
    }
}