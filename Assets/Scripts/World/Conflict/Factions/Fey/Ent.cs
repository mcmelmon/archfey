using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ent : MonoBehaviour {

    public float perception_range = 30f;
    public int perception_rating = 75;
    public float speed = 2f;
    public int stealth_persistence = 5;
    public int stealth_rating = 35;

    Actor actor;
    Senses senses;
    Stealth stealth;


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
        actor = GetComponent<Actor>();
        senses = GetComponent<Senses>();
        senses.SetRange(perception_range / transform.localScale.y);  // radius inflated by scale, and y is the biggest scale for an Ent
        senses.perception_rating = perception_rating;
        stealth = gameObject.AddComponent<Stealth>();
        stealth.stealth_rating = stealth_rating;
        stealth.stealh_persistence = stealth_persistence;
    }


    private void SetStats()
    {
        actor.fey.SetStats();
    }
}
