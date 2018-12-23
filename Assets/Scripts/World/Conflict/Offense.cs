using System.Collections;
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

        for (int i = 0; i < 14; i++)
        {
            Tile tile = Geography.Instance.RandomUnoccupiedTile();
            GameObject _heavy = Spawn(tile.Location);
            _heavy.AddComponent<Heavy>();
            tile.Occupier = _heavy.GetComponent<Actor>();
        }

        for (int i = 0; i < 6; i++)
        {
            Tile tile = Geography.Instance.RandomUnoccupiedTile();
            GameObject _striker = Spawn(tile.Location);
            _striker.AddComponent<Striker>();
            tile.Occupier = _striker.GetComponent<Actor>();
        }
    }


    // private


    private void LocateScout()
    {
        float distance_from_edge_percent = 0.1f;
        bool grounded = true;
        Vector3 circle_center = Geography.Instance.PointBetween(Geography.Instance.RandomBorderLocation(), Geography.Instance.GetCenter(), distance_from_edge_percent, grounded);
        Circle attack_circle = Circle.CreateCircle(circle_center, 15f);

    }


    private void SetComponents()
    {
        Units = new List<GameObject>();
    }


    private GameObject Spawn(Vector3 _point)
    {
        GameObject _soldier = (Faction == Conflict.Faction.Ghaddim) ? Ghaddim.SpawnUnit(_point) : Mhoddim.SpawnUnit(_point);
        _soldier.transform.parent = transform;
        _soldier.GetComponent<Actor>().Role = Conflict.Role.Offense;
        Units.Add(_soldier);
        Conflict.Units.Add(_soldier);
        return _soldier;
    }


    private void SpawnScouts()
    {
        foreach (Map.Cardinal border in Enum.GetValues(typeof(Map.Cardinal))) {
            // For each border, choose a location between its endpoints.
            // Move a short distance from that point toward the center.

            if (border == Map.Cardinal.Sky) continue;
            Vector3[] end_points = Geography.Instance.GetBorder(border);
            Vector3 entry_point = Geography.Instance.PointBetween(end_points[1], end_points[0], UnityEngine.Random.Range(0.05f, .95f), true);
            Vector3 spawn_point = Geography.Instance.PointBetween(entry_point, Geography.Instance.GetCenter(), UnityEngine.Random.Range(0.15f, 0.25f), true);
            GameObject _scout = Spawn(spawn_point);
            _scout.AddComponent<Scout>();
            Units.Add(_scout);
        }
    }
}