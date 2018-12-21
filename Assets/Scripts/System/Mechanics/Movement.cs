using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class Movement : MonoBehaviour {

    // properties

    public NavMeshAgent Agent { get; set; }
    public Route Route { get; set; }


    // Unity


    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }


    private void OnDrawGizmos()
    {
        if (Route != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, (Route.Current - gameObject.transform.position));
        }
    }


    // public

    // TODO: warp the agent to the nearest navmesh point if it is off the mesh
    // TODO: deal with an agent that is stuck
    // TODO: if a destination is unreachable, choose the nearest reachable one


    public void Advance()
    {
        if (ReachedNearObjective())
            GetNextObjective();
        if (ObjectiveComplete())
            GetNewObjective();
    }


    public void ResetPath()
    {
        if (Agent.isOnNavMesh) 
            Agent.ResetPath();
    }


    public void SetDestination(Vector3 destination)
    {
        if (Agent.isOnNavMesh) 
            Agent.SetDestination(new Vector3(destination.x, 0, destination.z));  // TODO: sample the height at the destination from terrain
    }


    public void SetRoute(Route _route)
    {
        if (Route != null) _route.AccumulateRoutes(Route);
        Route = _route;
        Agent.SetDestination(new Vector3(Route.Current.x, 0, Route.Current.z));  // TODO: sample the height at the destination from terrain
    }


    // private


    private void GetNewObjective()
    {
        Agent.ResetPath();
        Action new_task = Route.WhenComplete;
        new_task();
    }


    private void GetNextObjective()
    {
        if (!Route.Completed()) {
            SetDestination(Route.SetNext());
        } else {
            ResetPath();
        }
    }


    private bool ObjectiveComplete()
    {
        return Route != null && Route.Completed();
    }


    private bool ReachedNearObjective()
    {
        return Agent != null && Route != null && Route.ReachedCurrent(Agent.transform.position);
    }
}