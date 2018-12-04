using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle {

    public Vector3 center;
    public List<Vector3> vertices = new List<Vector3>();
    public int vertex_count;
    public float radius;
    public float theta = 0f;
    public float delta_theta;


    // public


    public static Circle CreateCircle(Vector3 center, float radius, int vertices = 12, bool draw_vertices = false)
    {
        Circle _circle = new Circle();
        _circle.center = center;
        _circle.radius = radius;
        _circle.vertex_count = vertices;
        _circle.delta_theta = (2f * Mathf.PI) / _circle.vertex_count;

        _circle.Draw(draw_vertices);

        return _circle;
    }


    public Vector3 RandomContainedPoint()
    {
        if (center == Vector3.zero) return Vector3.zero;

        Vector3 point_3;
        Vector2 point_2 = new Vector2(center.x, center.z);
        Vector2 _center = new Vector2(center.x, center.z);

        point_2 = _center + Random.insideUnitCircle * radius;
        point_3 = new Vector3(point_2.x, 0, point_2.y);
        return point_3;
    }


    public Vector3 RandomVertex()
    {
        return vertices[Random.Range(0, vertices.Count)];
    }


    public Vector3 VertexClosestTo(Vector3 point)
    {
        float shortest_distance = Mathf.Infinity;
        Vector3 nearest = Vector3.zero;

        foreach (var vertex in vertices)
        {
            float distance = Vector3.Distance(vertex, point);
            if (distance < shortest_distance) {
                shortest_distance = distance;
                nearest = vertex;
            }
        }

        return nearest;
    }


    public void Redraw(Vector3 _center, float _radius, int _vertex_count = 12, bool draw_vertices = false)
    {
        center = _center;
        radius = _radius;
        vertex_count = _vertex_count;
        vertices.Clear();
        Draw(draw_vertices);
    }


    // private


    private void Draw(bool draw_vertices)
    {
        for (int i = 0; i < vertex_count; i++)
        {
            Vector3 vertex = new Vector3(radius * Mathf.Cos(theta), 0f, radius * Mathf.Sin(theta));
            if (draw_vertices)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.name = "Vertex";
                cube.transform.position = (center + vertex);
                cube.transform.localScale = new Vector3(2, 10, 2);
            }
            vertices.Add(center + vertex);
            theta += delta_theta;
        }
    }
}