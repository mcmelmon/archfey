using System;
using System.Collections.Generic;
using UnityEngine;

public class Circle {

    // properties

    public Vector3 Center { get; set; }
    public float DeltaTheta { get; set; }
    public float Radius { get; set; }
    public float Theta { get; set; }
    public int VertexCount { get; set; }
    public List<Vector3> Vertices { get; set; }


    // static


    public static Circle New(Vector3 center, float radius, int vertices = 12, bool draw_vertices = false)
    {
        Circle _circle = new Circle
        {
            Center = center,
            Radius = radius,
            VertexCount = vertices,
            Vertices = new List<Vector3>()
    };
        _circle.DeltaTheta = (2f * Mathf.PI) / _circle.VertexCount;
        _circle.Draw(draw_vertices);

        return _circle;
    }


    // public


    public bool Equals(Circle other_circle)
    {
        return Center == other_circle.Center && Mathf.Approximately(Radius, other_circle.Radius);
    }


    public Vector3 RandomContainedPoint()
    {
        if (Center == Vector3.zero) return Vector3.zero;

        UnityEngine.Random.InitState(DateTime.Now.Millisecond);

        Vector3 point_3;
        Vector2 point_2 = new Vector2(Center.x, Center.z);
        Vector2 _center = new Vector2(Center.x, Center.z);

        point_2 = _center + UnityEngine.Random.insideUnitCircle * Radius;
        point_3 = new Vector3(point_2.x, Center.y, point_2.y);
        //point_3 = new Vector3(point_2.x, Geography.Terrain.SampleHeight(new Vector3(point_2.x, 0, point_2.y)), point_2.y);
        return point_3;
    }


    public Vector3 RandomVertex()
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);

        return Vertices[UnityEngine.Random.Range(0, Vertices.Count)];
    }


    public Vector3 VertexClosestTo(Vector3 point)
    {
        float shortest_distance = Mathf.Infinity;
        Vector3 nearest = Vector3.zero;

        foreach (var vertex in Vertices) {
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
        Center = _center;
        Radius = _radius;
        VertexCount = _vertex_count;
        Vertices.Clear();
        Draw(draw_vertices);
    }


    // private


    private void Draw(bool draw_vertices)
    {
        for (int i = 0; i < VertexCount; i++) {
            Vector3 vertex = new Vector3(Radius * Mathf.Cos(Theta), 0f, Radius * Mathf.Sin(Theta));
            if (draw_vertices) {
                GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
                marker.name = "Vertex";
                marker.transform.position = (Center + vertex);
                marker.transform.localScale = new Vector3(1, 10, 1);
            }
            Vertices.Add(Center + vertex);
            Theta += DeltaTheta;
        }
    }
}