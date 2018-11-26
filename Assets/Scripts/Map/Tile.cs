using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public Installation installation;
    public Actor occupier;


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
        Tile _tile;
        Vector3 location = new Vector3(w * scale, h, d * scale);
        _tile = Instantiate(this, location, transform.rotation, terrain.transform);
        _tile.transform.localScale = new Vector3(scale, 1, scale);
        return _tile;
    }
}