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
        foreach (var sighting in sightings)
        {
            if (sighting == null) continue;
            Gizmos.DrawRay(transform.position, (sighting.transform.position - transform.position));
        }
    }


    private void Start()
    {
        StartCoroutine(Sight());
    }


    // public


    public List<GameObject> GetSightings()
    {
        return sightings;
    }

    public void SetRange(float _range)
    {
        GetComponent<SphereCollider>().radius = radius = _range;
    }


    // private


    private IEnumerator Sight()
    {
        while (true) {
            sightings.Clear();

            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

            for (int i = 0; i < colliders.Length; i++) {
                GameObject sighting = colliders[i].gameObject;

                if (sighting.tag == "Actor" && sighting != gameObject) {  // don't sight ourselves
                    Stealth _stealth = sighting.GetComponent<Stealth>();

                    if (_stealth == null || _stealth.Spotted(perception_rating) && !sightings.Contains(sighting)) {
                        sightings.Add(colliders[i].gameObject);
                    }
                }
            }

            yield return null;
        }
    }
}