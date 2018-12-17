using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruin : MonoBehaviour
{

    public static float minimum_ruin_proximity = 15f;

    public bool is_controlled;
    public Material ghaddim_skin;
    public Material mhoddim_skin;

    List<GameObject> control_points = new List<GameObject>();
    Conflict.Faction control;

    // Unity


    private void Awake()
    {
        control = Conflict.Faction.None;
        is_controlled = false;
        SetControlPoints();
    }


    // public


    public GameObject GetUnoccupiedControlPoint()
    {
        foreach (var control_point in control_points) {
            if (!control_point.GetComponent<RuinControlPoint>().IsOccupied())
                return control_point;
        }

        return null;
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

        switch (control) {
            case Conflict.Faction.Ghaddim:
                GetComponent<Renderer>().material = ghaddim_skin;
                break;
            case Conflict.Faction.Mhoddim:
                GetComponent<Renderer>().material = mhoddim_skin;
                break;
        }
    }
}


public class RuinControlPoint : MonoBehaviour
{
    List<GameObject> contenders = new List<GameObject>();
    GameObject controller;
    float control_radius;
    float control_resistance_rating;
    SphereCollider control_zone;
    float current_resistance_points;
    Conflict.Faction faction;
    bool occupied;
    public float starting_resistance_points;


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
        control_radius = 1f;
        control_resistance_rating = 50;
        current_resistance_points = 10f;
        control_zone = gameObject.AddComponent<SphereCollider>();
        control_zone.radius = control_radius;
        control_zone.isTrigger = true;
        faction = Conflict.Faction.None;
        occupied = false;
        starting_resistance_points = 10f;
    }


    // public


    public bool IsOccupied()
    {
        return occupied;
    }


    public void TakeControl(GameObject _unit, float ruin_control_rating)
    {
        if (OccupiedBy(_unit)) {
            current_resistance_points -= starting_resistance_points * ruin_control_rating * control_resistance_rating;
            if (current_resistance_points <= 0) {
                controller = _unit;
                current_resistance_points = starting_resistance_points;
            }
        }
    }


    // private


    private IEnumerator CheckForOccupation()
    {
        while (!occupied) {
            yield return new WaitForSeconds(Turn.action_threshold);

            Collider[] colliders = Physics.OverlapSphere(transform.position, control_radius);

            for (int i = 0; i < colliders.Length; i++) {
                GameObject occupier = colliders[i].gameObject;

                if (occupier.tag == "Actor" && occupier.GetComponent<Fey>() == null) {
                    occupied = true;
                    contenders.Add(occupier);
                    faction = (occupier.GetComponent<Ghaddim>() != null) ? Conflict.Faction.Ghaddim : Conflict.Faction.Mhoddim;
                    break;
                }
            }
        }
    }


    private bool OccupiedBy(GameObject _unit)
    {
        return contenders.Contains(_unit) && contenders.Count == 1;
    }



}
