using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Objectives : MonoBehaviour {

    // Inspector settings
    public Objective objective_prefab;

    // properties

    public static List<Objective> AllObjectives { get; set; }
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
        Construct();
        Conflict.VictoryThreshold = Mathf.RoundToInt(AllObjectives.Count * 0.66f);
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

        foreach (var objective in AllObjectives) {
            distance = Vector3.Distance(location, objective.transform.position);
            if (distance < shortest_distance) {
                shortest_distance = distance;
                _objective = objective;
            }
        }

        return _objective;
    }


    // private


    private void Construct()
    {
        // pick a set of random tiles, build a ruin on each, and add all of the ruins into a complex

        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        int objective_count = 5;
        int buffer = 60;

        foreach (var tile in Geography.Instance.RandomTiles(objective_count)) {
            if (tile.Location.x < buffer || tile.Location.x > Geography.Instance.GetResolution() - buffer || tile.Location.z < buffer || tile.Location.z > Geography.Instance.GetResolution() - buffer) continue;

            Objective closest_objective = ObjectiveNearest(tile.Location);
            if (closest_objective != null && Vector3.Distance(closest_objective.transform.position, tile.Location) < Objective.MinimumSpacing) continue;

            Objective _objective = Objective.Create(objective_prefab, tile.Location, this);
            foreach (var rend in _objective.renderers) {
                rend.material = _objective.unclaimed_skin;
            }

            AllObjectives.Add(_objective);
            ObjectiveControlPoints.AddRange(_objective.control_points);
            HeldByFaction[Conflict.Faction.None].Add(_objective);
            tile.Objective = _objective;
        }
    }


    private void SetComponents()
    {
        AllObjectives = new List<Objective>();
        ObjectiveControlPoints = new List<ObjectiveControlPoint>();
        HeldByFaction = new Dictionary<Conflict.Faction, List<Objective>>
        {
            [Conflict.Faction.Ghaddim] = new List<Objective>(),
            [Conflict.Faction.Mhoddim] = new List<Objective>(),
            [Conflict.Faction.None] = new List<Objective>()
        };

    }
}