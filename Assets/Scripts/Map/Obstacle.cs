using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

    // Unity


    void Awake()
    {

    }


    void Update()
    {

    }


    // public

    public Tile GetTile()
    {
        return transform.GetComponentInParent<Tile>();
    }


    public Obstacle InstantiateScaledObstacle(int _w, float _h, int _d, Terrain terrain)
    {
        Obstacle _obstacle = Instantiate(this, new Vector3(_w, _h, _d), transform.rotation, terrain.transform);
        _obstacle.transform.localScale = new Vector3(Random.Range(1,4), Random.Range(1,4), Random.Range(1,4));
        return _obstacle;
    }
}