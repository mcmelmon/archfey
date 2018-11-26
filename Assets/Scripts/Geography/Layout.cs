using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Layout : MonoBehaviour {
    public Tile tile;
    public Tile[,,] tiles;
    public int width, height, depth, tile_scale;
    public NavMeshSurface surface;

    List<Tile> borders = new List<Tile>();
    List<Tile> interior = new List<Tile>();


    // Unity

    void Awake () {
        tiles = new Tile[width, height, depth];
        CreateTiles();
        SetInterior();
        SetBorders();
        surface.BuildNavMesh();
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


    public void CreateTiles()
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


    public Tile PickRandomEdgeTile()
    {
        return borders[Random.Range(0, borders.Count)];
    }


    public Tile PickRandomInteriorTile()
    {
        return interior[Random.Range(0, interior.Count)];
    }


    // private


    private void OnValidate()
    {
        if (width < 4) width = 4;
        if (depth < 4) depth = 4;
    }


    private void SetBorders()
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

    private void SetInterior()
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
}
