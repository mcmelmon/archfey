using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Senses : MonoBehaviour {

    public float radius = 20f;
    public List<GameObject> sightings = new List<GameObject>();


    // Unity


    private void Awake()
    {
        transform.gameObject.AddComponent<SphereCollider>();
    }


    private void Start()
    {
        GetComponent<SphereCollider>().radius = radius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Actor" ) 
        {
            RecordSighting(other.gameObject);
        }
    }


    // private


    private void RecordSighting(GameObject sighting)
    {
        if (!sightings.Contains(sighting)) {
            sightings.Add(sighting);
        }
    }
}