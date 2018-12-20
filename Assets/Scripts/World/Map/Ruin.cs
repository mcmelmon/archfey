using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruin : MonoBehaviour
{

    public Material ghaddim_skin;
    public Material mhoddim_skin;
    public Material unclaimed_skin;
    

    // properties

    public Conflict.Faction Control { get; set; }
    public static float ControlDistance { get; set; }
    public bool Controlled { get; set; }
    public List<RuinControlPoint> ControlPoints { get; set; }
    public static float MinimumRuinSpacing { get; set; }
    public GameObject NearestActor { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    // public


    public bool IsFriendlyTo(GameObject _unit)
    {
        if (!Controlled || _unit == null) return false;

        switch (Control)
        {
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

            foreach (var control_point in ControlPoints) {
                RuinControlPoint _point = control_point.GetComponent<RuinControlPoint>();
                Conflict.Faction _faction = _point.Faction;

                if (contending_faction == Conflict.Faction.None)
                    contending_faction = _faction;

                if ((_faction == Conflict.Faction.None)) {
                    Control = contending_faction = Conflict.Faction.None;
                    Controlled = false;
                    GetComponent<Renderer>().material = unclaimed_skin;
                    break;
                } else if (contending_faction != _faction) {
                    Control = contending_faction = Conflict.Faction.None;
                    Controlled = false;
                    GetComponent<Renderer>().material = unclaimed_skin;
                    break;
                }
            }

            if (contending_faction != Conflict.Faction.None)
                TransferControl(contending_faction);
        }
    }


    private void SetComponents()
    {
        Control = Conflict.Faction.None;
        Controlled = false;
        ControlPoints = new List<RuinControlPoint>();
        SetControlPoints();
        StartCoroutine(CheckControl());
    }


    private void SetControlPoints()
    {
        Circle _center = Circle.CreateCircle(transform.position, 10f, 3);

        foreach (var vertex in _center.vertices)
        {
            RuinControlPoint control_point = RuinControlPoint.CreateControlPoint(vertex);
            control_point.Ruin = this;
            control_point.transform.parent = Ruins.Instance.transform; // using the Ruin transform scales the point out
            ControlPoints.Add(control_point);
            Ruins.AllRuinControlPoints.Add(control_point.GetComponent<RuinControlPoint>());
        }

    }


    private void TransferControl(Conflict.Faction faction)
    {
        Control = faction;
        Controlled = true;
        GetComponent<Renderer>().material = (Control == Conflict.Faction.Ghaddim) ? ghaddim_skin : mhoddim_skin;
    }
}


public class RuinControlPoint : MonoBehaviour
{

    // properties

    public float ControlResistanceRating { get; set; }
    public float CurrentResistancePoints { get; set; }
    public Conflict.Faction Faction { get; set; }
    public Actor NearestActor { get; set; }
    public bool Occupied { get; set; }
    public Actor Occupier { get; set; }
    public Ruin Ruin { get; set; }
    public float StartingResistancePoints { get; set; }


    // static


    public static RuinControlPoint CreateControlPoint(Vector3 _position)
    {
        GameObject ruin_control_point = new GameObject { name = "Control Point" };
        ruin_control_point.transform.position = _position;
        ruin_control_point.AddComponent<RuinControlPoint>();
        ruin_control_point.GetComponent<RuinControlPoint>().SetComponents();

        return ruin_control_point.GetComponent<RuinControlPoint>();
    }


    // private


    private IEnumerator CheckOccupation()
    {
        while (true) {
            if (NearestActor == null) {
                Occupied = false;
                Occupier = null;
                Faction = Conflict.Faction.None;
                yield return null;
            } else {
                yield return new WaitForSeconds(Turn.action_threshold);

                float distance = (NearestActor != null) ? Vector3.Distance(NearestActor.transform.position, transform.position) : float.MaxValue;
                if (!Occupied && distance < Route.reached_threshold) {
                    CurrentResistancePoints -= Mathf.Clamp((NearestActor.RuinControlRating - ControlResistanceRating) + Assitance(), 0, NearestActor.RuinControlRating);
                    if (CurrentResistancePoints <= 0) {
                        CurrentResistancePoints = 0;
                        NearestActor.RuinControlPoint = this;
                        Occupied = true;
                        Occupier = NearestActor;
                        Faction = NearestActor.GetComponent<Actor>().Faction;

                        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        marker.name = "Marker";
                        marker.transform.position = transform.position;
                        marker.transform.localScale = new Vector3(1, 5, 1);
                        marker.transform.parent = transform;
                        marker.GetComponent<Renderer>().material.color = Color.red;
                    }
                } else if (Occupied && distance > Route.reached_threshold) {
                    CurrentResistancePoints += ControlResistanceRating + 0.5f;
                    if (CurrentResistancePoints >= StartingResistancePoints) {
                        CurrentResistancePoints = StartingResistancePoints;
                        Occupied = false;
                        if (Occupier != null) Occupier.GetComponent<Actor>().RuinControlPoint = null;
                        Occupier = null;
                        Faction = Conflict.Faction.None;
                        Destroy(transform.Find("Marker").gameObject);
                    }
                }
            }
        }
    }


    private IEnumerator FindNearestActor()
    {
        while (true) {
            yield return new WaitForSeconds(Turn.action_threshold);

            float shortest_distance = float.MaxValue;
            float distance;
            GameObject nearest_actor = null;

            foreach (var unit in Conflict.Units) {
                if (unit == null) continue;
                distance = Vector3.Distance(transform.position, unit.transform.position);
                if (distance < shortest_distance)
                {
                    shortest_distance = distance;
                    nearest_actor = unit;
                }
            }

            NearestActor = nearest_actor.GetComponent<Actor>();
        }
    }


    private float Assitance()
    {
        float assistance = 0;
        float distance;

        foreach (var unit in Conflict.Units) {
            if (unit == null) continue;
            distance = Vector3.Distance(transform.position, unit.transform.position);
            if (distance < Route.reached_threshold) {
                assistance += Mathf.Clamp(unit.GetComponent<Actor>().RuinControlRating - ControlResistanceRating, 0, unit.GetComponent<Actor>().RuinControlRating);
            }
        }

        return assistance;
    }


    private void SetComponents()
    {
        ControlResistanceRating = 2;
        CurrentResistancePoints = StartingResistancePoints = 12;
        Faction = Conflict.Faction.None;
        Occupied = false;

        StartCoroutine(CheckOccupation());
        StartCoroutine(FindNearestActor());
    }
}
