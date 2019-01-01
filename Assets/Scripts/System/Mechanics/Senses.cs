using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Senses : MonoBehaviour {

    // properties

    public Actor Actor { get; set; }
    public float Darkvision { get; set; }
    public int PerceptionRating { get; set; }
    public float PerceptionRange { get; set; }
    public List<Actor> Sightings { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


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
            Actor _sighting = colliders[i].gameObject.GetComponent<Actor>();

            if (_sighting != null && _sighting != GetComponent<Actor>()) {  // don't sight ourselves
                Stealth sighting_stealth = _sighting.GetComponent<Stealth>();

                if (sighting_stealth == null || sighting_stealth.Spotted(Actor, PerceptionRating) && !Sightings.Contains(_sighting)) {
                    Sightings.Add(_sighting);
                }
            }
        }
    }


    // private

    private void SetComponents()
    {
        Actor = GetComponent<Actor>();
        transform.gameObject.AddComponent<SphereCollider>();
        GetComponent<SphereCollider>().isTrigger = true;
        Sightings = new List<Actor>();
    }
}