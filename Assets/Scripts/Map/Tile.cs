using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public Installation installation;
    public Actor occupier;
    public List<Obstacle> obstacles = new List<Obstacle>();
    public List<Tree> trees = new List<Tree>();


    // Unity


    void Awake ()
    {

	}


	void Update ()
    {

	}


    // public


    public Tile InstantiateScaledTile(int w, int h, int d, int scale, Terrain terrain)
    {
        Vector3 location = new Vector3(w * scale, h, d * scale);
        Tile _tile = Instantiate(this, location, transform.rotation, terrain.transform);
        _tile.transform.localScale = new Vector3(scale, 1, scale);
        return _tile;
    }
}