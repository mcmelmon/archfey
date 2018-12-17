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


    private void Start()
    {
        StartCoroutine(MonitorProgress());
    }


    void Update () {

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
        destination.y = transform.position.y;
        if (agent.isOnNavMesh) 
            agent.SetDestination(destination);
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


    private IEnumerator MonitorProgress()
    {
        while (agent.hasPath) {
            if (ReachedNearObjective()) GetNextObjective();
            if (ObjectiveComplete()) GetNewObjective();
            if (Vector3.Distance(transform.position, agent.destination) < .4f)
                agent.ResetPath();

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