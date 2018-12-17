using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


public class Route
{
    public bool completed;
    public Vector3 current, finish, next, start;
    public bool looping;
    public List<Vector3> points = new List<Vector3>();
    public bool retracing;
    public List<Route> routes_followed = new List<Route>();
    Action when_complete;


    public void AccumulateRoutes(Route previous_route)
    {
        routes_followed = previous_route.routes_followed;
        routes_followed.Add(previous_route);
    }


    public void Add(Vector3 _point)
    {
        // This will "work" for a circle, but kind of awkward

        points.Add(_point);
    }


    public static Route Circular(Vector3 _start, Circle _circle, Action _when_complete = null, bool _retracing = false, bool _looping = false)
    {
        Route route = new Route
        {
            current = _start,
            start = _start,
            completed = false,
            looping = _looping,
            retracing = _retracing,
            when_complete = _when_complete
        };

        foreach (var vertex in _circle.vertices) {
            route.points.Add(vertex);
        }

        route.SetNext();

        return route;
    }


    public static Route Linear(Vector3 _start, Vector3 _next, Action _when_complete = null, bool _retracing = false, bool _looping = false)
    {
        Route route = new Route
        {
            current = _next,
            start = _start,
            completed = false,
            looping = _looping,
            retracing = _retracing,
            when_complete = _when_complete
        };

        route.points.Add(_start);
        route.points.Add(_next);

        return route;
    }


    public bool ReachedCurrent(Vector3 unit_position)
    {
        return (Vector3.Distance(current, unit_position) < 5f) ? true : false;
    }


    public Action GetWhenComplete()
    {
        return when_complete;
    }


    public void SetNext()
    {
        bool keep_going = (looping || retracing);
        if (completed && !keep_going) return;

        int next_index;
        int current_index = points.IndexOf(current);
        next_index = (next == start && retracing) ? ((current_index - 1) + (points.Count)) % points.Count : (current_index + 1) % points.Count;
        next = points[next_index];
        current = next;

        if (next == start) completed = !looping || !retracing;
    }
}