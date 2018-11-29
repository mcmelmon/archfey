﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Offense : MonoBehaviour 
{
    readonly Dictionary<string, Circle> attack_circles = new Dictionary<string, Circle>();
    readonly List<GameObject> deployed = new List<GameObject>();

    Dictionary<string, Circle> ruin_circles = new Dictionary<string, Circle>();
    Geography geography;

    Queue<GameObject> aggressors = new Queue<GameObject>();
    List<GameObject> scouts = new List<GameObject>();
    List<GameObject> strikers = new List<GameObject>();
    List<GameObject> heavies = new List<GameObject>();

    // Unity


    private void Awake()
    {
        geography = GetComponentInParent<World>().GetComponentInChildren<Geography>();
        ruin_circles = GetComponentInParent<World>().GetComponentInChildren<Ruins>().GetOrCreateRuinCircles();
    }


    private void Start()
    {

    }

    private void Update()
    {

    }


    // public


    public void Attack(Queue<GameObject> _aggressors)
    {
        aggressors = _aggressors;
        Locate();
        Deploy();
        Scout();
    }


    // private


    private void Deploy()
    {
        GameObject offense_parent = new GameObject();
        offense_parent.name = "Offense";
        offense_parent.transform.parent = transform;

        foreach (KeyValuePair<string, Circle> keyValue in attack_circles)
        {
            switch (keyValue.Key)
            {
                case "primary":
                    for (int i = 0; i < 12; i++)
                    {
                        heavies.Add( Spawn(keyValue.Value.RandomContainedPoint(), offense_parent.transform) );
                    }
                    break;
                case "secondary":
                    for (int i = 0; i < 5; i++)
                    {
                        strikers.Add( Spawn(keyValue.Value.RandomContainedPoint(), offense_parent.transform) );
                    }
                    break;
                case "tertiary":
                    for (int i = 0; i < 3; i++)
                    {
                        scouts.Add( Spawn(keyValue.Value.RandomContainedPoint(), offense_parent.transform) );
                    }
                    break;
            }
        }
    }


    private void Locate()
    {
        if (geography == null) geography = GetComponentInParent<World>().GetComponentInChildren<Geography>();
        LocatePrimaryAttack();
        LocateSecondaryAttack();
        LocateTertiaryAttack();
    }


    public void LocatePrimaryAttack()
    {
        Circle attack_circle = new Circle();
        float distance_from_edge_percent = 0.15f;
        bool grounded = true;
        Vector3 circle_center = geography.PointBetween(geography.RandomBorderLocation(), geography.GetCenter(), distance_from_edge_percent, grounded);

        attack_circles["primary"] = attack_circle.Inscribe(circle_center, 10f);
    }


    public void LocateSecondaryAttack()
    {
        Circle attack_circle = new Circle();
        float distance_from_edge_percent = 0.1f;
        bool grounded = true;
        Vector3 circle_center = geography.PointBetween(geography.RandomBorderLocation(), geography.GetCenter(), distance_from_edge_percent, grounded);

        attack_circles["secondary"] = attack_circle.Inscribe(circle_center, 5f);
    }


    public void LocateTertiaryAttack()
    {
        Circle attack_circle = new Circle();
        float distance_from_edge_percent = 0.1f;
        bool grounded = true;
        Vector3 circle_center = geography.PointBetween(geography.RandomBorderLocation(), geography.GetCenter(), distance_from_edge_percent, grounded);

        attack_circles["tertiary"] = attack_circle.Inscribe(circle_center, 5f);
    }


    private void Scout()
    {
        foreach (var scout in scouts)
        {
            scout.GetComponent<Actor>().SetDestination(geography.GetCenter());
        }
    }


    private GameObject Spawn(Vector3 point, Transform offense_parent)
    {
        GameObject _aggressor = aggressors.Dequeue();
        _aggressor.transform.position = point;
        _aggressor.transform.parent = offense_parent;
        _aggressor.SetActive(true);
        deployed.Add(_aggressor);
        return _aggressor;
    }
}