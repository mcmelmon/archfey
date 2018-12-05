using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rectangle
{
    public Vector3 top_left;
    public int width, depth;
    public float spacing;
    public Vector3[,] grid;
    public List<Vector3> points = new List<Vector3>();

    public static Rectangle CreateRectangle(Vector3 _top_left, int _width, int _depth, float _spacing, bool draw_points = false)
    {
        Rectangle _rectangle = new Rectangle
        {
            top_left = _top_left,
            width = _width,
            depth = _depth,
            spacing = _spacing,
            grid = new Vector3[_width, _depth]
        };

        _rectangle.Draw(draw_points);

        return _rectangle;
    }


    // private

    private void Draw(bool draw_points)
    {
        for (int w = 0; w < width; w++)
        {
            for (int d = 0; d < depth; d++)
            {
                grid[w, d] = top_left + new Vector3(w, 0, 0) * spacing + new Vector3(0, 0, -d) * spacing;
                points.Add(grid[w, d]);

                if (draw_points)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.name = "Point";
                    cube.transform.position = grid[w, d];
                    cube.transform.localScale = new Vector3(2, 10, 2);
                    cube.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
                }
            }
        }

    }
}