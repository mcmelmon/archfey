using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Senses : MonoBehaviour {

    // properties

    public int PerceptionRating { get; set; }
    public float PerceptionRange { get; set; }
    public List<GameObject> Sightings { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    foreach (var sighting in Sightings) {
    //        if (sighting == null) continue;
    //        Gizmos.DrawRay(transform.position, (sighting.transform.position - transform.position));
    //    }
    //}


    // public


    public void SetRange(float _range)
    {
        GetComponent<SphereCollider>().radius = PerceptionRange = _range;
    }


    public void Sight()
    {
        Sightings.Clear();

        Collider[] colliders = Physics.OverlapSphere(transform.position, PerceptionRange);

        for (int i = 0; i < colliders.Length; i++) {
            GameObject sighting = colliders[i].gameObject;

            if (sighting.tag == "Actor" && sighting != gameObject && sighting != null) {  // don't sight ourselves
                Stealth sighting_stealth = sighting.GetComponent<Stealth>();

                if (sighting_stealth == null || sighting_stealth.Spotted(gameObject, PerceptionRating) && !Sightings.Contains(sighting)) {
                    Sightings.Add(colliders[i].gameObject);
                }
            }
        }
    }


    // private

    private void SetComponents()
    {
        transform.gameObject.AddComponent<SphereCollider>();
        GetComponent<SphereCollider>().isTrigger = true;
        Sightings = new List<GameObject>();
    }
}