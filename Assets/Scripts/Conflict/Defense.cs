using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense : MonoBehaviour
{

    Map map;
    Geography geography;
    Queue<GameObject> defenders = new Queue<GameObject>();
    List<GameObject> deployed = new List<GameObject>();
    Dictionary<string, Circle> ruin_circles = new Dictionary<string, Circle>();

    // Unity


    private void Awake()
    {
        map = GetComponentInParent<World>().GetComponentInChildren<Map>();
        geography = GetComponentInParent<World>().GetComponentInChildren<Geography>();  // can't rely on map loading its geography first
    }


    private void Start()
    {

    }

    private void Update()
    {
        if (ruin_circles.Count <= 0) {
            ruin_circles = GetComponentInParent<World>().GetComponentInChildren<Ruins>().GetRuinCircles();
        } else if (defenders.Count > 0) {
            DeployDefense();
        }
    }


    // public


    public void Defend(Queue<GameObject> _defenders)
    {
        defenders = _defenders;
    }


    // private


    private void DeployDefense()
    {
        foreach (KeyValuePair<string, Circle> keyValue in ruin_circles)
        {
            switch (keyValue.Key)
            {
                case "primary":
                    for (int i = 0; i < 12; i++)
                    {
                        Spawn(keyValue.Value.RandomContainedPoint());
                    }
                    break;
                case "secondary":
                    for (int i = 0; i < 5; i++)
                    {
                        Spawn(keyValue.Value.RandomContainedPoint());
                    }
                    break;
                case "tertiary":
                    for (int i = 0; i < 3; i++)
                    {
                        Spawn(keyValue.Value.RandomContainedPoint());
                    }
                    break;
            }
        }
    }


    private void Spawn(Vector3 point)
    {
        GameObject _defender = defenders.Dequeue();
        _defender.transform.position = GetComponentInParent<Conflict>().ClearSpawn(point, _defender);
        _defender.SetActive(true);
        deployed.Add(_defender);
    }
}