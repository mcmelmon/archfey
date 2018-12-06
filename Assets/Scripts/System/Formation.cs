using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formation
{
    // A formation manages homogenous units.  

    // TODO: manage heterogenous groups of units.

    public enum Profile { Round = 0, Square = 1, Triangle = 2 };

    public bool has_objective = false;
    public Profile profile;
    public Vector3 anchor;
    readonly List<GameObject> units = new List<GameObject>();
    Circle circular_formation;
    Rectangle rectangular_formation;


    public static Formation CreateFormation(Vector3 _anchor, Profile _profile)
    {
        Formation _formation = new Formation
        {
            profile = _profile,
            anchor = _anchor
        };

        return _formation;
    }


    public void Face(Vector3 facing)
    {
        switch (profile)
        {
            case Profile.Round:
                foreach (var unit in units) {
                    facing = unit.transform.position - anchor;
                    facing.y = 0;
                    unit.transform.rotation = Quaternion.LookRotation(facing);
                }
                break;
            case Profile.Square:
                foreach (var unit in units)
                {
                    facing.y = 0;
                    unit.transform.rotation = Quaternion.LookRotation(facing);
                }
                break;
            case Profile.Triangle:
                break;
        }
    }


    public void JoinFormation(GameObject unit)
    {
        units.Add(unit);

        switch (profile)
        {
            case Profile.Round:
                circular_formation = Circle.CreateCircle(anchor, units.Count, units.Count);
                PositionCircle(circular_formation);
                break;
            case Profile.Square:
                rectangular_formation = Rectangle.CreateRectangle(anchor, Mathf.RoundToInt(Mathf.Sqrt(units.Count)) + 1, Mathf.RoundToInt(Mathf.Sqrt(units.Count)) + 1, 5f);
                PositionRectangle(rectangular_formation);
                break;
            case Profile.Triangle:
                break;
        }
    }


    public void Strategize()
    {
        // TODO: differentiate between Mhoddim and Ghaddim

        if (has_objective) return;
        if (units[0].GetComponent<Striker>() != null)
        {
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
        else
        {
           
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