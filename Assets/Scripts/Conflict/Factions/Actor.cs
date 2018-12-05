using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour {

    public Mhoddim mhoddim;
    public Ghaddim ghaddim;
    public Attack attack;
    public Defend defend;
    public Movement movement;


    // Unity


    private void Awake()
    {
        if (GetComponent<SphereCollider>() == null) AddSenses();
    }


    private void Start()
    {

    }


    private void Update()
    {

    }


    // public


    public void Attack()
    {

    }


    public void SetComponents()
    {
        mhoddim = GetComponent<Mhoddim>();
        ghaddim = GetComponent<Ghaddim>();
        attack = GetComponent<Attack>();
        defend = GetComponent<Defend>();
        movement = GetComponent<Movement>();
    }


    public void SetStats()
    {
        if (mhoddim != null) {
            mhoddim.SetHealthStats(gameObject);
        }
        else {
            ghaddim.SetHealthStats(gameObject);
        }
    }


    // private


    private void AddSenses()
    {
        transform.gameObject.AddComponent<Senses>();
        GetComponent<SphereCollider>().isTrigger = true;
    }
}