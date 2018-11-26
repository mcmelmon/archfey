using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raven : MonoBehaviour {

    float speed = 10f;
    public Camera eye;

    float distance_to_ground;
    float distance_to_sky;

	void Start () {
	}
	
	void Update () {
        if (Input.GetMouseButton(0))
        {
            Vector3 click_position = -Vector3.one;
            Ray ray = eye.ScreenPointToRay(Input.mousePosition);

            if (Map.HeavenAndEarth.earth.Raycast(ray, out distance_to_ground)) {
                click_position = ray.GetPoint(distance_to_ground);
            } else if (Map.HeavenAndEarth.heaven.Raycast(ray, out distance_to_sky)) {
                click_position = ray.GetPoint(distance_to_sky);
            }

            Vector3 direction = click_position - transform.position;
            transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
        }
    }
}