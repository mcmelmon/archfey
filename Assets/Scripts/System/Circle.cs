using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle {

    public Vector3 center;
    public List<Vector3> vertices = new List<Vector3>();
    public int vertex_count = 12;
    public float radius = 0f;
    public float theta = 0f;
    public float delta_theta;


    public static Circle CreateCircle(Vector3 center, float radius)
    {
        Circle _circle = new Circle();
        _circle.center = center;
        _circle.radius = radius;

        _circle.delta_theta = (2f * Mathf.PI) / _circle.vertex_count;

        for (int i = 0; i < _circle.vertex_count; i++)
        {
            Vector3 vertex = new Vector3(radius * Mathf.Cos(_circle.theta), 0f, radius * Mathf.Sin(_circle.theta));
            _circle.vertices.Add(center + vertex);
            _circle.theta += _circle.delta_theta;
        }

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
}