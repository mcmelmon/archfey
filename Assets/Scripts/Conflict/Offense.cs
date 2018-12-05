using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Offense : MonoBehaviour 
{
    public enum Category { Primary = 0, Secondary = 1, Tertiary = 2 };

    readonly Dictionary<Category, Circle> attack_circles = new Dictionary<Category, Circle>();
    readonly List<GameObject> deployed = new List<GameObject>();

    Dictionary<Ruins.Category, Circle> ruin_circles = new Dictionary<Ruins.Category, Circle>();
    Geography geography;

    Queue<GameObject> aggressors = new Queue<GameObject>();
    List<GameObject> scouts = new List<GameObject>();
    List<GameObject> strikers = new List<GameObject>();
    List<GameObject> heavies = new List<GameObject>();

    // Unity


    private void Awake()
    {
        geography = GetComponentInParent<World>().GetComponentInChildren<Geography>();
        ruin_circles = GetComponentInParent<World>().GetComponentInChildren<Ruins>().GetOrCreateRuinCircles();
    }


    private void Start()
    {

    }

    private void Update()
    {

    }


    // public


    public void Attack(Queue<GameObject> _aggressors)
    {
        GameObject offense_parent = new GameObject();
        offense_parent.name = "Attack";
        offense_parent.AddComponent<Attack>();
        offense_parent.transform.parent = transform;
        aggressors = _aggressors;

        Locate();
        Deploy(offense_parent);
    }


    public Dictionary<Category, Circle> GetAttackCircles()
    {
        return attack_circles;
    }


    public Dictionary<Ruins.Category, Circle> GetRuinCircles()
    {
        return ruin_circles;
    }


    public List<GameObject> GetDeployed()
    {
        return deployed;
    }


    // private


    private void Deploy(GameObject parent)
    {
        foreach (KeyValuePair<Category, Circle> keyValue in attack_circles)
        {
            switch (keyValue.Key)
            {
                case Category.Primary:
                    for (int i = 0; i < 12; i++)
                    {
                        heavies.Add( Spawn(keyValue.Value.RandomContainedPoint(), parent.transform) );
                    }
                    break;
                case Category.Secondary:
                    for (int i = 0; i < 5; i++)
                    {
                        GameObject _striker = Spawn(keyValue.Value.RandomContainedPoint(), parent.transform);
                        _striker.AddComponent<Striker>();
                        strikers.Add(_striker);
                    }
                    break;
                case Category.Tertiary:
                    for (int i = 0; i < 3; i++)
                    {
                        GameObject _scout = Spawn(keyValue.Value.RandomContainedPoint(), parent.transform);
                        _scout.AddComponent<Scout>();
                        scouts.Add(_scout);
                    }
                    break;
            }
        }

        Formation strike_formation = Formation.CreateFormation(attack_circles[Category.Secondary].center, 10f, Formation.Profile.Round);
        foreach (var striker in strikers)
        {
            strike_formation.JoinFormation(striker);
        }
    }


    private void Locate()
    {
        if (geography == null) geography = GetComponentInParent<World>().GetComponentInChildren<Geography>();
        LocatePrimaryAttack();
        LocateSecondaryAttack();
        LocateTertiaryAttack();
    }


    private void LocatePrimaryAttack()
    {
        float distance_from_edge_percent = 0.15f;
        bool grounded = true;
        Vector3 circle_center = geography.PointBetween(geography.RandomBorderLocation(), geography.GetCenter(), distance_from_edge_percent, grounded);
        Circle attack_circle = Circle.CreateCircle(circle_center, 10f);

        attack_circles[Category.Primary] = attack_circle;
    }


    private void LocateSecondaryAttack()
    {
        float distance_from_edge_percent = 0.1f;
        bool grounded = true;
        Vector3 circle_center = geography.PointBetween(geography.RandomBorderLocation(), geography.GetCenter(), distance_from_edge_percent, grounded);
        Circle attack_circle = Circle.CreateCircle(circle_center, 5f);

        attack_circles[Category.Secondary] = attack_circle;
    }


    private void LocateTertiaryAttack()
    {
        float distance_from_edge_percent = 0.1f;
        bool grounded = true;
        Vector3 circle_center = geography.PointBetween(geography.RandomBorderLocation(), geography.GetCenter(), distance_from_edge_percent, grounded);
        Circle attack_circle = Circle.CreateCircle(circle_center, 5f);

        attack_circles[Category.Tertiary] = attack_circle;
    }


    private GameObject Spawn(Vector3 point, Transform offense_parent)
    {
        GameObject _aggressor = aggressors.Dequeue();
        _aggressor.transform.position = point;
        _aggressor.AddComponent<Attack>();
        _aggressor.transform.parent = offense_parent;
        _aggressor.SetActive(true);
        deployed.Add(_aggressor);
        return _aggressor;
    }
}