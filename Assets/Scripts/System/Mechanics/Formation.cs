using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formation
{
    // A formation manages homogenous units.  

    // TODO: manage heterogenous groups of units.

    public enum Profile { Circle = 0, Rectangle = 1 };

    public bool has_objective = false;
    public Profile profile;
    public Vector3 anchor;
    readonly List<GameObject> units = new List<GameObject>();
    Circle circular_formation;
    Rectangle rectangular_formation;
    float spacing;
    Route route;

    public static Formation CreateFormation(Vector3 _anchor, Profile _profile, float _spacing = 5f)
    {
        Formation _formation = new Formation
        {
            profile = _profile,
            anchor = _anchor,
            spacing = _spacing
        };

        return _formation;
    }


    public void Face(Vector3 facing)
    {
        switch (profile)
        {
            case Profile.Circle:
                foreach (var unit in units) {
                    facing = unit.transform.position - anchor;
                    facing.y = 0;
                    unit.transform.rotation = Quaternion.LookRotation(facing);
                }
                break;
            case Profile.Rectangle:
                foreach (var unit in units)
                {
                    facing.y = 0;
                    unit.transform.rotation = Quaternion.LookRotation(facing);
                }
                break;
        }
    }


    public void JoinFormation(GameObject unit)
    {
        units.Add(unit);

        switch (profile)
        {
            case Profile.Circle:
                circular_formation = Circle.CreateCircle(anchor, units.Count, units.Count);
                PositionCircle(circular_formation);
                break;
            case Profile.Rectangle:
                rectangular_formation = Rectangle.CreateRectangle(anchor, Mathf.RoundToInt(Mathf.Sqrt(units.Count)) + 1, Mathf.RoundToInt(Mathf.Sqrt(units.Count)) + 1, spacing);
                PositionRectangle(rectangular_formation);
                break;
        }
    }


    public void Restrategize()
    {
        if (units[0].GetComponent<Heavy>() != null) {
            // offense sits tight, defense patrols the ruins
            if (units[0].GetComponent<Defender>() != null)
            {
                // copied from Scout
                // TODO: don't copy from Scout
                Route previous_route = route;

                Dictionary<Ruins.Category, Circle> ruins_by_category = units[0].GetComponentInParent<Defense>().GetRuinCircles();
                List<Circle> _ruins = new List<Circle>();

                // Create a list of the ruin circles
                foreach (KeyValuePair<Ruins.Category, Circle> keyValue in ruins_by_category)
                {
                    _ruins.Add(keyValue.Value);
                }

                // Clear the paths traveled if we've covered every ruin circle
                if (_ruins.Count == previous_route.routes_followed.Count)
                {
                    previous_route.routes_followed.Clear();
                }

                // Patrol the first ruin that we haven't traveled (recently)
                Circle next_circle = _ruins[previous_route.routes_followed.Count];
                route = Route.Circular(next_circle.VertexClosestTo(anchor), next_circle, false, Restrategize);
                route.AccumulateRoutes(previous_route);
            }
        }
    }

    public void Strategize()
    {
        // TODO: differentiate between Mhoddim and Ghaddim

        if (has_objective || units.Count <= 0) return;

        if (units[0].GetComponent<Striker>() != null) {
            // move toward scout report
            Scout[] scouts = Object.FindObjectsOfType<Scout>();
            foreach (var scout in scouts) {
                if (scout.reports.Count > 0) {
                    if (scout.GetComponent<Defender>() == units[0].GetComponent<Defender>()) {
                        // move the formation's units to the reported location.
                        // TODO: move in formation
                        // TODO: give the Formation a Route
                        has_objective = true;
                        route = Route.Line(units[0].transform.position, scout.reports[0], true, Restrategize);  // march to the report and loop
                        March();
                    }
                }
            }

        }
        else if (units[0].GetComponent<Heavy>() != null) {
           // offense sits tight, defense patrols the ruins
            if (units[0].GetComponent<Defender>() != null) {
                // copied from Scout
                // TODO: don't copy from Scout
                // Move to the primary circle and patrol it, other circles handled by Restrategize()
                has_objective = true;
                Dictionary<Ruins.Category, Circle> ruin_circles = units[0].GetComponentInParent<Defense>().GetRuinCircles();
                route = Route.Circular(ruin_circles[Ruins.Category.Primary].VertexClosestTo(anchor), ruin_circles[Ruins.Category.Primary], true, Restrategize);
                March();
            }
        }
    }


    // private


    private void March()
    {
        foreach (var unit in units) {
            if (route != null) {
                unit.GetComponent<Movement>().SetRoute(route);
            }
        }
    }

    private void PositionCircle(Circle formation)
    {
        for (int i = 0; i < units.Count; i++)
        {
            units[i].transform.position = formation.vertices[i];
        }
        Face(Vector3.zero);
    }


    private void PositionRectangle(Rectangle formation)
    {
        for (int i = 0; i < units.Count; i++)
        {
            units[i].transform.position = formation.points[i];
        }
        Face(rectangular_formation.GetDepthDirection());
    }
}