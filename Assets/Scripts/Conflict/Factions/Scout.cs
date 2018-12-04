using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scout : MonoBehaviour {

    Geography geography;
    Actor actor;
    Senses senses;
    readonly float sense_radius = 40f;
    Route my_route;


    // Unity

    private void Awake()
    {
        geography = GetComponentInParent<World>().GetComponentInChildren<Geography>();
        actor = GetComponent<Actor>();
        GetComponent<Senses>().radius = sense_radius;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.TransformDirection(Vector3.forward * 100));

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, (my_route.current_vertex - transform.position));
    }


    private void Start () {
        EstablishRoute();
    }


    private void Update () 
    {
        if (ReachedGoal()) ExploreMap();
        if (FinishedPath()) ContractRoute();
    }


    // private


    private void ContractRoute()
    {
        my_route.ContractRoute();
        actor.Move(my_route.current_vertex);
    }


    private void EstablishRoute()
    {
        Circle exploration_circle = Circle.CreateCircle(geography.GetCenter(), (geography.GetResolution() / 2f) - sense_radius, 18);
        my_route = Route.CreateRoute(exploration_circle.VertexClosestTo(transform.position), exploration_circle);
        actor.Move(my_route.current_vertex);
    }


    private void ExploreMap()
    {
        my_route.SetNextVertex();
        actor.Move(my_route.current_vertex);
    }


    private bool FinishedPath()
    {
        return my_route.completed;
    }


    private bool ReachedGoal()
    {
        return my_route.ReachedCurrentVertex(transform.position);
    }
}