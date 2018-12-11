using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Senses : MonoBehaviour {

    public float radius = 20f;
    public float perception = 10f;
    public List<GameObject> sightings = new List<GameObject>();


    // Unity


    private void Awake()
    {
        transform.gameObject.AddComponent<SphereCollider>();
        GetComponent<SphereCollider>().radius = radius;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (var sighting in sightings)
        {
            if (sighting == null) continue;
            Gizmos.DrawRay(transform.position, (sighting.transform.position - transform.position));
        }
    }


    private void Update()
    {

    }


    // public


    public List<GameObject> GetSightings()
    {
        sightings.Clear();

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        for (int i = 0; i < colliders.Length; i++) {
            if (colliders[i].gameObject.tag == "Actor") sightings.Add(colliders[i].gameObject);
        }

        return sightings;
    }


    public void SetPerception(float _perception)
    {
        perception = _perception;
    }


    public void SetRange(float range)
    {
        GetComponent<SphereCollider>().radius = range;
    }
}