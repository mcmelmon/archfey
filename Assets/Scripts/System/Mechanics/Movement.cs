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
        Agent = GetComponentInParent<NavMeshAgent>();
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
        if (Agent.hasPath) {
            if (ReachedNearObjective())
                GetNextObjective();
            if (ObjectiveComplete())
                GetNewObjective();
        }
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


    public void SetRoute(Route _route, bool accumulate = false)
    {
        if (Route != null && accumulate) _route.AccumulateRoutes(Route);
        Route = _route;
        StartCoroutine(FindThePath());
    }


    // private


    private IEnumerator FindThePath()
    {
        int attempt = 0;
        int max_attempts = 5;

        while (Route != null && !Agent.hasPath && attempt < max_attempts) {
            if (Agent.isOnNavMesh) {
                Agent.SetDestination(new Vector3(Route.Current.x, Geography.Terrain.SampleHeight(Route.Current), Route.Current.z));
            } else {
                attempt++;
                NavMesh.SamplePosition(Agent.transform.position, out NavMeshHit hit, 10.0f, NavMesh.AllAreas);
                Agent.Warp(hit.position);
                Debug.Log("Warp " + attempt);
            }
            yield return new WaitForSeconds(Turn.action_threshold);
        }
    }

    private void GetNewObjective()
    {
        if (Route.WhenComplete != null) {
            Route.WhenComplete.Invoke();
        } else if (Route.RoutesFollowed.Count > 0) {
            Route = Route.RoutesFollowed[Route.RoutesFollowed.Count -1];
        } else {
            Route = null;
        }
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