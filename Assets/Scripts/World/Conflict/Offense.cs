﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Offense : MonoBehaviour
{
    // properties

    public Conflict.Faction Faction { get; set; }
    public static Offense Instance { get; set; }
    public static List<GameObject> Units { get; set; }


    // Unity


    private void Awake()
    {
        if (Instance != null) {
            Debug.LogError("More than one offense instance");
            Destroy(this);
            return;
        }
        Instance = this;
        SetComponents();
    }


    // public


    public void Deploy()
    {
        // must be called by Conflict instead of Start to ensure Map setup complete

        foreach (var objective in Objectives.Instance.objectives)
        {
            if (objective.name == "ElvenRuin")
            {
                for (int i = 0; i < 3; i++)
                {
                    Circle spawn_circle = Circle.New(objective.control_points[0].transform.position, 5);
                    Vector3 _point = spawn_circle.RandomContainedPoint();
                    GameObject gnoll = Spawn(new Vector3(_point.x, objective.control_points[0].transform.position.y, _point.z));
                    gnoll.AddComponent<Gnoll>();
                }
            }
        }
    }


    // private


    private void SetComponents()
    {
        Units = new List<GameObject>();
    }


    private GameObject Spawn(Vector3 _point)
    {
        GameObject _soldier = (Faction == Conflict.Faction.Ghaddim) ? Ghaddim.SpawnUnit(_point) : Mhoddim.SpawnUnit(_point);  // offense will almost always be Ghaddim
        _soldier.transform.parent = transform;
        _soldier.GetComponent<Actor>().Role = Conflict.Role.Offense;
        Units.Add(_soldier);
        Conflict.Units.Add(_soldier);
        return _soldier;
    }
}