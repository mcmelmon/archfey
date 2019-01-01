using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commoner : MonoBehaviour {

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
    }


    // private


    private void SetComponents()
    {
        PerceptionRange = 10f;
        PerceptionRating = 0;
        Speed = 1.5f;

        Actor = GetComponent<Actor>();
        Actor.Movement.Agent.speed = Speed;
        Actor.ObjectiveControlRating = 25;
        Actor.Resources.CurrentMana = 0;
        Actor.Resources.IsCaster = false;
        Actor.Resources.mana_pool_maximum = 0;
        Actor.Senses.Darkvision = 0f;
        Actor.Senses.PerceptionRating = PerceptionRating;
        Actor.Senses.SetRange(PerceptionRange);
    }


    private void SetStats()
    {
        // can't do in Actor until the Heavy component has been attached

        Actor.Mhoddim.SetStats();  // the Ghaddim do not have commoners
    }
}
