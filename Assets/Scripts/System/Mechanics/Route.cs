using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


public class Route
{
    // properties

    public Vector3 Current { get; set; }
    public bool Looping { get; set; }
    public Vector3 Next { get; set; }
    public List<Vector3> Points { get; set; }
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

        route.Points = new List<Vector3>();

        foreach (var vertex in _circle.Vertices)
        {
            route.Points.Add(vertex);
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

        route.Points = new List<Vector3> {
            _start,
            _next
        };
        
        return route;
    }


    // public



    public void Add(Vector3 _point)
    {
        // This will "work" for a circle, but kind of awkward

        Points.Add(_point);
    }


    public bool Completed()
    {
        return (Next == Start) && !Looping && !Retracing;
    }


    public bool ReachedCurrent(Vector3 unit_position)
    {
        return Vector3.Distance(Current, unit_position) < Movement.ReachedThreshold;
    }


    public Vector3 SetNext()
    {
        bool keep_going = (Looping || Retracing);
        if (Completed() && !keep_going) return Current;

        int next_index;
        int current_index = Points.IndexOf(Current);
        next_index = (Next == Start && Retracing) ? ((current_index - 1) + (Points.Count)) % Points.Count : (current_index + 1) % Points.Count;
        Next = Points[next_index];
        Current = Next;
        return Current;
    }
}