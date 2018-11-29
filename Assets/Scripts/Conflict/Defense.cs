using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense : MonoBehaviour
{

    List<Circle> spawn_circles = new List<Circle>();

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
        Circle spawn_circle = new Circle();
        Vector3 edge_point = geography.RandomLocation();
        Vector3 circle_center = geography.PointBetween(edge_point, geography.GetCenter(), 0.15f, true);

        spawn_circles.Add(spawn_circle.Inscribe(circle_center, 8f));
    }


    private void Spawn()
    {
        int squad_size = defenders.Count / 5;
        // TODO: make squads real

        for (int i = 0; i < squad_size; i++)
        {
            GameObject _defenders = defenders.Dequeue();
            _defenders.transform.position = spawn_circles[0].RandomContainedPoint();
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