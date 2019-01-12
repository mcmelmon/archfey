using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class Movement : MonoBehaviour
{

    // properties

    public NavMeshAgent Agent { get; set; }
    public static float ReachedThreshold { get; set; }
    public Route Route { get; set; }
    public float Speed { get; set; }


    // Unity


    private void Awake()
    {
        Agent = GetComponentInParent<NavMeshAgent>();
        ReachedThreshold = 3f;
    }


    // public


    public void Advance()
    {
        if (Agent.hasPath) {
            if (ReachedNearObjective())
                GetNextObjective();
            if (ObjectiveComplete())
                GetNewObjective();
        }
    }


    public bool InProgress()
    {
        return Agent.hasPath && Agent.remainingDistance > ReachedThreshold;
    }


    public void ResetPath()
    {
        if (Agent.isOnNavMesh) 
            Agent.ResetPath();
    }


    public void SetDestination(Vector3 destination)
    {
        StopCoroutine(FindThePath(destination));
        StartCoroutine(FindThePath(destination));
    }


    // private


    private IEnumerator FindThePath(Vector3 _destination)
    {
        int attempt = 0;
        int max_attempts = 5;

        while (!Agent.hasPath && attempt < max_attempts) {
            if (Agent.isOnNavMesh) {
                Agent.SetDestination(new Vector3(_destination.x, Geography.Terrain.SampleHeight(_destination), _destination.z));
            } else {
                attempt++;
                NavMesh.SamplePosition(Agent.transform.position, out NavMeshHit hit, 10.0f, NavMesh.AllAreas);
                Agent.Warp(hit.position);
                Debug.Log("Warp " + attempt);
            }
            yield return new WaitForSeconds(Turn.ActionThreshold);
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