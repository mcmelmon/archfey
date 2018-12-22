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
    public bool Controlled { get; set; }
    public List<RuinControlPoint> ControlPoints { get; set; }
    public static float MinimumRuinSpacing { get; set; }
    public GameObject NearestActor { get; set; }


    // static


    public static Ruin InstantiateRuin(Ruin prefab, Vector3 point, Ruins _ruins)
    {
        Ruin _ruin = Instantiate(prefab, point, _ruins.transform.rotation, _ruins.transform);
        _ruin.transform.localScale += new Vector3(4, 16, 4);
        _ruin.transform.position += new Vector3(0, _ruin.transform.localScale.y / 2, 0);
        _ruin.SetComponents();

        return _ruin;
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
        MinimumRuinSpacing = 20f;
        SetControlPoints();
        StartCoroutine(CheckControl());
    }


    private void SetControlPoints()
    {
        Circle _center = Circle.CreateCircle(new Vector3 (transform.position.x, 0, transform.position.z), 10f, 3);

        foreach (var vertex in _center.vertices)
        {
            RuinControlPoint control_point = RuinControlPoint.New(vertex);
            control_point.Ruin = this;
            control_point.transform.parent = Ruins.Instance.transform; // using the Ruin transform scales the point out
            ControlPoints.Add(control_point);
            Ruins.RuinControlPoints.Add(control_point.GetComponent<RuinControlPoint>());
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


    public static RuinControlPoint New(Vector3 _position)
    {
        GameObject ruin_control_point = new GameObject { name = "Control Point" };
        ruin_control_point.transform.position = _position;
        ruin_control_point.AddComponent<RuinControlPoint>();
        ruin_control_point.GetComponent<RuinControlPoint>().SetComponents();

        return ruin_control_point.GetComponent<RuinControlPoint>();
    }


    private void Start()
    {
        StartCoroutine(CheckOccupation());
        StartCoroutine(FindNearestActor());
    }


    // public


    public Actor ConfirmOccupation()
    {
        if (Occupier != NearestActor) {
            if (Occupier.Ghaddim == NearestActor.Ghaddim && Occupier.Mhoddim == NearestActor.Mhoddim) {
                Occupier.GetComponent<Renderer>().material.color = Color.white;
                Occupier.RuinControlPoint = null;
                Occupier = NearestActor;
            }
        }
        return Occupier;
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
            } else if (Occupier != null && NearestActor.Faction != Occupier.Faction) {
                CurrentResistancePoints += ControlResistanceRating;
            }
            else {
                yield return new WaitForSeconds(Turn.action_threshold);

                float distance = (NearestActor != null) ? Vector3.Distance(NearestActor.transform.position, transform.position) : float.MaxValue;

                if (!Occupied && distance < Route.reached_threshold) {
                    CurrentResistancePoints -= Mathf.Clamp((NearestActor.RuinControlRating - ControlResistanceRating) + Assitance(), 0, NearestActor.RuinControlRating);
                    if (CurrentResistancePoints <= 0) {
                        CurrentResistancePoints = 0;
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
                } else if (Occupied && Vector3.Distance(Occupier.transform.position, transform.position) > Route.reached_threshold) {
                    CurrentResistancePoints += ControlResistanceRating;
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

            for (int i = 0; i < Conflict.Units.Count; i++) {
                GameObject _unit = Conflict.Units[i];
                if (_unit == null || transform == null) continue;

                distance = Vector3.Distance(transform.position, _unit.transform.position);
                if (distance < shortest_distance) {
                    shortest_distance = distance;
                    nearest_actor = _unit;
                }
            }

            if (nearest_actor != null)
                NearestActor = nearest_actor.GetComponent<Actor>();
        }
    }


    private float Assitance()
    {
        float assistance = 0;
        float distance;

        for (int i = 0; i < Conflict.Units.Count; i++) {
            GameObject _unit = Conflict.Units[i];
            if (_unit == null || transform == null) continue;
            distance = Vector3.Distance(transform.position, _unit.transform.position);
            if (distance < Route.reached_threshold) {
                assistance += Mathf.Clamp(_unit.GetComponent<Actor>().RuinControlRating - ControlResistanceRating, 0, _unit.GetComponent<Actor>().RuinControlRating);
            }
        }

        return assistance;
    }


    private void SetComponents()
    {
        ControlResistanceRating = Random.Range(1,6);
        CurrentResistancePoints = StartingResistancePoints = 100 + Random.Range(0,8);
        Faction = Conflict.Faction.None;
        Occupied = false;
    }
}
