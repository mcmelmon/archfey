using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Geography : MonoBehaviour {

    public enum GridType { Unit = 0 };

    public static int unit_spacing = 8;

    public Obstacle obstacle_prefab;
    public float obstacle_coverage;


    // properties

    public static Dictionary<GridType, Grid> Grids { get; set; }
    public static Geography Instance { get; set; }
    public static List<Obstacle> Obstacles { get; set; }
    public static Terrain Terrain { get; set; }
    public static TerrainData TerrainData { get; set; }
    public static List<MapTile> Tiles { get; set; }


    // Unity

    private void Awake () {
        if (Instance != null){
            Debug.LogError("More than one geography instance!");
            Destroy(this);
            return;
        }
        Instance = this;
        SetComponents();
    }


    private void OnValidate()
    {
        if (obstacle_coverage < 0f) obstacle_coverage = 0f;
        if (obstacle_coverage > 100f) obstacle_coverage = 100f;
    }


    // public


    public Dictionary<Map.Cardinal, float> DistanceToEdges(Vector3 _from)
    {
        Dictionary<Map.Cardinal, float> distances = new Dictionary<Map.Cardinal, float>();

        foreach (KeyValuePair<Map.Cardinal, Vector3[]> boundary in Map.Instance.Boundaries)
        {
            float distance = HandleUtility.DistancePointLine(_from, boundary.Value[1], boundary.Value[0]);
            distances[boundary.Key] = distance;
        }

        return distances;
    }


    public Vector3[] GetBorder(Map.Cardinal cardinal)
    {
        Vector3[] border = new Vector3[2];
        float resolution = TerrainData.heightmapResolution;

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
        return new Vector3(TerrainData.heightmapResolution / 2, 0, TerrainData.heightmapResolution / 2);  // TODO: sample height
    }


    public int GetResolution()
    {
        return TerrainData.heightmapResolution;
    }


    public void LayTheLand()
    {
        // Obstacles cause unit placement trouble.  
        // TODO: redo this by modifying the terrain itself
        //PlaceObstacles();
    }


    public Vector3 PointBetween(Vector3 _from, Vector3 _to, float step_percentage, bool grounded)
    {
        // TODO: convert this to a grid point

        Vector3 heading = TowardLocation(_from, _to);
        if (grounded) heading.y = 0;
        float distance = heading.magnitude * step_percentage;
        return distance * Vector3.Normalize(_to - _from) + _from;
    }


    public Vector3 RandomBorderLocation(GridType grid_type = GridType.Unit)
    {
        switch (UnityEngine.Random.Range(0, 4))
        {
            case 0:
                return Grids[grid_type].RandomPoint(Map.Cardinal.North);
            case 1:
                return Grids[grid_type].RandomPoint(Map.Cardinal.East);
            case 2:
                return Grids[grid_type].RandomPoint(Map.Cardinal.South);
            case 3:
                return Grids[grid_type].RandomPoint(Map.Cardinal.West);
        }

        return Vector3.zero;
    }


    public Vector3 RandomLocation(GridType grid_type = GridType.Unit)
    {
        return Grids[grid_type].RandomPoint();
    }


    public Vector3 RandomLocation(int distance_from_edge, GridType grid_type = GridType.Unit)
    {
        return Grids[grid_type].RandomPoint(distance_from_edge);
    }


    public List<MapTile> RandomTiles(int _number)
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);

        List<MapTile> randomized_tiles = new List<MapTile>(Tiles);

        for (int i = 0; i < randomized_tiles.Count; i++) {
            MapTile temp = randomized_tiles[i];
            int random_index = UnityEngine.Random.Range(i, randomized_tiles.Count);
            randomized_tiles[i] = randomized_tiles[random_index];
            randomized_tiles[random_index] = temp;
        }

        return randomized_tiles.GetRange(0, _number);
    }


    public MapTile RandomUnoccupiedTile()
    {
        List<MapTile> randomized_tiles = RandomTiles(Tiles.Count);

        for (int i = 0; i < randomized_tiles.Count * 5; i++) {
            // If we can't find an empty tile after five times through them all, it's a full map
            if (randomized_tiles[i].Unoccupied()) return randomized_tiles[i];
        }

        return null;
    }


    public Vector3 TowardLocation(Vector3 _from, Vector3 _to)
    {
        return _to - _from;
    }


    // private


    private void PlaceObstacles()
    {
        int number_of_obstacles = Mathf.RoundToInt(TerrainData.heightmapResolution * (obstacle_coverage / 100f));
        GameObject obstacles_parent = new GameObject { name = "Obstacles" };
        obstacles_parent.transform.parent = transform;

        for (int i = 0; i < number_of_obstacles; i++)
        {
            Obstacle _obstacle = obstacle_prefab.InstantiateScaledObstacle(RandomUnoccupiedTile().Location, obstacles_parent.transform);
            if (_obstacle != null) Obstacles.Add(_obstacle);
        }
    }


    private void SetComponents()
    {
        Grids = new Dictionary<GridType, Grid>();
        Obstacles = new List<Obstacle>();
        Terrain = GetComponentInChildren<Terrain>();
        TerrainData = Terrain.terrainData;

        Grids[GridType.Unit] = Grid.New(new Vector3(1, 0, GetResolution() - 2), GetResolution() / unit_spacing, GetResolution() / unit_spacing, unit_spacing, false);
        Tiles = new List<MapTile>();

        foreach (var location in Grids[GridType.Unit].Elements)
        {
            // tiles will help manage the initial contents of locations on the map
            MapTile _tile = MapTile.New(location);
            Tiles.Add(_tile);
        }
    }
}