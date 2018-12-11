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
            if (sighting == null) continue;
            Gizmos.DrawRay(transform.position, (sighting.transform.position - transform.position));
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Actor")
        {
            if (GetComponent<Defender>() == null && other.GetComponent<Defender>() != null)
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

        // NOTE: A sighted object may be destroyed in combat after we have sighted it

        foreach (var sighting in sightings)
        {
            if (sighting == null || Vector3.Distance(sighting.transform.position, transform.position) > radius) {
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