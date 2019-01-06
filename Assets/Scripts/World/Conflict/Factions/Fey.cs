using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fey : MonoBehaviour {


    // properties

    public Stats Stats { get; set; }


    // Unity


    private void Awake()
    {
        Stats = GetComponent<Stats>();
    }


    // public


    public void SetStats()
    {
        SetPrimaryStats();
        SetDefenseStats();
        SetHealthStats();
    }


    // private


    private void SetDefenseStats()
    {
        Defend defend = GetComponent<Defend>();
        if (defend == null) return;

        if (GetComponent<Ent>() != null)
        {

        }
    }


    private void SetHealthStats()
    {
        Health health = GetComponent<Health>();
        if (health == null) return;

        if (GetComponent<Ent>() != null)
        {

        }
    }


    private void SetPrimaryStats()
    {
        if (GetComponent<Ent>() != null) {

        }
    }
}
