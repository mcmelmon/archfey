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


    public Obstacle InstantiateScaledObstacle(Vector3 position, Geography geography)
    {
        Obstacle _obstacle = Instantiate(this, position, transform.rotation, geography.transform);
        _obstacle.transform.localScale = new Vector3(Random.Range(1,4), Random.Range(1,4), Random.Range(1,4));
        return _obstacle;
    }
}