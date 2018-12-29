using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heavy : MonoBehaviour {

    // properties

    public Actor Actor { get; set; }
    public float PerceptionRange { get; set; }
    public int PerceptionRating { get; set; }
    public float Speed { get; set; }


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
        PerceptionRating = 10;
        Speed = 1.5f;

        Actor = GetComponent<Actor>();
        Actor.RuinControlRating = 20;
        Actor.Movement.Agent.speed = Speed;
        Actor.Senses.PerceptionRating = PerceptionRating;
        Actor.Senses.SetRange(PerceptionRange);

        // TODO: configure caster/non-caster better
        if (Actor.Faction == Conflict.Faction.Mhoddim)
        {
            Actor.Resources.gameObject.AddComponent<Smite>();
            Actor.Resources.IsCaster = true;
        } 
        else 
        {
            Actor.Resources.CurrentMana = 0;
            Actor.Resources.mana_pool_maximum = 0;
        }
    }


    private void SetStats()
    {
        // can't do in Actor until the Heavy component has been attached

        if (Actor.Ghaddim != null) {
            Actor.Ghaddim.SetStats();
        } else if (Actor.Mhoddim != null) {
            Actor.Mhoddim.SetStats();
        }
    }
}
