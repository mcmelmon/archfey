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
        _obstacle.transform.localScale = new Vector3(Random.Range(3,12), Random.Range(1,9), Random.Range(3,12));
        _obstacle.transform.rotation = Quaternion.Euler(new Vector3(Random.Range(0,45), Random.Range(0, 45), Random.Range(0, 45)));
        _obstacle.transform.position += new Vector3(0, _obstacle.transform.localScale.y / 4, 0);
        return _obstacle;
    }
}