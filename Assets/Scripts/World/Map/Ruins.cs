using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruins : MonoBehaviour {

    public enum Category { Primary = 0, Secondary = 1, Tertiary = 2 };
    public Ruin ruin_prefab;


    // properties

    public static List<Ruin> RuinBlocks { get; set; }
    public static List<Grid> RuinComplexes { get; set; }
    public static List<RuinControlPoint> RuinControlPoints { get; set; }
    public static Ruins Instance { get; set; }


    // Unity


    private void Awake()
    {
        if (Instance != null) {
            Debug.LogError("More than one ruins instance");
            Destroy(this);
            return;
        }
        Instance = this;
    }


    // public


    public void ErectRuins()
    {
        SetComponents();
        Construct();
    }


    public RuinControlPoint GetNearestUnoccupiedControlPoint(GameObject _unit)
    {
        float distance;
        float shortest_distance = float.MaxValue;
        RuinControlPoint nearest_control_point = null;

        foreach (var control_point in RuinControlPoints) {
            if (!control_point.Occupied) {
                distance = Vector3.Distance(control_point.transform.position, _unit.transform.position);
                if (distance < shortest_distance) {
                    nearest_control_point = control_point;
                    shortest_distance = distance;
                }
            }
        }

        return nearest_control_point;
    }


    // private


    private void Construct()
    {
        Grid ruin_grid = Grid.New(Locate(200f), Random.Range(5, 8), Random.Range(5, 8), Spacing());
        RuinComplex _complex = RuinComplex.New(ruin_grid, ruin_prefab, this);
        if (_complex.RuinBlocks.Count > 0) {
            RuinBlocks.AddRange(_complex.RuinBlocks);
            RuinControlPoints.AddRange(_complex.RuinControlPoints);
        }
    }


    private Vector3 Locate(float distance_from_edge)
    {
        return Geography.Instance.RandomLocation(distance_from_edge);
    }


    private void SetComponents()
    {
        RuinBlocks = new List<Ruin>();
        RuinComplexes = new List<Grid>();
        RuinControlPoints = new List<RuinControlPoint>();
    }


    private float Spacing()
    {
        return ruin_prefab.transform.localScale.x * ruin_prefab.transform.localScale.x + ruin_prefab.transform.localScale.z * ruin_prefab.transform.localScale.z;
    }
}


public class RuinComplex
{
    // properties

    public Grid Grid { get; set; }
    public List<Ruin> RuinBlocks { get; set; }
    public List<RuinControlPoint> RuinControlPoints { get; set; }


    public static RuinComplex New(Grid _grid, Ruin prefab, Ruins _ruins)
    {
        RuinComplex _complex = new RuinComplex
        {
            Grid = _grid,
            RuinBlocks = new List<Ruin>(),
            RuinControlPoints = new List<RuinControlPoint>()
        };

        foreach (var location in _complex.Grid.Elements) {
            if (Random.Range(0, 99) < 40)
            {
                Ruin _ruin = Ruin.InstantiateRuin(prefab, location, _ruins);
                _complex.RuinBlocks.Add(_ruin);
                _complex.RuinControlPoints.AddRange(_ruin.ControlPoints);
            }
        }

        return _complex;
    }
}