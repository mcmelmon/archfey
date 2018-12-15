using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Offense : MonoBehaviour
{
    public static Offense offense_instance;
    public static Dictionary<Soldier.Clasification, Circle> attack_circles = new Dictionary<Soldier.Clasification, Circle>();
    public static List<GameObject> soldiers = new List<GameObject>();

    Geography geography;
    Ghaddim ghaddim;
    Mhoddim mhoddim;

    // Unity


    private void Awake()
    {
        if (offense_instance != null)
        {
            Debug.LogError("More than one offense instance");
            Destroy(this);
            return;
        }
        offense_instance = this;
        SetComponents();
    }


    private void Start()
    {
        Debug.Log("Starting offense");
    }


    // public


    public void Setup()
    {
        Locate();
        Deploy();
    }


    // private


    private void Deploy()
    {
        foreach (KeyValuePair<Soldier.Clasification, Circle> circle in attack_circles) {
            switch (circle.Key) {
                case Soldier.Clasification.Heavy:
                    Formation block_formation = Formation.CreateFormation(circle.Value.center, Formation.Profile.Rectangle, 3f);

                    for (int i = 0; i < 12; i++) {
                        GameObject _heavy = Spawn(circle.Value.RandomContainedPoint());
                        _heavy.AddComponent<Heavy>();
                        block_formation.JoinFormation(_heavy);
                        _heavy.GetComponent<Soldier>().SetFormation(block_formation);
                    }
                    break;
                case Soldier.Clasification.Striker:
                    Formation strike_formation = Formation.CreateFormation(circle.Value.center, Formation.Profile.Rectangle);

                    for (int i = 0; i < 5; i++) {
                        GameObject _striker = Spawn(circle.Value.RandomContainedPoint());
                        _striker.AddComponent<Striker>();
                        strike_formation.JoinFormation(_striker);
                        _striker.GetComponent<Soldier>().SetFormation(strike_formation);
                    }
                    break;
                case Soldier.Clasification.Scout:
                    for (int i = 0; i < 3; i++) {
                        GameObject _scout = Spawn(circle.Value.RandomContainedPoint());
                        _scout.AddComponent<Scout>();
                    }
                    break;
            }
        }
    }


    private void Locate()
    {
        if (geography == null) geography = GetComponentInParent<World>().GetComponentInChildren<Geography>();

        LocateHeavy();
        LocateStriker();
        LocateScout();
    }


    private void LocateHeavy()
    {
        float distance_from_edge_percent = 0.15f;
        bool grounded = true;
        Vector3 circle_center = geography.PointBetween(geography.RandomBorderLocation(), geography.GetCenter(), distance_from_edge_percent, grounded);
        Circle attack_circle = Circle.CreateCircle(circle_center, 15f);

        attack_circles[Soldier.Clasification.Heavy] = attack_circle;
    }


    private void LocateStriker()
    {
        float distance_from_edge_percent = 0.1f;
        bool grounded = true;
        Vector3 circle_center = geography.PointBetween(geography.RandomBorderLocation(), geography.GetCenter(), distance_from_edge_percent, grounded);
        Circle attack_circle = Circle.CreateCircle(circle_center, 15f);

        attack_circles[Soldier.Clasification.Striker] = attack_circle;
    }


    private void LocateScout()
    {
        float distance_from_edge_percent = 0.1f;
        bool grounded = true;
        Vector3 circle_center = geography.PointBetween(geography.RandomBorderLocation(), geography.GetCenter(), distance_from_edge_percent, grounded);
        Circle attack_circle = Circle.CreateCircle(circle_center, 15f);

        attack_circles[Soldier.Clasification.Scout] = attack_circle;
    }


    private void SetComponents()
    {
        geography = GetComponentInParent<World>().GetComponentInChildren<Geography>();
        ghaddim = GetComponentInParent<Ghaddim>();
        mhoddim = GetComponentInParent<Mhoddim>();
    }


    private GameObject Spawn(Vector3 point)
    {
        GameObject _soldier = (ghaddim != null) ? ghaddim.SpawnUnit() : mhoddim.SpawnUnit();
        _soldier.transform.position = point;
        _soldier.AddComponent<Attacker>();
        _soldier.transform.parent = transform;
        soldiers.Add(_soldier);
        return _soldier;
    }
}