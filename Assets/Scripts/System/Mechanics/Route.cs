using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Route : MonoBehaviour
{

    public enum RouteType { Loop, Retrace, Stop };

    // Inspector settings
    public Vector3[] local_stops = new Vector3[1];
    public int[] wait_times = new int[1];
    public RouteType route_type;

    // properties

    public bool Completed { get; set; }
    public int CurrentIndex { get; set; }
    public List<Vector3> Diversions { get; set; }
    public int FinishIndex { get; set; }
    public Actor Me { get; set; }
    public int NextIndex { get; set; }
    public Vector3[] WorldStops { get; set; }


    // Unity

    private void Awake()
    {
        CurrentIndex = 0;
        FinishIndex = local_stops.Length - 1;
        NextIndex = local_stops.Length > 1 ? 1 : 0;
        Completed = CurrentIndex == FinishIndex;
        Diversions = new List<Vector3>();
        Me = GetComponent<Actor>();

        WorldStops = new Vector3[local_stops.Length];
        for (int i = 0; i < WorldStops.Length; ++i)
            WorldStops[i] = transform.TransformPoint(local_stops[i]);
    }


    // public


    public void DivertTo(Vector3 point)
    {
        Diversions.Add(point);
    }


    public void MoveToNextPosition()
    {
        Me.Actions.Movement.SetDestination(GetNextPosition());
    }


    // private


    private bool CheckIfCompleted()
    {
        Completed = Completed || CurrentIndex == FinishIndex;
        if ((CurrentIndex == 0) && (route_type == RouteType.Retrace || route_type == RouteType.Loop)) {
            Completed = false;
        }
        return Completed;
    }


    private Vector3 GetNextPosition()
    {
        if (Diversions.Any()) {

        } else {
            switch(route_type) {
                case RouteType.Loop:
                    NextIndex = Completed ? 0 : CurrentIndex + 1;
                    break;
                case RouteType.Retrace:
                    NextIndex = Completed ? ((CurrentIndex - 1) + (local_stops.Length)) % local_stops.Length : (CurrentIndex + 1) % local_stops.Length;
                    break;
                case RouteType.Stop:
                    NextIndex = Completed ? FinishIndex : CurrentIndex + 1;
                    break;
            }
        }

        CurrentIndex = NextIndex;
        CheckIfCompleted();

        return WorldStops[NextIndex];
    }
}