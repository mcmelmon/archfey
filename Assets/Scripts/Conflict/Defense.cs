﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense : MonoBehaviour
{

    Queue<GameObject> defenders = new Queue<GameObject>();
    readonly List<GameObject> deployed = new List<GameObject>();
    Dictionary<Ruins.Category, Circle> ruin_circles = new Dictionary<Ruins.Category, Circle>();
    readonly List<Formation> formations = new List<Formation>();


    // Unity


    // public


    public void Defend(Queue<GameObject> _defenders)
    {
        GameObject defense_parent = new GameObject {name = "Defend"};
        defense_parent.AddComponent<Defend>();
        defense_parent.transform.parent = transform;
        defenders = _defenders;
        ruin_circles = GetComponentInParent<World>().GetComponentInChildren<Ruins>().GetOrCreateRuinCircles();

        Deploy(defense_parent);
    }


    public Dictionary<Ruins.Category, Circle> GetRuinCircles()
    {
        return ruin_circles;
    }
                

    // private


    private void Deploy(GameObject parent)
    {
        foreach (KeyValuePair<Ruins.Category, Circle> keyValue in ruin_circles)
        {
            switch (keyValue.Key)
            {
                case Ruins.Category.Primary:
                    for (int i = 0; i < 12; i++)
                    {
                        Spawn(keyValue.Value.RandomContainedPoint(), parent.transform);
                    }
                    break;
                case Ruins.Category.Secondary:
                    Formation strike_formation = Formation.CreateFormation(ruin_circles[Ruins.Category.Secondary].center, Formation.Profile.Square);
                    formations.Add(strike_formation);

                    for (int i = 0; i < 5; i++)
                    {
                        GameObject _striker = Spawn(keyValue.Value.RandomContainedPoint(), parent.transform);
                        _striker.AddComponent<Striker>();
                        strike_formation.JoinFormation(_striker);
                        _striker.GetComponent<Striker>().SetFormation(strike_formation);

                    }
                    break;
                case Ruins.Category.Tertiary:
                    for (int i = 0; i < 3; i++)
                    {
                        GameObject _scout = Spawn(keyValue.Value.RandomContainedPoint(), parent.transform);
                        _scout.AddComponent<Scout>();
                    }
                    break;
            }
        }
    }


    private GameObject Spawn(Vector3 point, Transform defense_parent)
    {
        GameObject _defender = defenders.Dequeue();
        _defender.transform.position = point;
        _defender.AddComponent<Defend>();
        _defender.transform.parent = defense_parent;
        _defender.SetActive(true);
        deployed.Add(_defender);
        return _defender;
    }
}