using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Objectives : MonoBehaviour {

    // Inspector settings
    public Objective objective_prefab;
    public List<Objective> objectives;

    // properties

    public static Dictionary<Conflict.Faction, List<Objective>> HeldByFaction { get; set; }
    public static Objectives Instance { get; set; }
    public static List<ObjectiveControlPoint> ObjectiveControlPoints { get; set; }


    // Unity


    private void Awake()
    {
        if (Instance != null) {
            Debug.LogError("More than one ruins instance");
            Destroy(this);
            return;
        }
        Instance = this;
    }


    // public


    public void AccountForControl(Conflict.Faction new_faction, Conflict.Faction previous_faction, Objective _ruin)
    {
        if (!HeldByFaction[new_faction].Contains(_ruin)) {
            HeldByFaction[new_faction].Add(_ruin);
            HeldByFaction[previous_faction].Remove(_ruin);
        }
    }


    public void ErectRuins()
    {
        SetComponents();
        Conflict.VictoryThreshold = Mathf.RoundToInt(objectives.Count * 0.66f);
    }


    public ObjectiveControlPoint GetNearestUnoccupiedControlPoint(Actor _actor)
    {
        float distance;
        float shortest_distance = float.MaxValue;
        ObjectiveControlPoint nearest_control_point = null;

        foreach (var control_point in ObjectiveControlPoints) {
            if (control_point.Faction != _actor.Faction) {  // if unoccupied, will have Faction of None
                distance = Vector3.Distance(control_point.transform.position, _actor.transform.position);
                if (distance < shortest_distance) {
                    nearest_control_point = control_point;
                    shortest_distance = distance;
                }
            }
        }

        return nearest_control_point;
    }


    public Objective ObjectiveNearest(Vector3 location)
    {
        Objective _objective = null;
        float shortest_distance = float.MaxValue;
        float distance;

        foreach (var objective in objectives) {
            distance = Vector3.Distance(location, objective.transform.position);
            if (distance < shortest_distance) {
                shortest_distance = distance;
                _objective = objective;
            }
        }

        return _objective;
    }


    // private


    private void SetComponents()
    {
        HeldByFaction = new Dictionary<Conflict.Faction, List<Objective>>
        {
            [Conflict.Faction.Ghaddim] = new List<Objective>(),
            [Conflict.Faction.Mhoddim] = new List<Objective>(),
            [Conflict.Faction.None] = new List<Objective>()
        };
        ObjectiveControlPoints = new List<ObjectiveControlPoint>();
        foreach (var objective in objectives) {
            ObjectiveControlPoints.AddRange(objective.control_points);
            HeldByFaction[objective.Control].Add(objective);
        }
    }
}