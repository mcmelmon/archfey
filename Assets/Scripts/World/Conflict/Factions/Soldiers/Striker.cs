using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Striker : MonoBehaviour {

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
        PerceptionRange = 20f;
        PerceptionRating = 15;
        Speed = 2f;
        StealthPersistence = 0;
        StealthRating = 25;
    
        Actor = GetComponent<Actor>();
        Actor.RuinControlRating = 20;
        Actor.Movement.Agent.speed = Speed;
        Actor.Senses.PerceptionRating = PerceptionRating;
        Actor.Senses.SetRange(PerceptionRange);
        Actor.Stealth = gameObject.AddComponent<Stealth>();
        Actor.Stealth.stealth_rating = StealthRating;
        Actor.Stealth.stealh_persistence = StealthPersistence;
    }


    private void SetStats()
    {
        // we can't do this in Actor until the Striker component has been attached

        if (Actor.Ghaddim != null) {
            Actor.Ghaddim.SetStats();
        } else if (Actor.Mhoddim != null) {
            Actor.Mhoddim.SetStats();
        }
    }
}