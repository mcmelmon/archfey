using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense : MonoBehaviour
{

    List<DefenseSpawnCircle> spawn_circles = new List<DefenseSpawnCircle>();

    float delay = 5f;
    float count = 5f;
    Map map;
    Geography geography;
    Queue<GameObject> defenders = new Queue<GameObject>();
    List<GameObject> deployed = new List<GameObject>();


    // Unity


    private void Awake()
    {
        map = GetComponentInParent<World>().GetComponentInChildren<Map>();
        geography = GetComponentInParent<World>().GetComponentInChildren<Geography>();  // can't rely on map loading its geography first
    }

    void Update()
    {
        if (defenders.Count > 0)
        {
            if (delay <= 0f)
            {
                StartCoroutine(Wave());
                delay = count;
            }

            delay -= Time.deltaTime;
        }
    }


    // public


    public void Defend(Queue<GameObject> _defenders)
    {
        defenders = _defenders;
        DeployDefense();
    }


    // private


    private void DeployDefense()
    {
        DefenseSpawnCircle spawn_circle = new DefenseSpawnCircle();
        Vector3 edge_point = geography.RandomLocation();
        Vector3 circle_center = geography.PointBetween(edge_point, geography.GetCenter(), 0.15f, true);

        spawn_circles.Add(spawn_circle.DrawCircle(circle_center));
    }


    private void Spawn()
    {
        int squad_size = defenders.Count / 5;
        // TODO: make squads real

        for (int i = 0; i < squad_size; i++)
        {
            GameObject _defenders = defenders.Dequeue();
            _defenders.transform.position = spawn_circles[0].RandomPointWithin();
            _defenders.SetActive(true);
            deployed.Add(_defenders);
        }
    }


    private IEnumerator Wave()
    {
        Spawn();
        yield return new WaitForSeconds(delay);
    }
}


public class DefenseSpawnCircle
{
    public Vector3 center;
    public List<Vector3> vertices = new List<Vector3>();
    public int vertex_count = 12;
    public int radius = 15;
    public float theta = 0f;
    public float delta_theta;

    public DefenseSpawnCircle DrawCircle(Vector3 _center)
    {
        center = _center;
        delta_theta = (2f * Mathf.PI) / vertex_count;

        for (int i = 0; i < vertex_count; i++)
        {
            Vector3 vertex = new Vector3(radius * Mathf.Cos(theta), 0f, radius * Mathf.Sin(theta));
            vertices.Add(center + vertex);
            theta += delta_theta;
        }

        return this;
    }


    public Vector3 RandomPointWithin()
    {
        if (center != null)
        {
            Vector3 point_3;
            Vector2 point_2 = new Vector2(center.x, center.z);
            Vector2 _center = new Vector2(center.x, center.z);

            point_2 = _center + Random.insideUnitCircle * radius;
            point_3 = new Vector3(point_2.x, 0, point_2.y);
            return point_3;
        }

        return Vector3.zero;
    }
}