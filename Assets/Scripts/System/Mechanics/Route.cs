using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


public class Route : MonoBehaviour
{
    // Inspector settings
    public List<Vector3> points = new List<Vector3>();

    // properties

    public Vector3 Current { get; set; }
    public bool Looping { get; set; }
    public Vector3 Next { get; set; }
    public bool Retracing { get; set; }
    public Vector3 Start { get; set; }


    // static


    public static Route Circular(Vector3 _start, Circle _circle, bool _retracing = false, bool _looping = false)
    {
        Route route = new Route
        {
            Current = _start,
            Start = _start,
            Looping = _looping,
            Retracing = _retracing
        };

        route.points = new List<Vector3>();

        foreach (var vertex in _circle.Vertices)
        {
            route.points.Add(vertex);
        }

        route.SetNext();

        return route;
    }


    public static Route Linear(Vector3 _start, Vector3 _next, bool _retracing = false, bool _looping = false)
    {
        Route route = new Route
        {
            Current = _next,
            Start = _start,
            Looping = _looping,
            Retracing = _retracing
        };

        route.points = new List<Vector3> {
            _start,
            _next
        };
        
        return route;
    }


    // public



    public void Add(Vector3 _point)
    {
        // This will "work" for a circle, but kind of awkward

        points.Add(_point);
    }


    public bool Completed()
    {
        return (Next == Start) && !Looping && !Retracing;
    }


    public bool ReachedCurrent(Vector3 unit_position)
    {
        return Vector3.Distance(Current, unit_position) < 3f;  // TODO: make unit specific
    }


    public Vector3 SetNext()
    {
        bool keep_going = (Looping || Retracing);
        if (Completed() && !keep_going) return Current;

        if (Start == Vector3.zero) {
            Start = Current = points[0];
            return Current;
        } else {
            int next_index;
            int current_index = points.IndexOf(Current);
            next_index = (Next == Start && Retracing) ? ((current_index - 1) + (points.Count)) % points.Count : (current_index + 1) % points.Count;
            Next = points[next_index];
            Current = Next;
            return Current;
        }
    }
}