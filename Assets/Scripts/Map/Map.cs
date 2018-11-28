using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

    public List<Vector3[]> boundaries = new List<Vector3[]>();

    Geography geography;
    Terrain terrain;
    int sky_height = 100;


    public struct HeavenAndEarth
    {
        public static Plane earth;
        public static Plane heaven;
    }


    // Unity


    void Awake ()
    {
        geography = GetComponentInChildren<Geography>();
        terrain = GetComponentInChildren<Terrain>();
        SetHeavenAndEarth();
        SetBounds();
    }


    // public


    public Geography GetGeography()
    {
        return geography;
    }


    // private


    void AddDirectionBoundaries()
    {
        for (int i = 0; i < boundaries.Count; i++)
        {
            Vector3[] boundary = boundaries[i];
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.parent = transform;
            Vector3 heading = boundary[1] - boundary[0];
            wall.transform.localScale = new Vector3(heading.magnitude, heading.magnitude, 1);
            wall.transform.gameObject.GetComponentInChildren<Renderer>().enabled = false;
            wall.transform.up = heading;
            if (i == 1 || i == 3) wall.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 90));
            if (i == boundaries.Count - 1) {
                wall.transform.position = new Vector3(terrain.terrainData.heightmapResolution / 2, sky_height, terrain.terrainData.heightmapResolution / 2);
                wall.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 90));
            } else {
                wall.transform.position = boundary[0] + heading / 2;
            }
        }
    }


    void SetBounds()
    {
        float resolution = terrain.terrainData.heightmapResolution;
        Vector3[] north = new Vector3[2], east = new Vector3[2], south = new Vector3[2], west = new Vector3[2], sky = new Vector3[2];

        north[0]    = new Vector3(0, 0, resolution);
        north[1]    = new Vector3(resolution, 0, resolution);
        boundaries.Add(north);

        east[0]     = north[1];
        east[1]     = new Vector3(resolution, 0, 0);
        boundaries.Add(east);

        south[0]    = east[1];
        south[1]    = new Vector3(0, 0, 0);
        boundaries.Add(south);

        west[0]     = south[1];
        west[1]     = north[0];
        boundaries.Add(west);

        sky[0] = north[0];
        sky[1] = south[0];
        boundaries.Add(sky);

        AddDirectionBoundaries();
    }


    void SetHeavenAndEarth()
    {
        HeavenAndEarth.earth = new Plane(Vector3.up, 0f);
        HeavenAndEarth.heaven = new Plane(Vector3.down, sky_height * 5);
    }
}