using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Route : MonoBehaviour
{
    // Inspector settings
    public List<Transform> points = new List<Transform>();
    public bool looping;
    public bool retracing;

    // properties

    public bool Completed { get; set; }
    public int CurrentIndex { get; set; }
    public List<Transform> Diversions { get; set; }
    public int FinishIndex { get; set; }
    public Actor Me { get; set; }
    public int NextIndex { get; set; }


    // Unity


    private void Awake()
    {
        if (points.Any()) {
            CurrentIndex = 0;
            FinishIndex = points.Count - 1;
            NextIndex = points.Count > 1 ? 1 : 0;
        }

        Completed = CurrentIndex == FinishIndex;
        Diversions = new List<Transform>();
        Me = GetComponent<Actor>();

        if (looping) retracing = false;
    }


    // public


    public void DivertTo(Transform point)
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
        if ((CurrentIndex == 0) && (retracing || looping)) {
            Completed = false;
        }
        return Completed;
    }


    private Vector3 GetNextPosition()
    {
        if (Diversions.Any()) {

        } else {
            if (retracing) {
                NextIndex = Completed ? ((CurrentIndex - 1) + (points.Count)) % points.Count : (CurrentIndex + 1) % points.Count;
            } else if (looping) {
                NextIndex = Completed ? 0 : CurrentIndex + 1;
            } else if (!Completed) {
                NextIndex = CurrentIndex + 1;
            }
        }

        CurrentIndex = NextIndex;
        CheckIfCompleted();

        return points[NextIndex].position;
    }
}