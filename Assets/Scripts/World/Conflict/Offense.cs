using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Offense : MonoBehaviour 
{
    public enum Category { Primary = 0, Secondary = 1, Tertiary = 2 };

    readonly Dictionary<Category, Circle> attack_circles = new Dictionary<Category, Circle>();
    Dictionary<Ruins.Category, Circle> ruin_circles = new Dictionary<Ruins.Category, Circle>();
    private Queue<GameObject> aggressors = new Queue<GameObject>();
    readonly List<GameObject> deployed = new List<GameObject>();
    readonly List<Formation> formations = new List<Formation>();

    Geography geography;

    // Unity


    private void Awake()
    {
        geography = GetComponentInParent<World>().GetComponentInChildren<Geography>();
    }


    private void Update()
    {
        // TODO: units will move in accordance with their turns; or their formation turns
    }


    // public


    public void Attack(Queue<GameObject> _aggressors)
    {
        GameObject offense_parent = new GameObject {name = "Offense"};
        offense_parent.transform.parent = transform;
        aggressors = _aggressors;

        Locate();
        Deploy(offense_parent);
    }


    public void CommandFormations()
    {
        foreach (var formation in formations)
        {
            if (!formation.has_objective) formation.Strategize();
        }
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


    public List<Formation> GetFormations()
    {
        return formations;
    }


    // private


    private void Deploy(GameObject parent)
    {
        foreach (KeyValuePair<Category, Circle> keyValue in attack_circles)
        {
            switch (keyValue.Key)
            {
                case Category.Primary:
                    Formation block_formation = Formation.CreateFormation(attack_circles[Category.Primary].center, Formation.Profile.Rectangle, 3f);
                    formations.Add(block_formation);

                    for (int i = 0; i < 12; i++)
                    {
                        GameObject _heavy = Spawn(keyValue.Value.RandomContainedPoint(), parent.transform);
                        _heavy.AddComponent<Heavy>();
                        block_formation.JoinFormation(_heavy);
                        _heavy.GetComponent<Heavy>().SetFormation(block_formation);
                    }
                    break;
                case Category.Secondary:
                    Formation strike_formation = Formation.CreateFormation(attack_circles[Category.Secondary].center, Formation.Profile.Rectangle);
                    formations.Add(strike_formation);

                    for (int i = 0; i < 5; i++)
                    {
                        GameObject _striker = Spawn(keyValue.Value.RandomContainedPoint(), parent.transform);
                        _striker.AddComponent<Striker>();
                        strike_formation.JoinFormation(_striker);
                        _striker.GetComponent<Striker>().SetFormation(strike_formation);
                    }
                    break;
                case Category.Tertiary:
                    for (int i = 0; i < 3; i++)
                    {
                        GameObject _scout = Spawn(keyValue.Value.RandomContainedPoint(), parent.transform);
                        _scout.AddComponent<Scout>();
                    }
                    break;
            }
        }
    }


    private void Locate()
    {
        if (geography == null) geography = GetComponentInParent<World>().GetComponentInChildren<Geography>();
        ruin_circles = GetComponentInParent<World>().GetComponentInChildren<Ruins>().GetOrCreateRuinCircles();

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
        _aggressor.AddComponent<Attacker>();
        _aggressor.transform.parent = offense_parent;
        _aggressor.SetActive(true);
        deployed.Add(_aggressor);
        return _aggressor;
    }
}