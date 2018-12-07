using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    // TODO: Tile no longer builds the map, but should define an "actable area"

    public Dictionary<string, int> coordinates = new Dictionary<string, int>();
    public Ruin ruin;
    public Actor occupier;
    public List<Obstacle> obstacles = new List<Obstacle>();
    public List<Tree> trees = new List<Tree>();

    Geography geography;


    // Unity


    void Awake ()
    {
        geography = transform.GetComponentInParent<Geography>();
	}


    // public


    public void AddObstacle(Obstacle _obstacle)
    {
        _obstacle.GetTile().RemoveObstacle(_obstacle);
        _obstacle.transform.position = transform.position + new Vector3(0, (obstacles.Count + 1), 0);
        obstacles.Add(_obstacle);
    }


    public Dictionary<string, Tile> GetNeighbors()
    {
        // TODO: return near objects
        Dictionary<string, Tile> neighbors = new Dictionary<string, Tile>();

        return neighbors;
    }


    public Tile InstantiateScaledTile(int w, int h, int d, int scale, Geography geography)
    {
        Vector3 location = new Vector3(w * scale, h, d * scale);
        Tile _tile = Instantiate(this, location, transform.rotation, geography.transform);
        _tile.transform.localScale = new Vector3(scale, 1, scale);
        _tile.coordinates["w"] = w; 
        _tile.coordinates["h"] = h;
        _tile.coordinates["d"] = d;

        return _tile;
    }


    public void RemoveObstacle(Obstacle _obstacle)
    {
        if (obstacles.Contains(_obstacle)) obstacles.Remove(_obstacle);
    }
}