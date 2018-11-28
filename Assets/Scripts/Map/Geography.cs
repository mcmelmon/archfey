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
        map = GetComponentInParent<Map>();
        terrain = GetComponentInChildren<Terrain>();
        terrain_data = terrain.terrainData;
        CreateNavigationMesh();
        PlaceObstacles();
    }


    private void Start()
    {

    }


    // public


    public Vector3 FaceLocation(Vector3 _from, Vector3 _to)
    {
        return _to - _from;
    }



    public Vector3 GetCenter()
    {
        return new Vector3(terrain_data.heightmapResolution / 2, 0, terrain_data.heightmapResolution / 2);  // TODO: sample height
    }


    public int GetResolution()
    {
        return terrain_data.heightmapResolution;
    }

    public Terrain GetTerrain()
    {
        return terrain;
    }


    public bool PathToCenter(Vector3 from_here, int radius)
    {
        // TODO: take a circle, ensure that it has a path to "somewhere"
        return true;
    }


    public Vector3 PointBetween(Vector3 _from, Vector3 _to, float step_percentage, bool grounded)
    {
        Vector3 heading = FaceLocation(_from, _to);
        if (grounded) heading.y = 0;
        float distance = heading.magnitude * step_percentage;
        return distance * Vector3.Normalize(_to - _from) + _from;
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
        int _w = Random.Range(0, terrain_data.heightmapResolution);
        int _d = Random.Range(0, terrain_data.heightmapResolution);
        float _h = terrain.SampleHeight(new Vector3(_d, 0, _w));

        return new Vector3(_w, _h, _d);
    }


    // private


    private void OnValidate()
    {
        if (obstacle_coverage < 0f) obstacle_coverage = 0f;
        if (obstacle_coverage > 100f) obstacle_coverage = 100f;
    }


    private bool AdjustObstacle(Dictionary<string, Tile> neighbors, Obstacle _obstacle)
    {

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
            Obstacle _obstacle = obstacle_prefab.InstantiateScaledObstacle(RandomLocation(), this);
            if (_obstacle != null) obstacles.Add(_obstacle);
        }
    }
}