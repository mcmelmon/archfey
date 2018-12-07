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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.TransformDirection(Vector3.forward * 50));
    }


    // public


    public void Attack()
    {

    }


    public void Move(Route _route)
    {
        movement.SetRoute(_route);
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