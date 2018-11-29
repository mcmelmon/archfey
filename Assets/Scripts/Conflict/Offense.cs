using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Offense : MonoBehaviour {

    float delay = 5f;
    float count = 5f;
    Map map;
    Geography geography;
    Queue<GameObject> aggressors = new Queue<GameObject>();
    List<GameObject> deployed = new List<GameObject>();

    Dictionary<string, Circle> attack_circles = new Dictionary<string, Circle>();


    // Unity


    private void Awake()
    {
        map = GetComponentInParent<World>().GetComponentInChildren<Map>();
        geography = GetComponentInParent<World>().GetComponentInChildren<Geography>();  // can't rely on map loading its geography first
    }


    private void Start()
    {
        LocateAttacks();
    }

    private void Update()
    {
        if (aggressors.Count > 0)
        {
            DeployOffense();
        }
    }


    // public


    public void Attack(Queue<GameObject> _aggressors)
    {
        aggressors = _aggressors;
    }


    // private


    private void DeployOffense()
    {
        foreach (KeyValuePair<string, Circle> keyValue in attack_circles)
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


    private void LocateAttacks()
    {
        LocatePrimaryAttack();
        LocateSecondaryAttack();
        LocateTertiaryAttack();
    }


    public void LocatePrimaryAttack()
    {
        Circle attack_circle = new Circle();
        Vector3 edge_point = geography.RandomBorderLocation();
        Vector3 circle_center = geography.PointBetween(edge_point, geography.GetCenter(), .5f, true);

        attack_circles["primary"] = attack_circle.Inscribe(circle_center, 40f);
    }


    public void LocateSecondaryAttack()
    {
        Circle attack_circle = new Circle();
        Vector3 edge_point = geography.RandomBorderLocation();
        Vector3 circle_center = geography.PointBetween(edge_point, geography.GetCenter(), .3f, true);

        attack_circles["secondary"] = attack_circle.Inscribe(circle_center, 20f);
    }


    public void LocateTertiaryAttack()
    {
        Circle attack_circle = new Circle();
        Vector3 edge_point = geography.RandomBorderLocation();
        Vector3 circle_center = geography.PointBetween(edge_point, geography.GetCenter(), .1f, true);

        attack_circles["tertiary"] = attack_circle.Inscribe(circle_center, 12f);
    }


    private void Spawn(Vector3 point)
    {
        GameObject _aggressor = aggressors.Dequeue();
        _aggressor.transform.position = GetComponentInParent<Conflict>().ClearSpawn(point, _aggressor);
        _aggressor.SetActive(true);
        deployed.Add(_aggressor);
    }
}