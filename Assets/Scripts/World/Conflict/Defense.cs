using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense : MonoBehaviour
{

    public static Defense defense_instance;
    public static Conflict.Faction faction;
    public static List<GameObject> units = new List<GameObject>();

    Geography geography;
    Ghaddim ghaddim;
    Mhoddim mhoddim;
    Ruins ruins;


    // Unity


    private void Awake()
    {
        if (defense_instance != null)
        {
            Debug.LogError("More than one defense instance");
            Destroy(this);
            return;
        }
        defense_instance = this;
        SetComponents();
    }


    // public


    public void Setup()
    {
        Deploy();
    }


    public List<GameObject> GetUnits()
    {
        return units;
    }


    // private


    private void Deploy()
    {
        foreach (KeyValuePair<Ruins.Category, Circle> circle in Ruins.ruin_circles) {
            switch (circle.Key) {
                case Ruins.Category.Primary:
                    Formation block_formation = Formation.CreateFormation(circle.Value.center, Formation.Profile.Circle);

                    for (int i = 0; i < 14; i++) {
                        GameObject _heavy = Spawn(circle.Value.RandomContainedPoint());
                        _heavy.AddComponent<Heavy>();
                        block_formation.JoinFormation(_heavy);
                        _heavy.GetComponent<Soldier>().SetFormation(block_formation);
                    }
                    break;
                case Ruins.Category.Secondary:
                    Formation strike_formation = Formation.CreateFormation(circle.Value.center, Formation.Profile.Rectangle);

                    for (int i = 0; i < 6; i++) {
                        GameObject _striker = Spawn(circle.Value.RandomContainedPoint());
                        _striker.AddComponent<Striker>();
                        strike_formation.JoinFormation(_striker);
                        _striker.GetComponent<Soldier>().SetFormation(strike_formation);

                    }
                    break;
            }
        }
    }


    private void SetComponents()
    {
        ruins = GetComponentInParent<World>().GetComponentInChildren<Ruins>();
        geography = GetComponentInParent<World>().GetComponentInChildren<Geography>();
        ghaddim = GetComponentInParent<Ghaddim>();
        mhoddim = GetComponentInParent<Mhoddim>();
        faction = (ghaddim != null) ? Conflict.Faction.Ghaddim : Conflict.Faction.Mhoddim;
    }


    private GameObject Spawn(Vector3 point)
    {
        GameObject _soldier = (ghaddim != null) ? ghaddim.SpawnUnit() : mhoddim.SpawnUnit();
        _soldier.transform.position = point;
        _soldier.transform.parent = transform;
        _soldier.GetComponent<Actor>().role = Conflict.Role.Defense;
        units.Add(_soldier);
        return _soldier;
    }
}