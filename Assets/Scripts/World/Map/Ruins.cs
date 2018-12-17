﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruins : MonoBehaviour {

    public enum Category { Primary = 0, Secondary = 1, Tertiary = 2 };

    public static Ruins ruins_instance;
    public static Dictionary<Category, Circle> ruin_circles = new Dictionary<Category, Circle>();

    public Ruin ruin_prefab;

    Geography geography;
    List<Ruin> ruins = new List<Ruin>();


    // Unity


    private void Awake()
    {
        if (ruins_instance != null) {
            Debug.LogError("More than one ruins instance");
            Destroy(this);
            return;
        }
        ruins_instance = this;
    }


    // public


    public void ErectRuins()
    {
        SetComponents();
        Locate();
        Construct();
    }


    public List<Vector3> GetRuinPositions()
    {
        List<Vector3> positions = new List<Vector3>();

        foreach (var ruin in ruins)
        {
            positions.Add(ruin.transform.position);
        }

        return positions;
    }


    public List<Ruin> GetRuins()
    {
        return ruins;
    }


    // private


    private void Construct()
    {
        foreach (KeyValuePair<Category, Circle> circle in ruin_circles) {
            switch (circle.Key) {
                case Category.Primary:
                    for (int i = 0; i < 9; i++) {
                        Vector3 position = circle.Value.RandomVertex();
                        if (!NearRuin(position, Ruin.minimum_ruin_proximity))
                            InstantiateRuin(position, this);
                    }
                    break;
                case Category.Secondary:
                    for (int i = 0; i < 5; i++) {
                        Vector3 position = circle.Value.RandomVertex();
                        if (!NearRuin(position, Ruin.minimum_ruin_proximity))
                            InstantiateRuin(position, this);
                    }
                    break;
                case Category.Tertiary:
                    for (int i = 0; i < 3; i++) {
                        Vector3 position = circle.Value.RandomVertex();
                        if (!NearRuin(position, Ruin.minimum_ruin_proximity))
                            InstantiateRuin(position, this);
                    }
                    break;
            }
        }
    }


    private void InstantiateRuin(Vector3 point, Ruins _ruins)
    {
        Ruin _ruin = Instantiate(ruin_prefab, point, transform.rotation, _ruins.transform);
        _ruin.transform.localScale += new Vector3(4, 16, 4);
        _ruin.transform.position += new Vector3(0, _ruin.transform.localScale.y / 2, 0);
        if (_ruin != null) ruins.Add(_ruin);
    }


    private void Locate()
    {
        LocatePrimaryRuinComplex();
        LocateSecondaryRuinComplex();
        LocateTertiaryRuinComplex();
    }


    private void LocatePrimaryRuinComplex()
    {
        float distance_from_edge = 100f;
        Vector3 circle_center = geography.RandomLocation(distance_from_edge);
        Circle spawn_circle = Circle.CreateCircle(circle_center, 40f);

        ruin_circles[Category.Primary] = spawn_circle;
    }


    private void LocateSecondaryRuinComplex()
    {
        float distance_from_edge = 80f;
        Vector3 circle_center = geography.RandomLocation(distance_from_edge);
        Circle spawn_circle = Circle.CreateCircle(circle_center, 20f);

        ruin_circles[Category.Secondary] = spawn_circle;
    }


    private void LocateTertiaryRuinComplex()
    {
        float distance_from_edge = 80f;
        Vector3 circle_center = geography.RandomLocation(distance_from_edge);
        Circle spawn_circle = Circle.CreateCircle(circle_center, 12f);

        ruin_circles[Category.Tertiary] = spawn_circle;
    }


    private bool NearRuin(Vector3 position, float how_close)
    {
        foreach (var ruin in GetRuinPositions())
        {
            float distance = Vector3.Distance(position, ruin);
            if (distance < how_close) return true;
        }

        return false;
    }


    private bool NearRuinCircle(Vector3 position, float how_close)
    {
        foreach (KeyValuePair<Category, Circle> circle in ruin_circles)
        {
            float distance = Vector3.Distance(position, circle.Value.center);
            if (distance < how_close) return true;
        }

        return false;
    }


    private void SetComponents()
    {
        geography = GetComponentInParent<Map>().GetComponentInChildren<Geography>();
    }
}