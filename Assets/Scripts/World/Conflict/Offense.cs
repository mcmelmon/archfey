using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Offense : MonoBehaviour
{
    public static Conflict.Faction faction;
    public static Offense offense_instance;
    public static List<GameObject> soldiers = new List<GameObject>();

    Geography geography;
    Ghaddim ghaddim;
    Mhoddim mhoddim;
    Ruins ruins;

    static List<GameObject> scouts = new List<GameObject>();


    // Unity


    private void Awake()
    {
        if (offense_instance != null)
        {
            Debug.LogError("More than one offense instance");
            Destroy(this);
            return;
        }
        offense_instance = this;
        SetComponents();
    }


    // public


    public void Setup()
    {
        // Scouts will enter the map at a random point along each border
        // Scouts will look for ruins
        // Heavy units will enter the map on the border closest to the largest spotted ruin
        // Striker units will spawn to either side.
        // Offense will try to advance from its border to the opposite

        SpawnScouts();
    }


    // private


    private void LocateScout()
    {
        float distance_from_edge_percent = 0.1f;
        bool grounded = true;
        Vector3 circle_center = geography.PointBetween(geography.RandomBorderLocation(), geography.GetCenter(), distance_from_edge_percent, grounded);
        Circle attack_circle = Circle.CreateCircle(circle_center, 15f);

    }


    private void SetComponents()
    {
        ruins = GetComponentInParent<World>().GetComponentInChildren<Ruins>();
        geography = GetComponentInParent<World>().GetComponentInChildren<Geography>();
        ghaddim = GetComponentInParent<Ghaddim>();
        mhoddim = GetComponentInParent<Mhoddim>();
        faction = (ghaddim != null) ? Conflict.Faction.Ghaddim : Conflict.Faction.Mhoddim;
    }


    private GameObject Spawn(Vector3 point)
    {
        GameObject _soldier = (ghaddim != null) ? ghaddim.SpawnUnit() : mhoddim.SpawnUnit();
        _soldier.transform.position = point;
        _soldier.transform.parent = transform;
        _soldier.GetComponent<Actor>().role = Conflict.Role.Offense;
        soldiers.Add(_soldier);
        return _soldier;
    }


    private void SpawnScouts()
    {
        if (scouts.Count >= 4) return;

        foreach (Map.Cardinal border in Enum.GetValues(typeof(Map.Cardinal))) {
            // For each border, choose a location between its endpoints.
            // Move a short distance from that point toward the center.

            if (border == Map.Cardinal.Sky) continue;
            Vector3[] end_points = geography.GetBorder(border);
            Vector3 entry_point = geography.PointBetween(end_points[1], end_points[0], UnityEngine.Random.Range(0.05f, .95f), true);
            Vector3 spawn_point = geography.PointBetween(entry_point, geography.GetCenter(), UnityEngine.Random.Range(0.15f, 0.25f), true);
            GameObject _scout = Spawn(spawn_point);
            _scout.AddComponent<Scout>();
            soldiers.Add(_scout);
            scouts.Add(_scout);
        }
    }
}