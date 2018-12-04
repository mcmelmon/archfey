using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

    public Dictionary<string, Vector3[]> boundaries = new Dictionary<string, Vector3[]>();

    Geography geography;
    Terrain terrain;
    readonly int sky_height = 100;


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
        GameObject bounds = new GameObject();
        bounds.transform.parent = transform;
        bounds.name = "Bounds";

        foreach (KeyValuePair <string, Vector3[]> keyValue in boundaries)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.parent = bounds.transform;
            Vector3 heading = keyValue.Value[1] - keyValue.Value[0];
            wall.transform.localScale = new Vector3(heading.magnitude, heading.magnitude, 1);
            wall.transform.gameObject.GetComponentInChildren<Renderer>().enabled = false;
            wall.transform.up = heading;

            if (keyValue.Key == "east" || keyValue.Key == "west") wall.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 90));

            if (keyValue.Key == "sky") {
                wall.transform.position = new Vector3(terrain.terrainData.heightmapResolution / 2, sky_height, terrain.terrainData.heightmapResolution / 2);
                wall.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 90));
            } else {
                wall.transform.position = keyValue.Value[0] + heading / 2;
            }
        }
    }


    void SetBounds()
    {
        // using a dictionary instead of list or array to ensure accurate lookup by edge name (e.g. "north")

        boundaries["north"] = geography.GetBorder("north");
        boundaries["east"] = geography.GetBorder("east");
        boundaries["south"] = geography.GetBorder("south");
        boundaries["west"] = geography.GetBorder("west");

        Vector3[] sky = new Vector3[2];
        sky[0] = boundaries["north"][0];
        sky[1] = boundaries["south"][0];
        boundaries["sky"] = sky;

        AddDirectionBoundaries();
    }


    void SetHeavenAndEarth()
    {
        HeavenAndEarth.earth = new Plane(Vector3.up, 0f);
        HeavenAndEarth.heaven = new Plane(Vector3.down, sky_height * 5);
    }
}