using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

            Conflict.Faction new_faction = Conflict.Faction.None;
            Conflict.Faction previous_faction = Control;

            foreach (var control_point in ControlPoints) {
                RuinControlPoint _point = control_point.GetComponent<RuinControlPoint>();
                Conflict.Faction point_faction = _point.Faction;

                if (new_faction == Conflict.Faction.None)
                    // We have just entered the loop
                    new_faction = point_faction;

                if ((point_faction == Conflict.Faction.None)) {
                    Control = new_faction = Conflict.Faction.None;
                    Controlled = false;
                    GetComponent<Renderer>().material = unclaimed_skin;
                    break;
                } else if (new_faction != point_faction) {
                    Control = new_faction = Conflict.Faction.None;
                    Controlled = false;
                    GetComponent<Renderer>().material = unclaimed_skin;
                    break;
                }
            }

            if (new_faction != Conflict.Faction.None) {
                TransferControl(new_faction, previous_faction);
            }
        }
    }


    private void PresentRuinControl(Conflict.Faction new_faction, Conflict.Faction previous_faction)
    {
        GameObject ruin_control = Ruins.Instance.ruin_control_ui;

        if (new_faction != Conflict.Faction.None) {
            Transform _captured = ruin_control.transform.Find("RuinCaptured");
            Transform _text = _captured.Find("Text");
            Transform _faction = _text.Find("Faction");
            Transform _summary = _text.Find("Summary");
            TextMeshProUGUI faction_text = _faction.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI summary_text = _summary.GetComponent<TextMeshProUGUI>();


            switch (new_faction) {
                case Conflict.Faction.Ghaddim:
                    Ghaddim.Ruins.Add(this);
                    faction_text.text = "Ashen";
                    summary_text.text = "Ashen control " + Ghaddim.Ruins.Count + ((Ghaddim.Ruins.Count == 1) ? " ruin!" : " ruins!");
                    _captured.gameObject.SetActive(true);
                    break;
                case Conflict.Faction.Mhoddim:
                    Mhoddim.Ruins.Add(this);
                    faction_text.text = "Nibelung";
                    summary_text.text = "Nibelung control " + Mhoddim.Ruins.Count + ((Mhoddim.Ruins.Count == 1) ? " ruin!" : " ruins!");
                    _captured.gameObject.SetActive(true);
                    break;
            }
        } else {
            Transform _lost = ruin_control.transform.Find("RuinLost");
            Transform _text = _lost.Find("Text");
            Transform _faction = _text.Find("Faction");
            Transform _summary = _text.Find("Summary");
            TextMeshProUGUI faction_text = _faction.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI summary_text = _summary.GetComponent<TextMeshProUGUI>();

            switch (previous_faction) {
                case Conflict.Faction.Ghaddim:
                    Ghaddim.Ruins.Remove(this);
                    faction_text.text = "Ashen";
                    summary_text.text = "Ashen control " + Ghaddim.Ruins.Count + ((Ghaddim.Ruins.Count == 1) ? " ruin!" : " ruins!");
                    _lost.gameObject.SetActive(true);
                    break;
                case Conflict.Faction.Mhoddim:
                    Mhoddim.Ruins.Remove(this);
                    faction_text.text = "Nibelung";
                    summary_text.text = "Nibelung control " + Mhoddim.Ruins.Count + ((Mhoddim.Ruins.Count == 1) ? " ruin!" : " ruins!");
                    _lost.gameObject.SetActive(true);
                    break;
                case Conflict.Faction.None:
                    break;
            }
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


    private void TransferControl(Conflict.Faction new_faction, Conflict.Faction previous_faction)
    {
        Control = new_faction;
        Controlled = true;
        GetComponent<Renderer>().material = (Control == Conflict.Faction.Ghaddim) ? ghaddim_skin : mhoddim_skin;
        PresentRuinControl(new_faction, previous_faction);
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
            } else if (Occupier != null && NearestActor.Faction != Occupier.Faction) {
                CurrentResistancePoints += ControlResistanceRating;
            }
            else {
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
                        marker.GetComponent<Renderer>().material.color = Color.blue;
                    }
                } else if (Occupied && (Occupier == null || Occupier.Faction != Faction || Vector3.Distance(Occupier.transform.position, transform.position) > Route.reached_threshold)) {
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

            yield return new WaitForSeconds(Turn.action_threshold);
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
