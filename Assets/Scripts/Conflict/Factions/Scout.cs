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
        if (my_route.ReachedCurrentVertex(transform.position)) ExploreMap();
    }


    // private


    private void EstablishRoute()
    {
        if (actor.GetAttackTransform() != null)
        {
            Circle exploration_circle = Circle.CreateCircle(geography.GetCenter(), (geography.GetResolution() / 2f) - (sense_radius / 2f), 18);
            my_route = Route.CreateRoute(exploration_circle.VertexClosestTo(transform.position), exploration_circle);
            actor.Move(my_route.current_vertex);
        }
    }


    private void ExploreMap()
    {
        my_route.SetNextVertex(true);
        actor.Move(my_route.current_vertex);
    }
}