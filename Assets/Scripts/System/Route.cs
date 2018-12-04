using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: make this less specific to Scouts

class Route
{
    public Circle path;
    public Vector3 starting_vertex;
    public Vector3 current_vertex;
    public bool completed;
    public bool clockwise;


    public void ContractRoute()
    {
        path.Redraw(path.center, path.radius * .7f, path.vertex_count);
        completed = false;
        current_vertex = path.VertexClosestTo(current_vertex);
        starting_vertex = current_vertex;
    }


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
        return (Vector3.Distance(current_vertex, current_location) < 8f) ? true : false;
    }


    public void SetNextVertex(bool looping = false)
    {
        if (!looping && completed) return;
        int next_index;

        int index = path.vertices.IndexOf(current_vertex);
        if (clockwise)
        {
            next_index = ((index - 1) + (path.vertex_count)) % path.vertex_count;
        }
        else
        {
            next_index = (index + 1) % path.vertex_count;
        }
        Vector3 next_vertex = path.vertices[next_index];

        if (next_vertex == starting_vertex)
        {
            completed = true;
        }

        current_vertex = next_vertex;
    }
}