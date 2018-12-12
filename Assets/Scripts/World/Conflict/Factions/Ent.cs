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
        ConfigureRoleSpecificProperties();
        //Strategize();
    }


    // public

    public Ent SummonEnt(Vector3 _position, Transform _parent)
    {
        Ent _ent = Instantiate(this, _position, transform.rotation, _parent);
        return _ent;
    }


    // private

    private void ConfigureRoleSpecificProperties()
    {
        senses = GetComponent<Senses>();
        senses.SetRange(sense_radius);
        senses.SetPerception(sense_perception);
        actor = GetComponent<Actor>();
        actor.SetComponents();
        actor.SetStats();
    }
}
