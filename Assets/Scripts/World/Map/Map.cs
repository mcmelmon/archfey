using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

    public enum Cardinal { North = 0, East = 1, South = 2, West = 3, Sky = 4 };

    readonly int sky_height = 100;

    // properties

    public Dictionary<Cardinal, Vector3[]> Boundaries { get; set; }
    public static Map Instance { get; set; }
    public static Terrain Terrain { get; set; }


    public struct HeavenAndEarth
    {
        public static Plane earth;
        public static Plane heaven;
    }


    // Unity


    private void Awake ()
    {
        if (Instance != null){
            Debug.LogError("More than one map instance!");
            Destroy(this);
            return;
        }
        Instance = this;
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

        foreach (KeyValuePair <Cardinal, Vector3[]> boundary in Boundaries)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.parent = bounds.transform;
            Vector3 heading = boundary.Value[1] - boundary.Value[0];
            wall.transform.localScale = new Vector3(heading.magnitude, heading.magnitude, 20);
            wall.transform.gameObject.GetComponentInChildren<Renderer>().enabled = false;
            wall.transform.up = heading;

            if (boundary.Key == Cardinal.East || boundary.Key == Cardinal.West) wall.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 90));

            if (boundary.Key == Cardinal.Sky) {
                wall.transform.position = new Vector3(Terrain.terrainData.heightmapResolution / 2, sky_height, Terrain.terrainData.heightmapResolution / 2);
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

        Boundaries[Cardinal.North] = Geography.Instance.GetBorder(Cardinal.North);
        Boundaries[Cardinal.East] = Geography.Instance.GetBorder(Cardinal.East);
        Boundaries[Cardinal.South] = Geography.Instance.GetBorder(Cardinal.South);
        Boundaries[Cardinal.West] = Geography.Instance.GetBorder(Cardinal.West);

        Vector3[] sky = new Vector3[2];
        sky[0] = Boundaries[Cardinal.North][0];
        sky[1] = Boundaries[Cardinal.South][0];
        Boundaries[Cardinal.Sky] = sky;

        AddDirectionBoundaries();
    }


    private void SetComponents()
    {
        Terrain = GetComponentInChildren<Terrain>();
        Boundaries = new Dictionary<Cardinal, Vector3[]>();
    }


    void SetHeavenAndEarth()
    {
        HeavenAndEarth.earth = new Plane(Vector3.up, 0f);
        HeavenAndEarth.heaven = new Plane(Vector3.down, sky_height * 5);
    }
}