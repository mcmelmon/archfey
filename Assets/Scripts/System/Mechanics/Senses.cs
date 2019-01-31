using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Senses : MonoBehaviour
{

    // properties

    public Actor Me { get; set; }
    public float Darkvision { get; set; }
    public float PerceptionRange { get; set; }
    public List<Actor> Actors { get; set; }
    public List<Structure> Structures { get; set; }


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
        Actors.Clear();
        Structures.Clear();

        Collider[] colliders = Physics.OverlapSphere(transform.position, PerceptionRange);

        for (int i = 0; i < colliders.Length; i++) {
            Actor _actor = colliders[i].gameObject.GetComponent<Actor>();
            Structure _structure = colliders[i].gameObject.GetComponent<Structure>();

            if (_actor != null && _actor != Me) {
                Stealth sighting_stealth = _actor.GetComponent<Stealth>();

                if (sighting_stealth == null || sighting_stealth.SpottedBy(Me) && !Actors.Contains(_actor)) {
                    Actors.Add(_actor);
                }
            }

            if (_structure != null && !Structures.Contains(_structure)) {
                Structures.Add(_structure);
            }
        }

        Me.Actions.Attack.SetEnemyRanges();
    }


    // private

    private void SetComponents()
    {
        Actors = new List<Actor>();
        Me = GetComponent<Actor>();
        Structures = new List<Structure>();

        transform.gameObject.AddComponent<SphereCollider>();
        GetComponent<SphereCollider>().isTrigger = true;
    }
}