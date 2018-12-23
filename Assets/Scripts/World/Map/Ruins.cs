﻿using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Ruins : MonoBehaviour {

    public enum Category { Primary = 0, Secondary = 1, Tertiary = 2 };
    public Ruin ruin_prefab;


    // properties

    public static List<Ruin> RuinBlocks { get; set; }
    public static List<RuinControlPoint> RuinControlPoints { get; set; }
    public static Ruins Instance { get; set; }


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


    public void ErectRuins()
    {
        SetComponents();
        Construct();
    }


    public RuinControlPoint GetNearestUnoccupiedControlPoint(GameObject _unit)
    {
        float distance;
        float shortest_distance = float.MaxValue;
        RuinControlPoint nearest_control_point = null;

        foreach (var control_point in RuinControlPoints) {
            if (!control_point.Occupied) {
                distance = Vector3.Distance(control_point.transform.position, _unit.transform.position);
                if (distance < shortest_distance) {
                    nearest_control_point = control_point;
                    shortest_distance = distance;
                }
            }
        }

        return nearest_control_point;
    }


    public Ruin RuinClosestTo(Vector3 location)
    {
        Ruin _ruin = null;
        float shortest_distance = float.MaxValue;
        float distance;

        foreach (var ruin in RuinBlocks) {
            distance = Vector3.Distance(location, ruin.transform.position);
            if (distance < shortest_distance) {
                shortest_distance = distance;
                _ruin = ruin;
            }
        }

        return _ruin;
    }


    // private


    private void Construct()
    {
        // pick a set of random tiles, build a ruin on each, and add all of the ruins into a complex

        UnityEngine.Random.InitState(DateTime.Now.Millisecond);

        foreach (var tile in Geography.Instance.RandomTiles(20)) {
            if (tile.Location.x < 20 || tile.Location.x > Geography.Instance.GetResolution() - 20 || tile.Location.z < 20 || tile.Location.z > Geography.Instance.GetResolution() - 20) continue;

            Ruin nearest_ruin = RuinClosestTo(tile.Location);
            if (nearest_ruin != null && Vector3.Distance(nearest_ruin.transform.position, tile.Location) < Ruin.MinimumRuinSpacing) continue;

            Ruin _ruin = Ruin.InstantiateRuin(ruin_prefab, tile.Location, this);
            RuinBlocks.Add(_ruin);
            RuinControlPoints.AddRange(_ruin.ControlPoints);
            tile.Ruin = _ruin;
        }
    }


    private void SetComponents()
    {
        RuinBlocks = new List<Ruin>();
        RuinControlPoints = new List<RuinControlPoint>();
    }
}