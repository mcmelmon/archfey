using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

    public Obstacle InstantiateScaledObstacle(Vector3 position, Transform obstacles)
    {
        Obstacle _obstacle = Instantiate(this, position, transform.rotation, obstacles);
        _obstacle.transform.localScale = new Vector3(Random.Range(3,12), Random.Range(1,9), Random.Range(3,12));
        _obstacle.transform.rotation = Quaternion.Euler(new Vector3(Random.Range(0,45), Random.Range(0, 45), Random.Range(0, 45)));
        _obstacle.transform.position += new Vector3(0, _obstacle.transform.localScale.y / 4, 0);
        return _obstacle;
    }
}