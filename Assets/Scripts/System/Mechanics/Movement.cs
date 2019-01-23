using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    public enum CommonDestination { Home = 0, Harvest = 1, Craft = 2, Military = 3, Warehouse = 4 };

    // properties

    public Actor Me { get; set; }
    public NavMeshAgent Agent { get; set; }
    public Dictionary<CommonDestination, Vector3> Destinations { get; set; }
    public static float ReachedThreshold { get; set; }
    public Route Route { get; set; }
    public float Speed { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    // public


    public void AddDestination(CommonDestination key, Vector3 destination)
    {
        if (!Destinations.ContainsKey(key)) {
            Destinations[key] = destination;
        }
    }


    public void Home()
    {
        SetDestination(Destinations[CommonDestination.Home]);
    }


    public bool InProgress()
    {
        Vector3 height_adjusted_destination = new Vector3(Agent.destination.x, transform.position.y, Agent.destination.z);
        float separation = Vector3.Distance(transform.position, height_adjusted_destination);
        return Agent.hasPath && separation >= ReachedThreshold;
    }


    public void ResetPath()
    {
        if (Agent.isOnNavMesh) 
            Agent.ResetPath();
    }


    public void SetDestination(Transform _destination)
    {
        ResetPath();

        Collider target_collider = _destination.GetComponent<Collider>();

        Vector3 destination = (target_collider != null) ? target_collider.ClosestPointOnBounds(transform.position) : _destination.position;
    
        StopCoroutine(FindThePath(destination));
        StartCoroutine(FindThePath(destination));
    }


    public void SetDestination(Vector3 destination)
    {
        ResetPath();
        StopCoroutine(FindThePath(destination));
        StartCoroutine(FindThePath(destination));
    }


    public void Warehouse()
    {
        SetDestination(Destinations[CommonDestination.Warehouse]);
    }


    public void Work()
    {
        if (Destinations.ContainsKey(CommonDestination.Craft)) {
            SetDestination(Destinations[CommonDestination.Craft]);
        } else if (Destinations.ContainsKey(CommonDestination.Harvest)) {
            SetDestination(Destinations[CommonDestination.Harvest]);
        }
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


    private bool ObjectiveComplete()
    {
        return Route != null && Route.Completed();
    }


    private bool ReachedNearObjective()
    {
        return Agent != null && Route != null && Route.ReachedCurrent(Agent.transform.position);
    }


    private void SetComponents()
    {
        Agent = GetComponentInParent<NavMeshAgent>();
        Agent.ResetPath();
        Destinations = new Dictionary<CommonDestination, Vector3>();
        Me = GetComponentInParent<Actor>();
        ReachedThreshold = Me.Size;
    }
}