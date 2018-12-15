﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

    public static Map map_instance;
    public enum Cardinal { North = 0, East = 1, South = 2, West = 3, Sky = 4 };
    public Dictionary<Cardinal, Vector3[]> boundaries = new Dictionary<Cardinal, Vector3[]>();

    Geography geography;
    Terrain terrain;
    readonly int sky_height = 100;


    public struct HeavenAndEarth
    {
        public static Plane earth;
        public static Plane heaven;
    }


    // Unity


    private void Awake ()
    {
        if (map_instance != null){
            Debug.LogError("More than one map instance!");
            Destroy(this);
            return;
        }

        map_instance = this;
        SetComponents();
    }


    // public


    public void DrawMap()
    {
        SetHeavenAndEarth();
        SetBounds();
        SetFoundations();
    }


    // private


    void AddDirectionBoundaries()
    {
        GameObject bounds = new GameObject();
        bounds.transform.parent = transform;
        bounds.name = "Bounds";

        foreach (KeyValuePair <Cardinal, Vector3[]> boundary in boundaries)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.parent = bounds.transform;
            Vector3 heading = boundary.Value[1] - boundary.Value[0];
            wall.transform.localScale = new Vector3(heading.magnitude, heading.magnitude, 20);
            wall.transform.gameObject.GetComponentInChildren<Renderer>().enabled = false;
            wall.transform.up = heading;

            if (boundary.Key == Cardinal.East || boundary.Key == Cardinal.West) wall.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 90));

            if (boundary.Key == Cardinal.Sky) {
                wall.transform.position = new Vector3(terrain.terrainData.heightmapResolution / 2, sky_height, terrain.terrainData.heightmapResolution / 2);
                wall.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 90));
            } else {
                wall.transform.position = boundary.Value[0] + heading / 2;
            }
        }
    }


    private void SetFoundations()
    {
        GetComponentInChildren<Geography>().LayTheLand();
        GetComponentInChildren<Biosphere>().Eden();
        GetComponentInChildren<Civilization>().DawnOfMhoddim();
    }


    private void SetBounds()
    {
        // using a dictionary instead of list or array to ensure accurate lookup by edge name (e.g. "north")

        boundaries[Cardinal.North] = geography.GetBorder(Cardinal.North);
        boundaries[Cardinal.East] = geography.GetBorder(Cardinal.East);
        boundaries[Cardinal.South] = geography.GetBorder(Cardinal.South);
        boundaries[Cardinal.West] = geography.GetBorder(Cardinal.West);

        Vector3[] sky = new Vector3[2];
        sky[0] = boundaries[Cardinal.North][0];
        sky[1] = boundaries[Cardinal.South][0];
        boundaries[Cardinal.Sky] = sky;

        AddDirectionBoundaries();
    }


    private void SetComponents()
    {
        geography = GetComponentInChildren<Geography>();
        terrain = GetComponentInChildren<Terrain>();
    }


    void SetHeavenAndEarth()
    {
        HeavenAndEarth.earth = new Plane(Vector3.up, 0f);
        HeavenAndEarth.heaven = new Plane(Vector3.down, sky_height * 5);
    }
}