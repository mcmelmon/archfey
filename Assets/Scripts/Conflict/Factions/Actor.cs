using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour {

    Movement mover;


    // Unity


    private void Awake()
    {
        if (GetComponent<SphereCollider>() == null) AddSenses();
        mover = GetComponent<Movement>();
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


    // private


    private void AddSenses()
    {
        transform.gameObject.AddComponent<Senses>();
        GetComponent<SphereCollider>().isTrigger = true;
    }
}