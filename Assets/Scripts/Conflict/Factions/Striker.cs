using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Striker : MonoBehaviour {

    Geography geography;
    Actor actor;

    // Unity

    private void Awake()
    {
        geography = GetComponentInParent<World>().GetComponentInChildren<Geography>();
        actor = GetComponent<Actor>();
    }


    private void Start()
    {
        ConfigureRoleSpecificProperties();
        Strategize();
    }


    private void Update()
    {

    }


    // public


    private void Restrategize()
    {
        if (actor.attack != null) {

        }
        else {

        }
    }


    private void Strategize()
    {
        // TODO: differentiate between Mhoddim and Ghaddim approaches

        if (actor.attack != null) {
        }
        else {
        }
    }


    // private


    private void ConfigureRoleSpecificProperties()
    {
        actor = GetComponent<Actor>();
        actor.SetComponents();
        actor.SetStats();
    }
}