using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// TODO: make this less specific to Scouts

public class Route
{
    public Circle path;
    public Vector3 starting_vertex;
    public Vector3 current_vertex;
    public bool completed;
    public bool clockwise;
    public bool looping;
    Action when_complete;
    public List<Route> routes_followed = new List<Route>();


    public void AccumulateRoutes(Route previous_route)
    {
        routes_followed = previous_route.routes_followed;
        routes_followed.Add(previous_route);
    }


    public static Route CircularRoute(Vector3 start, Circle circle, bool _looping = false, Action _when_complete = null)
    {
        Route route = new Route
        {
            path = circle,
            starting_vertex = start,
            current_vertex = start,
            completed = false,
            clockwise = (UnityEngine.Random.Range(0, 1f) > .5f) ? true : false,
            looping = _looping,
            when_complete = _when_complete
        };

        return route;
    }

    public bool ReachedCurrentVertex(Vector3 current_location)
    {
        return (Vector3.Distance(current_vertex, current_location) < 8f) ? true : false;
    }


    public Action GetWhenComplete()
    {
        return when_complete;
    }


    public void SetNextVertex()
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
            completed = !looping;
        }

        current_vertex = next_vertex;
    }
}