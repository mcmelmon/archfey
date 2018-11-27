using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Terrain : MonoBehaviour {
    public Tile tile;
    public Tile[,,] tiles;
    public Obstacle obstacle_prefab;
    public List<Obstacle> obstacles = new List<Obstacle>();
    public int width, height, depth, tile_scale;
    public int obstacle_coverage_percent;

    List<Tile> borders = new List<Tile>();
    List<Tile> interior = new List<Tile>();
    List<Tile> all = new List<Tile>();



    // Unity

    void Awake () {
        tiles = new Tile[width, height, depth];
        CreateTiles();
        CreateNavigationMesh();
        ListAllTiles();
        PlaceObstacles();
    }

    private void Start()
    {

    }


    void Update () {

	}


    // public


    public bool AllBordersOccupied()
    {
        foreach (var _tile in borders)
        {
            if (_tile.occupier == null) return false;
        }
        return true;
    }


    public List<Tile> GetAllTiles()
    {
        return all;
    }


    public Tile PickRandomEdgeTile()
    {
        return borders[Random.Range(0, borders.Count)];
    }


    public Tile PickRandomInteriorTile()
    {
        return interior[Random.Range(0, interior.Count)];
    }


    public Tile PickRandomTile()
    {
        return all[Random.Range(0, all.Count)];
    }


    // private


    private void OnValidate()
    {
        if (width < 4) width = 4;
        if (depth < 4) depth = 4;
        if (obstacle_coverage_percent < 0) obstacle_coverage_percent = 0;
        if (obstacle_coverage_percent > 100) obstacle_coverage_percent = 100;
    }


    private void CreateNavigationMesh()
    {
        transform.parent.Find("Navigation").gameObject.GetComponent<NavMeshSurface>().BuildNavMesh();
    }


    private void CreateTiles()
    {
        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                for (int d = 0; d < depth; d++)
                {
                    tiles[w, h, d] = tile.InstantiateScaledTile(w, h, d, tile_scale, this);
                }
            }
        }
    }


    private void ListAllTiles()
    {
        ListInterior();
        ListBorders();
        all.AddRange(interior);
        all.AddRange(borders);
    }


    private void ListBorders()
    {
        for (int w = 0; w < width; w++)
        {
            borders.Add(tiles[w, 0, 0]);
            borders.Add(tiles[w, 0, depth - 1]);
        }

        for (int d = 0; d < depth; d++)
        {
            borders.Add(tiles[width - 1, 0, d]);
            borders.Add(tiles[0, 0, d]);
        }
    }

    private void ListInterior()
    {
        for (int w = 1; w < width - 1; w++)
        {
            for (int h = 0; h < height; h++)
            {
                for (int d = 1; d < depth - 1; d++)
                {
                    interior.Add(tiles[w, h, d]);
                }
            }
        }
    }


    private void PlaceObstacles()
    {
        int number_of_obstacles = Mathf.RoundToInt(all.Count * (obstacle_coverage_percent /100f));
        for (int i = 0; i < number_of_obstacles; i++)
        {
            Tile _tile = PickRandomTile();
            Obstacle _obstacle = obstacle_prefab.InstantiateScaledObstacle(_tile);
            obstacles.Add(_obstacle);
        }
    }
}