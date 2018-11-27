using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Geography : MonoBehaviour {

    public Obstacle obstacle_prefab;
    public List<Obstacle> obstacles = new List<Obstacle>();
    public float obstacle_coverage;

    Map map;
    Terrain terrain;
    TerrainData terrain_data;

    // Unity

    void Awake () {
        map = transform.GetComponentInParent<Map>();
        terrain = transform.GetComponentInChildren<Terrain>();
        terrain_data = terrain.terrainData;
        CreateNavigationMesh();
        PlaceObstacles();
    }


    private void Start()
    {

    }


    // public


    public Vector3 GetCenter()
    {
        return new Vector3(terrain_data.heightmapResolution / 2, 0, terrain_data.heightmapResolution / 2);  // TODO: sample height
    }


    public int GetResolution()
    {
        return terrain_data.heightmapResolution;
    }


    public Vector3 RandomBorderLocation()
    {
        Vector3 ne = new Vector3(terrain_data.heightmapResolution, 0, terrain_data.heightmapResolution);
        Vector3 se = new Vector3(terrain_data.heightmapResolution, 0, 0);
        Vector3 sw = Vector3.zero;
        Vector3 nw = new Vector3(0, 0, terrain_data.heightmapResolution);
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
        // TODO: use sampled height
        return new Vector3(Random.Range(0, terrain_data.heightmapResolution), 0, Random.Range(0, terrain_data.heightmapResolution));
    }


    // private


    private void OnValidate()
    {
        if (obstacle_coverage < 0) obstacle_coverage = 0;
        if (obstacle_coverage > 100) obstacle_coverage = 100;
    }


    private bool AdjustObstacle(Dictionary<string, Tile> neighbors, Obstacle _obstacle)
    {
        foreach (var neighbor in neighbors) {
            if (neighbor.Value.obstacles.Count > 0)
            {
                neighbor.Value.AddObstacle(_obstacle);
                return true;
            }

        }

        return false;
    }


    private void CreateNavigationMesh()
    {
        transform.parent.Find("Navigation").gameObject.GetComponent<NavMeshSurface>().BuildNavMesh();  // TODO: use GetComponentInChilren instead of Find
    }



    private void PlaceObstacles()
    {
        int number_of_obstacles = Mathf.RoundToInt(terrain_data.heightmapResolution * (obstacle_coverage / 100f));

        for (int i = 0; i < number_of_obstacles; i++)
        {
            int _w = Random.Range(0, terrain_data.heightmapResolution);
            int _d = Random.Range(0, terrain_data.heightmapResolution);
            float _h = terrain.SampleHeight(new Vector3(_d, 0, _w));
            Obstacle _obstacle = obstacle_prefab.InstantiateScaledObstacle(_w, _h, _d, terrain);
            if (_obstacle != null) obstacles.Add(_obstacle);
        }
    }
}