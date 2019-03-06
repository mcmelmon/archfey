using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    public enum CommonDestination { Home, Harvest, Craft, Military, Repair, Warehouse };

    // properties

    public Actor Me { get; set; }
    public NavMeshAgent Agent { get; set; }
    public Dictionary<CommonDestination, Vector3> Destinations { get; set; }
    public float ReachedThreshold { get; set; }
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
        Agent.ResetPath();
    }


    public void SetDestination(Transform target_object)
    {
        ResetPath();
        Collider target_collider = target_object.GetComponent<Collider>();
        Vector3 destination = (target_collider != null) ? target_collider.ClosestPointOnBounds(transform.position) : target_object.position;
        Vector3 new_facing = Vector3.RotateTowards(transform.forward, transform.position - destination, 30f * Time.deltaTime, 0f);
        transform.rotation = Quaternion.LookRotation(new_facing);
        Agent.SetDestination(destination);  // may have height issues on terrain
    }


    public void SetDestination(Vector3 destination)
    {
        ResetPath();
        Vector3 new_facing = Vector3.RotateTowards(transform.forward, transform.position - destination, 30f * Time.deltaTime, 0f);
        transform.rotation = Quaternion.LookRotation(new_facing);
        Agent.SetDestination(destination);  // may have height issues on terrain
    }


    public IEnumerator TrackUnit(Actor unit)
    {
        float separation = Vector3.Distance(transform.position, unit.transform.position);
        int count = 0;

        while (unit != null && count < Turn.ActionThreshold && separation > ReachedThreshold) {
            SetDestination(unit.MoveToInteractionPoint(Me));
            count++;
            yield return new WaitForSeconds(1);
        }
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


    private IEnumerator FindThePath(Vector3 destination)
    {
        // In case agents start getting spawned away from the navmesh...
        int attempt = 0;
        int max_attempts = 5;

        while (!Agent.hasPath && attempt < max_attempts) {
            if (Agent.isOnNavMesh) {
                Agent.SetDestination(new Vector3(destination.x, Geography.Terrain.SampleHeight(destination), destination.z));
            } else {
                attempt++;
                NavMesh.SamplePosition(Agent.transform.position, out NavMeshHit hit, 10.0f, NavMesh.AllAreas);
                Agent.Warp(hit.position);
                Debug.Log("Warp " + attempt);
            }
            yield return new WaitForSeconds(Turn.ActionThreshold);
        }
    }


    private void SetComponents()
    {
        Agent = GetComponentInParent<NavMeshAgent>();
        Agent.ResetPath();
        Destinations = new Dictionary<CommonDestination, Vector3>();
        Me = GetComponentInParent<Actor>();
    }
}