using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ent : MonoBehaviour {

    public float speed = 2f;
    public float sense_radius = 40f;
    public float sense_perception = 30f;
    Actor actor;
    Senses senses;


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
        actor.SetComponents();
        senses = GetComponent<Senses>();
        senses.SetRange(sense_radius / transform.localScale.y);  // radius inflated by scale, and y is the biggest scale for an Ent
        senses.SetPerception(sense_perception);
    }


    private void SetStats()
    {
        actor.fey.SetStats();
    }
}
