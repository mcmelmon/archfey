using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{

    // properties

    public int Depth { get; set; }
    public Vector3[,] ElementArray { get; set; }
    public List<Vector3> Elements { get; set; }
    public float Spacing { get; set; }
    public Vector3 TopLeft { get; set; }
    public int Width { get; set; }


    // static

    public static Grid New(Vector3 _top_left, int _width, int _depth, float _spacing, bool draw_points = false)
    {
        Grid _grid = new Grid
        {
            TopLeft = _top_left + new Vector3((_spacing / 2) - 1, 0, 0) - new Vector3(0, 0, (_spacing / 2) - 1),
            Width = _width,
            Depth = _depth,
            Spacing = _spacing,
            ElementArray = new Vector3[_width, _depth],
            Elements = new List<Vector3>()
        };

        _grid.Draw(draw_points);

        return _grid;
    }


    public Vector3 RandomPoint()
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        int _w = UnityEngine.Random.Range(0, Width);
        int _d = UnityEngine.Random.Range(0, Depth);
        return ElementArray[_w, _d];
    }


    public Vector3 RandomPoint(int distance_from_edge)
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);

        if (distance_from_edge > Width / Spacing || distance_from_edge > Depth / Spacing) distance_from_edge = (int)Math.Min(Width / Spacing, Depth / Spacing);

        UnityEngine.Random.InitState((int)Time.time);
        return ElementArray[UnityEngine.Random.Range(distance_from_edge, Width - distance_from_edge), UnityEngine.Random.Range(distance_from_edge, Depth - distance_from_edge)];
    }


    public Vector3 RandomPoint(Map.Cardinal cardinal)
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);

        switch (cardinal) {
            case Map.Cardinal.North:
                return ElementArray[UnityEngine.Random.Range(0, Width), Depth];
            case Map.Cardinal.East:
                return ElementArray[Width, UnityEngine.Random.Range(0, Depth)];
            case Map.Cardinal.South:
                return ElementArray[UnityEngine.Random.Range(0, Width), 0];
            case Map.Cardinal.West:
                return ElementArray[0, UnityEngine.Random.Range(0, Depth)];
        }

        return Vector3.zero;
    }


    // private


    private void Draw(bool draw_points)
    {
        for (int w = 0; w < Width; w++) {
            for (int d = 0; d < Depth; d++) {
                ElementArray[w, d] = TopLeft + new Vector3(w, 0, 0) * Spacing + new Vector3(0, 0, -d) * Spacing;
                Elements.Add(ElementArray[w, d]);

                if (draw_points) {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.name = "Point";
                    cube.transform.position = ElementArray[w, d];
                    cube.transform.localScale = new Vector3(Spacing, 1, Spacing);
                    cube.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
                }
            }
        }

    }
}