using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Senses : MonoBehaviour {

    public float radius = 20f;
    public List<GameObject> sightings = new List<GameObject>();
    public float perception = 10f;


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
            Gizmos.DrawRay(transform.position, (sighting.transform.position - transform.position));
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Actor")
        {
            if (GetComponent<Defend>() == null && other.GetComponent<Defend>() != null)
            {
                RecordSighting(other.gameObject);
            }
        }
    }


    private void Update()
    {
        if (sightings.Count > 0) PruneSightings();
    }


    // public


    public void SetPerception(float _perception)
    {
        perception = _perception;
    }


    public void SetRange(float range)
    {
        GetComponent<SphereCollider>().radius = range;
    }


    // private


    public void RecordSighting(GameObject sighting)
    {
        if (!sightings.Contains(sighting)) sightings.Add(sighting);
    }


    private void PruneSightings()
    {
        List<int> prunings = new List<int>();

        foreach (var sighting in sightings)
        {
            if (Vector3.Distance(sighting.transform.position, transform.position) > 60f) {
                prunings.Add(sightings.IndexOf(sighting));
            }
        }

        foreach (var index in prunings)
        {
            if (index < sightings.Count && index >= 0) {
                sightings.RemoveAt(index);
            }
        }
    }
}