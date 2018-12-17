using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruin : MonoBehaviour
{

    public static float minimum_ruin_proximity = 15f;

    public bool is_controlled;
    public Material ghaddim_skin;
    public Material mhoddim_skin;
    public Material unclaimed_skin;

    List<GameObject> control_points = new List<GameObject>();
    Conflict.Faction control;

    // Unity


    private void Awake()
    {
        control = Conflict.Faction.None;
        is_controlled = false;
        SetControlPoints();
        StartCoroutine(CheckControl());
    }


    // public


    public GameObject GetNearestUnoccupiedControlPoint(Vector3 _location)
    {
        float distance;
        float shortest_distance = float.MaxValue;
        GameObject nearest_control_point = null;

        foreach (var control_point in control_points) {
            if (!control_point.GetComponent<RuinControlPoint>().IsOccupied()) {
                distance = Vector3.Distance(control_point.transform.position, _location);
                if (distance < shortest_distance) {
                    nearest_control_point = control_point;
                    shortest_distance = distance;
                }
            }
        }

        return nearest_control_point;
    }


    public bool IsFriendlyTo(GameObject _unit)
    {
        if (!is_controlled || _unit == null) return false;

        switch (control) {
            case Conflict.Faction.Ghaddim:
                return _unit.GetComponent<Ghaddim>() != null;
            case Conflict.Faction.Mhoddim:
                return _unit.GetComponent<Mhoddim>() != null;
            default:
                return false;
        }
    }


    // private


    private IEnumerator CheckControl()
    {
        while (true) {
            yield return new WaitForSeconds(Turn.action_threshold);

            Conflict.Faction contending_faction = Conflict.Faction.None;

            foreach (var control_point in control_points) {
                RuinControlPoint _point = control_point.GetComponent<RuinControlPoint>();
                Conflict.Faction _faction = _point.faction;

                if (contending_faction == Conflict.Faction.None)
                    contending_faction = _faction;

                if ((_faction == Conflict.Faction.None)) {
                    control = contending_faction = Conflict.Faction.None;
                    is_controlled = false;
                    GetComponent<Renderer>().material = unclaimed_skin;
                    break;
                } else if (contending_faction != _faction) {
                    control = contending_faction = Conflict.Faction.None;
                    is_controlled = false;
                    GetComponent<Renderer>().material = unclaimed_skin;
                    break;
                }
            }

            if (contending_faction != Conflict.Faction.None)
                TransferControl(contending_faction);
        }
    }


    private void SetControlPoints()
    {
        Circle _center = Circle.CreateCircle(transform.position, 10f, 3);

        foreach (var vertex in _center.vertices) {
            GameObject control_point = RuinControlPoint.CreateControlPoint(vertex, this);
            control_points.Add(control_point);
        }

    }


    private void TransferControl(Conflict.Faction faction)
    {
        control = faction;
        is_controlled = true;
        GetComponent<Renderer>().material = (control == Conflict.Faction.Ghaddim) ? ghaddim_skin : mhoddim_skin;
    }
}


public class RuinControlPoint : MonoBehaviour
{
    public Conflict.Faction faction;

    GameObject occupier;
    int control_resistance_rating;
    int current_resistance_points;
    bool occupied;
    int starting_resistance_points;


    public static GameObject CreateControlPoint(Vector3 _position, Ruin _ruin)
    {
        GameObject ruin_control_point = new GameObject();
        ruin_control_point.name = "Control Point";
        ruin_control_point.transform.position = _position;
        ruin_control_point.AddComponent<RuinControlPoint>();
        ruin_control_point.transform.parent = _ruin.GetComponentInParent<Ruins>().transform;

        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
        marker.name = "Marker";
        marker.transform.position = ruin_control_point.transform.position;
        marker.transform.localScale = new Vector3(1, 5, 1);
        marker.transform.parent = ruin_control_point.transform;
        
        return ruin_control_point;
    }


    // Unity

    private void Awake()
    {
        control_resistance_rating = 2;
        current_resistance_points = 10;
        faction = Conflict.Faction.None;
        occupied = false;
        starting_resistance_points = 10;
        StartCoroutine(CheckControl());
    }


    // public


    public Conflict.Faction ControllingFaction()
    {
        return faction;
    }


    public bool IsOccupied()
    {
        return occupied;
    }


    public bool OccupiedBy(GameObject _unit)
    {
        return occupier == _unit;
    }


    public void Occupy(GameObject _unit)
    {
        if (!occupied) {
            occupier = _unit;
            occupied = true;
        }
    }


    // private


    private IEnumerator CheckControl()
    {
        while (true) {
            yield return new WaitForSeconds(Turn.action_threshold);

            if (IsOccupied()) {
                Actor actor = occupier.GetComponent<Actor>();

                current_resistance_points -= (actor.ruin_control_rating - control_resistance_rating);
                if (current_resistance_points <= 0) {
                    faction = (actor.GetComponent<Ghaddim>() != null) ? Conflict.Faction.Ghaddim : Conflict.Faction.Mhoddim;
                    current_resistance_points = starting_resistance_points;
                    transform.Find("Marker").GetComponent<Renderer>().material.color = Color.red;
                }
            }
        }
    }
}
