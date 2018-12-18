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


    // public

    // TODO: warp the agent to the nearest navmesh point if it is off the mesh
    // TODO: deal with an agent that is stuck
    // TODO: if a destination is unreachable, choose the nearest reachable one

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
        if (agent.isOnNavMesh) 
            agent.ResetPath();
    }


    public void SetDestination(Vector3 destination)
    {
        if (agent.isOnNavMesh) 
            agent.SetDestination(new Vector3(destination.x, 0, destination.z));  // TODO: sample the height at the destination from terrain
    }


    public void SetRoute(Route _route)
    {
        if (route != null) _route.AccumulateRoutes(route);
        route = _route;
        agent.SetDestination(new Vector3(route.current.x, 0, route.current.z));  // TODO: sample the height at the destination from terrain
        StartCoroutine(MonitorProgress());
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
        if (!route.completed)
            agent.SetDestination(route.current);
    }


    private IEnumerator MonitorProgress()
    {
        while (true) {
            if (ReachedNearObjective()) 
                GetNextObjective();
            if (ObjectiveComplete()) 
                GetNewObjective();

            yield return null;
        }
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