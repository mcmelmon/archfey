using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Geography : MonoBehaviour {

    public Obstacle obstacle_prefab;
    public List<Obstacle> obstacles = new List<Obstacle>();
    public float obstacle_coverage;

    Map map;
    Terrain terrain;
    TerrainData terrain_data;

    // Unity

    void Awake () {
        map = GetComponentInParent<Map>();
        terrain = GetComponentInChildren<Terrain>();
        terrain_data = terrain.terrainData;
        PlaceObstacles();
    }


    // public


    public Dictionary<Map.Cardinal, float> DistanceToEdges(Vector3 _from)
    {
        Dictionary<Map.Cardinal, float> distances = new Dictionary<Map.Cardinal, float>();

        foreach (KeyValuePair<Map.Cardinal, Vector3[]> keyValue in map.boundaries)
        {
            float distance = HandleUtility.DistancePointLine(_from, keyValue.Value[1], keyValue.Value[0]);
            distances[keyValue.Key] = distance;
        }

        return distances;
    }


    public Vector3 TowardLocation(Vector3 _from, Vector3 _to)
    {
        return _to - _from;
    }


    public Vector3[] GetBorder(Map.Cardinal cardinal)
    {
        if (terrain == null) {
            terrain = GetComponentInChildren<Terrain>();
            terrain_data = terrain.terrainData;
        }

        Vector3[] border = new Vector3[2];
        float resolution = terrain.terrainData.heightmapResolution;

        switch (cardinal){
            case Map.Cardinal.North:
                border[0] = new Vector3(0, 0, resolution);
                border[1] = new Vector3(resolution, 0, resolution);
                break;
            case Map.Cardinal.East:
                border[0] = new Vector3(resolution, 0, resolution); ;
                border[1] = new Vector3(resolution, 0, 0);
                break;
            case Map.Cardinal.South:
                border[0] = new Vector3(resolution, 0, 0);
                border[1] = new Vector3(0, 0, 0);
                break;
            case Map.Cardinal.West:
                border[0] = new Vector3(0, 0, 0);
                border[1] = new Vector3(0, 0, resolution);
                break;
            default:
                border[0] = Vector3.zero;
                border[1] = Vector3.zero;
                break;
        }

        return border;
    }


    public Vector3 GetCenter()
    {
        if (terrain == null)
        {
            terrain = GetComponentInChildren<Terrain>();
            terrain_data = terrain.terrainData;
        }
        return new Vector3(terrain_data.heightmapResolution / 2, 0, terrain_data.heightmapResolution / 2);  // TODO: sample height
    }


    public int GetResolution()
    {
        return terrain_data.heightmapResolution;
    }


    public Terrain GetOrCreateTerrain()
    {
        return terrain ?? GetComponentInChildren<Terrain>();
    }


    public Vector3 PointBetween(Vector3 _from, Vector3 _to, float step_percentage, bool grounded)
    {
        Vector3 heading = TowardLocation(_from, _to);
        if (grounded) heading.y = 0;
        float distance = heading.magnitude * step_percentage;
        return distance * Vector3.Normalize(_to - _from) + _from;
    }


    public Vector3 RandomBorderLocation()
    {
        Vector3 point = Vector3.zero;

        switch (Random.Range(0,4))
        {
            // TODO: use sampled height
            case 0:
                point = new Vector3(Random.Range(0, terrain_data.heightmapResolution), 0, terrain_data.heightmapResolution); 
                break;
            case 1:
                point = new Vector3(terrain_data.heightmapResolution, 0, Random.Range(0, terrain_data.heightmapResolution));
                break;
            case 2:
                point = new Vector3(Random.Range(0, terrain_data.heightmapResolution), 0, 0);
                break;
            case 3:
                point = new Vector3(0, 0, Random.Range(0, terrain_data.heightmapResolution));
                break;
        }

        return point;
    }


    public Vector3 RandomLocation()
    {
        int _w = Random.Range(0, terrain_data.heightmapResolution);
        int _d = Random.Range(0, terrain_data.heightmapResolution);
        float _h = terrain.SampleHeight(new Vector3(_d, 0, _w));

        return new Vector3(_w, _h, _d);
    }


    public Vector3 RandomLocation(float distance_from_edge)
    {
        Vector3 point = Vector3.zero;
        Circle extent = Circle.CreateCircle(GetCenter(), (terrain_data.heightmapResolution / 2) - distance_from_edge);
        return extent.RandomContainedPoint();
    }


    // private


    private void OnValidate()
    {
        if (obstacle_coverage < 0f) obstacle_coverage = 0f;
        if (obstacle_coverage > 100f) obstacle_coverage = 100f;
    }


    private void PlaceObstacles()
    {
        int number_of_obstacles = Mathf.RoundToInt(terrain_data.heightmapResolution * (obstacle_coverage / 100f));
        GameObject obstacles_parent = new GameObject();
        obstacles_parent.name = "Obstacles";
        obstacles_parent.transform.parent = transform;

        for (int i = 0; i < number_of_obstacles; i++)
        {
            Obstacle _obstacle = obstacle_prefab.InstantiateScaledObstacle(RandomLocation(), obstacles_parent.transform);
            if (_obstacle != null) obstacles.Add(_obstacle);
        }
    }
}