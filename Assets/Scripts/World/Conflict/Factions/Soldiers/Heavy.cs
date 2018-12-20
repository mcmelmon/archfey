using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heavy : MonoBehaviour {

    public float perception_range = 20f;
    public int perception_rating = 0;
    public float speed = 1.5f;

    Actor actor;
    Movement movement;
    Senses senses;


    // Unity


    private void Start()
    {
        SetComponents();
        SetStats();
        Strategize();
    }


    // public


    public void Restrategize()
    {

    }


    public void Strategize()
    {

    }


    // private


    private void SetComponents()
    {
        actor = GetComponent<Actor>();
        movement = GetComponent<Movement>();
        movement.Agent.speed = speed;
        senses = GetComponent<Senses>();
        senses.perception_rating = perception_rating;
        senses.SetRange(perception_range);
    }


    private void SetStats()
    {
        // can't do in Actor until the Heavy component has been attached

        if (actor.Ghaddim != null) {
            actor.Ghaddim.SetStats();
        } else if (actor.Mhoddim != null) {
            actor.Mhoddim.SetStats();
        }
    }
}
