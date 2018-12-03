using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scout : MonoBehaviour {

    Geography geography;
    Attack offense;
    Defend defense;
    readonly float sense_radius = 40f;
    Route my_route;


    // Unity

    private void Awake()
    {
        geography = GetComponentInParent<World>().GetComponentInChildren<Geography>();
        offense = GetComponentInParent<Attack>();
        defense = GetComponentInParent<Defend>();
        GetComponent<Senses>().radius = sense_radius;
    }


    private void Start () {
        EstablishRoute();
    }


    private void Update () 
    {
        if (my_route.ReachedCurrentVertex(transform.position)) ExploreMap();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.TransformDirection(Vector3.forward * 100));

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, (my_route.current_vertex - transform.position));
    }

    // private


    private void EstablishRoute()
    {
        Circle exploration_circle = Circle.CreateCircle(geography.GetCenter(), (geography.GetResolution() / 2f) - (sense_radius / 2f), 18);
        my_route = Route.CreateRoute(exploration_circle.VertexClosestTo(transform.position), exploration_circle);

        GetComponentInParent<Actor>().Move(my_route.current_vertex);
    }


    private void ExploreMap()
    {
        my_route.SetNextVertex(true);
        GetComponentInParent<Actor>().Move(my_route.current_vertex);
    }
}


class Route
{
    public Circle path;
    public Vector3 starting_vertex;
    public Vector3 current_vertex;
    public bool completed;
    public bool clockwise;

    public static Route CreateRoute(Vector3 start, Circle circle)
    {
        Route route = new Route();
        route.path = circle;
        route.starting_vertex = start;
        route.current_vertex = start;
        route.completed = false;
        route.clockwise = (Random.Range(0, 1f) > .5f) ? true : false;

        return route;
    }

    public bool ReachedCurrentVertex(Vector3 current_location)
    {
        return (Vector3.Distance(current_vertex, current_location) < 4f) ? true : false;
    }


    public void SetNextVertex(bool looping)
    {
        if (!looping && completed) return;
        int next_index;

        int index = path.vertices.IndexOf(current_vertex);
        if (clockwise)
        {
            next_index = ((index - 1) + (path.vertex_count)) % (path.vertex_count);
        } else {
            next_index = (index + 1) % (path.vertex_count);
        }
        Vector3 next_vertex = path.vertices[next_index];

        if (next_vertex == starting_vertex){
            completed = true;
        }

        current_vertex = next_vertex;
    }
}