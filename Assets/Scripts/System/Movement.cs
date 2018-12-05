using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class Movement : MonoBehaviour {

    NavMeshAgent agent;
    Route route;
    GameObject role;


    // Unity


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }


    private void OnDrawGizmos()
    {
        if (route != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, (route.current_vertex - gameObject.transform.position));
        }
    }


    void Update () {
        if (ReachedNearObjective()) GetNextObjective();
        if (ObjectiveComplete()) GetNewObjective();
    }


    // public


    public Route GetRoute()
    {
        return route;
    }


    public void SetRoute(Route _route)
    {
        route = _route;
        agent.SetDestination(route.current_vertex);
    }


    // private


    private void GetNewObjective()
    {
        Action when_completed = route.GetWhenComplete();
        when_completed();
    }


    private void GetNextObjective()
    {
        route.SetNextVertex();
        agent.SetDestination(route.current_vertex);
    }


    private bool ObjectiveComplete()
    {
        return route != null && route.completed ? true : false;
    }


    private bool ReachedNearObjective()
    {
        return agent != null && route != null && route.ReachedCurrentVertex(agent.transform.position);
    }
}