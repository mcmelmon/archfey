using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ent : MonoBehaviour {

    // properties

    public Actor Actor { get; set; }
    public float PerceptionRange { get; set; }
    public int PerceptionRating { get; set; }
    public float Speed { get; set; }
    public int StealthPersistence { get; set; }
    public int StealthRating { get; set; }


    // Unity


    private void Start()
    {
        SetComponents();
        SetStats();
        //Strategize();
    }


    // public

    public Ent SummonEnt(Vector3 _position, Transform _parent)
    {
        Ent _ent = Instantiate(this, _position, transform.rotation, _parent);
        return _ent;
    }


    // private

    private void SetComponents()
    {
        PerceptionRange = 30f;
        PerceptionRating = 75;
        StealthPersistence= 5;
        StealthRating = 35;

        Actor = GetComponent<Actor>();
        Actor.Senses.PerceptionRating = PerceptionRating;
        Actor.Senses.SetRange(PerceptionRange);
        Actor.Stealth = gameObject.AddComponent<Stealth>();
        Actor.Stealth.stealth_rating = StealthRating;
        Actor.Stealth.stealh_persistence = StealthPersistence;
    }


    private void SetStats()
    {
        Actor.Fey.SetStats();
    }
}
