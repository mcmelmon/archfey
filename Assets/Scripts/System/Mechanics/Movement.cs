using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class Movement : MonoBehaviour {

    NavMeshAgent agent;
    Route route;


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
            Gizmos.DrawRay(transform.position, (route.current - gameObject.transform.position));
        }
    }


    void Update () {
        if (ReachedNearObjective()) GetNextObjective();
        if (ObjectiveComplete()) GetNewObjective();
    }


    // public

    // TODO: warp the agent to the nearest navmesh point if it is off the mesh
    // TODO: deal with an agent that is stuck

    public NavMeshAgent GetAgent()
    {
        return agent;
    }


    public Route GetRoute()
    {
        return route;
    }


    public void ResetPath()
    {
        if (agent.isOnNavMesh) agent.ResetPath();
    }


    public void SetDestination(Vector3 destination)
    {
        destination.y = 0;
        if (agent.isOnNavMesh) agent.SetDestination(destination);
    }


    public void SetRoute(Route _route)
    {
        route = _route;
        agent.SetDestination(route.current);
    }


    // private


    private void GetNewObjective()
    {
        Action when_completed = route.GetWhenComplete();
        when_completed();
    }


    private void GetNextObjective()
    {
        route.SetNext();
        agent.SetDestination(route.current);
    }


    private bool ObjectiveComplete()
    {
        return route != null && route.completed ? true : false;
    }


    private bool ReachedNearObjective()
    {
        return agent != null && route != null && route.ReachedCurrent(agent.transform.position);
    }
}