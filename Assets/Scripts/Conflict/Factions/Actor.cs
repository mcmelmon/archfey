using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour {

    Dictionary<string, GameObject> senses = new Dictionary<string, GameObject>();
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


    public void Move(Vector3 objective)
    {
        mover.MoveToward(objective);
    }


    // private


    private void AddSenses()
    {
        transform.gameObject.AddComponent<Senses>();
        GetComponent<SphereCollider>().isTrigger = true;
    }
}