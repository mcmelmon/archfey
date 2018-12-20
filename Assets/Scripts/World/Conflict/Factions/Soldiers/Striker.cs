using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Striker : MonoBehaviour {

    public float perception_range = 20f;
    public int perception_rating = 15;
    public float speed = 2f;
    public int stealth_persistence = 0;
    public int stealth_rating = 25;

    Actor actor;
    Movement movement;
    Senses senses;
    Stealth stealth;


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
        stealth = gameObject.AddComponent<Stealth>();
        stealth.stealth_rating = stealth_rating;
        stealth.stealh_persistence = stealth_persistence;
        actor.Stealth = stealth;
    }


    private void SetStats()
    {
        // can't do in Actor until the Striker component has been attached

        if (actor.Ghaddim != null) {
            actor.Ghaddim.SetStats();
        } else if (actor.Mhoddim != null) {
            actor.Mhoddim.SetStats();
        }
    }
}