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


    public void Strategize()
    {
        // TODO: differentiate between Mhoddim and Ghaddim

        if (has_objective || units.Count <= 0) return;

        if (units[0].GetComponent<Striker>() != null) {
            // move toward scout report
            Scout[] scouts = Object.FindObjectsOfType<Scout>();
            foreach (var scout in scouts) {
                if (scout.reports.Count > 0) {
                    if (scout.GetComponent<Defend>() == units[0].GetComponent<Defend>()) {
                        // move the formation's units to the reported location.
                        // TODO: move in formation
                        // TODO: give the Formation a Route
                        has_objective = true;
                        foreach (var unit in units){
                            unit.GetComponent<Movement>().GetAgent().SetDestination(scout.reports[0]);
                        }
                    }
                }
            }

        }
        else if (units[0].GetComponent<Heavy>() != null) {
           
        }
    }


    // private


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