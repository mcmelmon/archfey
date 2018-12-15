using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heavy : MonoBehaviour {

    public float perception_range = 20f;
    public float perception_rating = 0.1f;
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
        actor.SetComponents();
        movement = GetComponent<Movement>();
        movement.GetAgent().speed = speed;
        senses = GetComponent<Senses>();
        senses.perception_rating = perception_rating;
        senses.SetRange(perception_range);
    }


    private void SetStats()
    {
        if (actor.ghaddim != null) {
            actor.ghaddim.SetStats();
        } else if (actor.mhoddim != null) {
            actor.mhoddim.SetStats();
        }
    }
}
