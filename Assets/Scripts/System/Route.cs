using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// TODO: make this less specific to Scouts

public class Route
{
    public enum Geometry { Circuitous = 0, Line = 1 };

    public Geometry geometry;
    public Vector3 current, finish, next, start;
    public bool completed;
    public bool looping;
    Action when_complete;
    public List<Route> routes_followed = new List<Route>();

    public Circuitous circuitous;


    public struct Circuitous {
        public Circle path;
        public bool clockwise;
    }


    public void AccumulateRoutes(Route previous_route)
    {
        routes_followed = previous_route.routes_followed;
        routes_followed.Add(previous_route);
    }


    public static Route Circular(Vector3 _start, Circle _circle, bool _looping = false, Action _when_complete = null)
    {
        // TODO: the vertices could be added to a list of vectors similar to the line
        // Move to the starting vertex, then follow _circle's vertices clockwise or counterclockwise
        Route route = new Route
        {
            geometry = Geometry.Circuitous,
            start = _start,
            current = _start,
            circuitous = new Circuitous(),
            completed = false,
            looping = _looping,
            when_complete = _when_complete
        };

        route.circuitous.path = _circle;
        route.circuitous.clockwise = (UnityEngine.Random.Range(0, 1f) > .5f) ? true : false;

        return route;
    }


    public static Route Line(Vector3 _start, Vector3 _finish, bool _looping = false, Action _when_complete = null)
    {
        // TODO: pass in a list of points and connect them.
        Route route = new Route
        {
            geometry = Geometry.Line,
            current = _finish,
            start = _start,
            finish = _finish,
            completed = false,
            looping = _looping,
            when_complete = _when_complete
        };

        return route;
    }

    public bool ReachedCurrent(Vector3 unit_position)
    {
        return (Vector3.Distance(current, unit_position) < 8f) ? true : false;
    }


    public Action GetWhenComplete()
    {
        return when_complete;
    }


    public void SetNext()
    {
        if (!looping && completed) return;

        if (geometry == Geometry.Circuitous) {
            int next_index;
            int index = circuitous.path.vertices.IndexOf(current);

            if (circuitous.clockwise) {
                next_index = ((index - 1) + (circuitous.path.vertex_count)) % circuitous.path.vertex_count;
            } else {
                next_index = (index + 1) % circuitous.path.vertex_count;
            }

            next = circuitous.path.vertices[next_index];
        } else {
            // TODO: handle moving through a list of vectors
        }

        if (next == start) {
            completed = !looping;
        }

        current = next;
    }
}