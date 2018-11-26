using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

    public int sky_height;

    Terrain terrain;
    Installations installations;
    
    public struct HeavenAndEarth
    {
        public static Plane earth;
        public static Plane heaven;
    }


    // Unity


    void Awake () 
    {
        terrain = transform.Find("Terrain").gameObject.GetComponent<Terrain>();
        installations = transform.Find("Civilization").gameObject.transform.Find("Installations").gameObject.GetComponent<Installations>();
        SetHeavenAndEarth();
        SetBounds();
    }

    private void Start()
    {

    }


    void Update ()
    {
		
	}


    // public


    public Vector3 GetCenter()
    {
        Vector3 center = new Vector3(terrain.width * terrain.tile_scale / 2, 0, terrain.depth * terrain.tile_scale / 2);
        return center;
    }


    public Installations GetInstallations()
    {
        return installations;
    }


    public Terrain GetTerrain()
    {
        return terrain;
    }


    // private

    private void OnValidate()
    {
        if (sky_height < 30) sky_height = 30;
    }


    void AddDirectionBoundary(string direction, Transform boundaries)
    {
        GameObject boundary = GameObject.CreatePrimitive(PrimitiveType.Cube);
        boundary.transform.parent = boundaries.transform;
        boundary.transform.localScale = new Vector3((terrain.width * terrain.tile_scale) + 2, 2, terrain.depth * terrain.tile_scale);
        boundary.name = direction;

        MeshRenderer plane_renderer = boundary.GetComponent<MeshRenderer>();
        plane_renderer.enabled = false;

        switch (direction){
            case "east":
                boundary.transform.position = new Vector3(terrain.width * terrain.tile_scale, 0, (terrain.depth * terrain.tile_scale / 2) - terrain.tile_scale / 2);
                boundary.transform.rotation = Quaternion.LookRotation(new Vector3(-1, 90, 0));
                break;
            case "west":
                boundary.transform.position = new Vector3(-terrain.tile_scale / 2, 0, (terrain.depth * terrain.tile_scale / 2) - terrain.tile_scale / 2);
                boundary.transform.rotation = Quaternion.LookRotation(new Vector3(-1, 90, 0));
                break;
            case "north":
                boundary.transform.position = new Vector3(terrain.width * terrain.tile_scale / 2, 0, (terrain.depth * terrain.tile_scale) - terrain.tile_scale / 2);
                boundary.transform.rotation = Quaternion.LookRotation(new Vector3(0, 90, 0));
                break;
            case "south":
                boundary.transform.position = new Vector3(terrain.width * terrain.tile_scale / 2, 0, -terrain.tile_scale / 2);
                boundary.transform.rotation = Quaternion.LookRotation(new Vector3(0, 90, 0));
                break;
            case "sky":
                boundary.transform.position = new Vector3((terrain.depth * terrain.tile_scale / 2) - terrain.tile_scale / 2, sky_height, (terrain.depth * terrain.tile_scale / 2) - terrain.tile_scale / 2);
                break;
            case "ground":
                boundary.transform.position = new Vector3((terrain.depth * terrain.tile_scale / 2) - terrain.tile_scale / 2, 0, (terrain.depth * terrain.tile_scale / 2) - terrain.tile_scale / 2);
                break;
            default:
                break;
        }
    }


    void SetBounds()
    {
        List<string> directions = new List<string>();
        GameObject boundaries = new GameObject("Boundaries");
        boundaries.transform.parent = this.transform;

        directions.Add("east");
        directions.Add("west");
        directions.Add("south");
        directions.Add("north");
        directions.Add("sky");
        directions.Add("ground");

        foreach (var direction in directions)
        {
            AddDirectionBoundary(direction, boundaries.transform);
        }
    }


    void SetHeavenAndEarth() 
    {
        HeavenAndEarth.earth = new Plane(Vector3.up, 0f);
        HeavenAndEarth.heaven = new Plane(Vector3.down, sky_height * 5);
    }
}