using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formation
{
    public enum Alignment { Circle = 0, Grid = 1 };
    Alignment alignment;


    // properties

    public bool Objective { get; set; }
    public Vector3 Anchor { get; set; }
    public List<GameObject> Units { get; set; }
    Circle circular_formation;
    Grid rectangular_formation;
    public float Spacing { get; set; }
    public Route Route { get; set; }


    // static


    public static Formation New(Vector3 _anchor, Alignment _profile, float _spacing = 5f)
    {
        Formation _formation = new Formation
        {
            alignment = _profile,
            Anchor = _anchor,
            Spacing = _spacing,
            Units = new List<GameObject>()
    };

        return _formation;
    }


    // public


    public void Face(Vector3 facing)
    {
        switch (alignment)
        {
            case Alignment.Circle:
                foreach (var unit in Units) {
                    facing = unit.transform.position - Anchor;
                    facing.y = 0;
                    unit.transform.rotation = Quaternion.LookRotation(facing);
                }
                break;
            case Alignment.Grid:
                foreach (var unit in Units)
                {
                    facing.y = 0;
                    unit.transform.rotation = Quaternion.LookRotation(facing);
                }
                break;
        }
    }


    public void JoinFormation(GameObject unit)
    {
        Units.Add(unit);

        switch (alignment)
        {
            case Alignment.Circle:
                circular_formation = Circle.CreateCircle(Anchor, Units.Count, Units.Count);
                PositionCircle(circular_formation);
                break;
            case Alignment.Grid:
                rectangular_formation = Grid.New(Anchor, Mathf.RoundToInt(Mathf.Sqrt(Units.Count)) + 1, Mathf.RoundToInt(Mathf.Sqrt(Units.Count)) + 1, Spacing);
                PositionInGrid(rectangular_formation);
                break;
        }
    }


    public void Restrategize()
    {

    }


    public void Strategize()
    {

    }


    // private


    private void March()
    {
        foreach (var unit in Units) {
            if (Route != null) {
                unit.GetComponent<Movement>().SetRoute(Route);
            }
        }
    }

    private void PositionCircle(Circle formation)
    {
        for (int i = 0; i < Units.Count; i++)
        {
            Units[i].transform.position = formation.vertices[i];
        }
        Face(Vector3.zero);
    }


    private void PositionInGrid(Grid formation)
    {
        for (int i = 0; i < Units.Count; i++) {
            Units[i].transform.position = formation.Elements[i];
        }
        Face(rectangular_formation.GetDepthDirection());
    }
}