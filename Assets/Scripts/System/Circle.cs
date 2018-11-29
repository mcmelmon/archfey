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

    public Circle Inscribe(Vector3 _center, float _radius)
    {
        center = _center;
        radius = _radius;
        delta_theta = (2f * Mathf.PI) / vertex_count;

        for (int i = 0; i < vertex_count; i++)
        {
            Vector3 vertex = new Vector3(radius * Mathf.Cos(theta), 0f, radius * Mathf.Sin(theta));
            vertices.Add(center + vertex);
            theta += delta_theta;
        }

        return this;
    }


    public Vector3 RandomContainedPoint()
    {
        if (center == null) return Vector3.zero;

        Vector3 point_3;
        Vector2 point_2 = new Vector2(center.x, center.z);
        Vector2 _center = new Vector2(center.x, center.z);

        point_2 = _center + Random.insideUnitCircle * radius;
        point_3 = new Vector3(point_2.x, 0, point_2.y);
        return point_3;
    }
}