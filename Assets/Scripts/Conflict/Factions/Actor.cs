﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Actor : MonoBehaviour {

    public Vector3 destination;

    NavMeshAgent agent;
    Dictionary<string, GameObject> senses = new Dictionary<string, GameObject>();

    // Unity


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        destination = Vector3.zero;
    }


    private void Start()
    {

    }


    private void Update()
    {

    }


    // public


    public void SetDestination(Vector3 point) 
    {
        destination = point;
        agent.SetDestination(destination);
    }


    // private


    private void AttackInMelee()
    {

    }


    private void AttackAtRange()
    {

    }


    private void EvaluateAttacks()
    {

    }


    private void Move()
    {

    }
}