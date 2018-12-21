using System.Collections;
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
            TopLeft = _top_left,
            Width = _width,
            Depth = _depth,
            Spacing = _spacing,
            ElementArray = new Vector3[_width, _depth],
            Elements = new List<Vector3>()
        };

        _grid.Draw(draw_points);

        return _grid;
    }


    public Vector3 GetDepthDirection()
    {
        return TopLeft - ElementArray[0, Depth - 1];
    }


    public Vector3 GetWidthDirection()
    {
        return TopLeft - ElementArray[Width - 1, 0];
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
                    cube.transform.localScale = new Vector3(2, 10, 2);
                    cube.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
                }
            }
        }

    }
}