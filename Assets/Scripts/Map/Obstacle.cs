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


    public Obstacle InstantiateScaledObstacle(Tile tile)
    {
        Vector3 height;

        height = tile.transform.position + new Vector3(0, (tile.obstacles.Count + 1), 0);

        Obstacle _obstacle = Instantiate(this, height, transform.rotation, tile.transform);

        if (_obstacle != null)
        {
            tile.obstacles.Add(_obstacle);
            return _obstacle;
        }

        return null;
    }
}