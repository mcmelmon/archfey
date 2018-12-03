using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense : MonoBehaviour
{

    Queue<GameObject> defenders = new Queue<GameObject>();
    readonly List<GameObject> deployed = new List<GameObject>();
    Dictionary<string, Circle> ruin_circles = new Dictionary<string, Circle>();

    // Unity


    private void Awake()
    {

    }


    private void Start()
    {

    }

    private void Update()
    {

    }


    // public


    public void Defend(Queue<GameObject> _defenders)
    {
        defenders = _defenders;
        ruin_circles = GetComponentInParent<World>().GetComponentInChildren<Ruins>().GetOrCreateRuinCircles();
        Deploy();
    }


    // private


    private void Deploy()
    {
        GameObject defense_parent = new GameObject();
        defense_parent.name = "Defend";
        defense_parent.AddComponent<Defend>();
        defense_parent.transform.parent = transform;

        foreach (KeyValuePair<string, Circle> keyValue in ruin_circles)
        {
            switch (keyValue.Key)
            {
                case "primary":
                    for (int i = 0; i < 12; i++)
                    {
                        Spawn(keyValue.Value.RandomContainedPoint(), defense_parent.transform);
                    }
                    break;
                case "secondary":
                    for (int i = 0; i < 5; i++)
                    {
                        Spawn(keyValue.Value.RandomContainedPoint(), defense_parent.transform);
                    }
                    break;
                case "tertiary":
                    for (int i = 0; i < 3; i++)
                    {
                        Spawn(keyValue.Value.RandomContainedPoint(), defense_parent.transform);
                    }
                    break;
            }
        }
    }


    private void Spawn(Vector3 point, Transform defense_parent)
    {
        GameObject _defender = defenders.Dequeue();
        _defender.transform.position = point;
        _defender.AddComponent<Defend>();
        _defender.transform.parent = defense_parent;
        _defender.SetActive(true);
        deployed.Add(_defender);
    }
}