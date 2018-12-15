using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Senses : MonoBehaviour {

    public float perception_rating;
    public float radius;

    List<GameObject> sightings = new List<GameObject>();


    // Unity


    private void Awake()
    {
        transform.gameObject.AddComponent<SphereCollider>();
        GetComponent<SphereCollider>().isTrigger = true;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (var sighting in GetSightings())
        {
            if (sighting == null) continue;
            Gizmos.DrawRay(transform.position, (sighting.transform.position - transform.position));
        }
    }


    // public


    public List<GameObject> GetSightings()
    {
        sightings.Clear();

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        for (int i = 0; i < colliders.Length; i++) {
            if (colliders[i].gameObject.tag == "Actor" && colliders[i].gameObject != gameObject) {  // don't sight yourself
                Stealth _stealth = colliders[i].GetComponent<Stealth>();
                if (_stealth == null || _stealth.Spotted(perception_rating)) {
                    sightings.Add(colliders[i].gameObject);
                }
            }
        }

        return sightings;
    }


    public void SetRange(float _range)
    {
        GetComponent<SphereCollider>().radius = radius = _range;
    }
}