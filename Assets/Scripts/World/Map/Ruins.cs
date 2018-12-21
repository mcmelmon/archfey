using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruins : MonoBehaviour {

    public enum Category { Primary = 0, Secondary = 1, Tertiary = 2 };
    public Ruin ruin_prefab;


    // properties

    public static List<RuinControlPoint> AllRuinControlPoints { get; set; }
    public static List<Ruin> AllRuins { get; set; }
    public static Dictionary<Category, Circle> Circles { get; set; }
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
        Locate();
        Construct();
    }


    public RuinControlPoint GetNearestUnoccupiedControlPoint(GameObject _unit)
    {
        float distance;
        float shortest_distance = float.MaxValue;
        RuinControlPoint nearest_control_point = null;

        foreach (var control_point in AllRuinControlPoints) {
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
        foreach (KeyValuePair<Category, Circle> circle in Circles) {
            switch (circle.Key) {
                case Category.Primary:
                    for (int i = 0; i < 3; i++) {
                        Vector3 position = circle.Value.RandomVertex();
                        if (!NearRuin(position, Ruin.MinimumRuinSpacing))
                            InstantiateRuin(position, this);
                    }
                    break;
                //case Category.Secondary:
                //    for (int i = 0; i < 5; i++) {
                //        Vector3 position = circle.Value.RandomVertex();
                //        if (!NearRuin(position, Ruin.MinimumRuinSpacing))
                //            InstantiateRuin(position, this);
                //    }
                //    break;
                //case Category.Tertiary:
                    //for (int i = 0; i < 3; i++) {
                    //    Vector3 position = circle.Value.RandomVertex();
                    //    if (!NearRuin(position, Ruin.MinimumRuinSpacing))
                    //        InstantiateRuin(position, this);
                    //}
                    //break;
            }
        }
    }


    private List<Vector3> GetRuinPositions()
    {
        List<Vector3> positions = new List<Vector3>();

        foreach (var ruin in AllRuins)
        {
            positions.Add(ruin.transform.position);
        }

        return positions;
    }


    private void InstantiateRuin(Vector3 point, Ruins _ruins)
    {
        Ruin _ruin = Instantiate(ruin_prefab, point, transform.rotation, _ruins.transform);
        _ruin.transform.localScale += new Vector3(4, 16, 4);
        _ruin.transform.position += new Vector3(0, _ruin.transform.localScale.y / 2, 0);
        if (_ruin != null) AllRuins.Add(_ruin);
    }


    private void Locate()
    {
        LocatePrimaryRuinComplex();
        LocateSecondaryRuinComplex();
        LocateTertiaryRuinComplex();
    }


    private void LocatePrimaryRuinComplex()
    {
        float distance_from_edge = 100f;
        Vector3 circle_center = Geography.Instance.RandomLocation(distance_from_edge);
        Circle spawn_circle = Circle.CreateCircle(circle_center, 40f);

        Circles[Category.Primary] = spawn_circle;
    }


    private void LocateSecondaryRuinComplex()
    {
        float distance_from_edge = 80f;
        Vector3 circle_center = Geography.Instance.RandomLocation(distance_from_edge);
        Circle spawn_circle = Circle.CreateCircle(circle_center, 20f);

        Circles[Category.Secondary] = spawn_circle;
    }


    private void LocateTertiaryRuinComplex()
    {
        float distance_from_edge = 80f;
        Vector3 circle_center = Geography.Instance.RandomLocation(distance_from_edge);
        Circle spawn_circle = Circle.CreateCircle(circle_center, 12f);

        Circles[Category.Tertiary] = spawn_circle;
    }


    private bool NearRuin(Vector3 position, float how_close)
    {
        foreach (var ruin in GetRuinPositions())
        {
            float distance = Vector3.Distance(position, ruin);
            if (distance < how_close) return true;
        }

        return false;
    }


    private void SetComponents()
    {
        AllRuinControlPoints = new List<RuinControlPoint>();
        AllRuins = new List<Ruin>();
        Circles = new Dictionary<Category, Circle>();
    }
}