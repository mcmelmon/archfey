using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Offense : MonoBehaviour {

    List<Circle> spawn_circles = new List<Circle>();

    float delay = 5f;
    float count = 5f;
    Map map;
    Geography geography;
    Queue<GameObject> aggressors = new Queue<GameObject>();
    List<GameObject> deployed = new List<GameObject>();


    // Unity


    private void Awake()
    {
        map = GetComponentInParent<World>().GetComponentInChildren<Map>();
        geography = GetComponentInParent<World>().GetComponentInChildren<Geography>();  // can't rely on map loading its geography first
    }

    void Update()
    {
        if (aggressors.Count > 0)
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


    public void Attack(Queue<GameObject> _aggressors)
    {
        aggressors = _aggressors;
        DeployOffense();
    }


    // private


    private void DeployOffense()
    {
        Circle spawn_circle = new Circle();
        Vector3 edge_point = geography.RandomBorderLocation();
        Vector3 circle_center = geography.PointBetween(edge_point, geography.GetCenter(), 0.15f, true);

        spawn_circles.Add(spawn_circle.Inscribe(circle_center, 12f));
    }


    private void Spawn()
    {
        int squad_size = aggressors.Count / 5;
        // TODO: make squads real

        for (int i = 0; i < squad_size; i++)
        {
            GameObject _aggressor = aggressors.Dequeue();
            _aggressor.transform.position = spawn_circles[0].RandomContainedPoint();
            _aggressor.SetActive(true);
            deployed.Add(_aggressor);
        }
    }


    private IEnumerator Wave()
    {
        Spawn();
        yield return new WaitForSeconds(delay);
    }
}