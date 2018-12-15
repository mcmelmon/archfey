using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Striker : MonoBehaviour {

    public float perception_range = 20f;
    public float perception_rating = 0.15f;
    public float speed = 2f;
    public float stealth_persistence = 0.1f;
    public float stealth_rating = 0.25f;

    Actor actor;
    Geography geography;
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
        actor.SetComponents();
        geography = GetComponentInParent<World>().GetComponentInChildren<Geography>();
        movement = GetComponent<Movement>();
        movement.GetAgent().speed = speed;
        senses = GetComponent<Senses>();
        senses.perception_rating = perception_rating;
        senses.SetRange(perception_range);
        //stealth = gameObject.AddComponent<Stealth>();
        //stealth.stealth_rating = stealth_rating;
        //stealth.stealh_persistence = stealth_persistence;
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