using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense : MonoBehaviour
{

    // properties

    public Conflict.Faction Faction { get; set; }
    public static Defense Instance { get; set; }
    public static List<GameObject> Units { get; set; }

    // Unity


    private void Awake()
    {
        if (Instance != null) {
            Debug.LogError("More than one defense instance");
            Destroy(this);
            return;
        }
        Instance = this;
        SetComponents();
    }


    // public


    public void Deploy()
    {
        // must be called by Conflict instead of Start to ensure Map setup complete

        foreach (KeyValuePair<Ruins.Category, Circle> circle in Ruins.Circles) {
            switch (circle.Key) {
                case Ruins.Category.Primary:
                    Formation block_formation = Formation.CreateFormation(circle.Value.center, Formation.Profile.Circle);

                    for (int i = 0; i < 10; i++) {
                        GameObject _heavy = Spawn(circle.Value.RandomContainedPoint());
                        _heavy.AddComponent<Heavy>();
                        block_formation.JoinFormation(_heavy);
                        _heavy.GetComponent<Soldier>().SetFormation(block_formation);
                    }
                    break;
                //case Ruins.Category.Secondary:
                    //Formation strike_formation = Formation.CreateFormation(circle.Value.center, Formation.Profile.Rectangle);

                    //for (int i = 0; i < 6; i++) {
                    //    GameObject _striker = Spawn(circle.Value.RandomContainedPoint());
                    //    _striker.AddComponent<Striker>();
                    //    strike_formation.JoinFormation(_striker);
                    //    _striker.GetComponent<Soldier>().SetFormation(strike_formation);

                    //}
                    //break;
            }
        }
    }


    // private


    private void SetComponents()
    {
        Units = new List<GameObject>();
    }


    private GameObject Spawn(Vector3 point)
    {
        GameObject _soldier = (Faction == Conflict.Faction.Ghaddim) ? Ghaddim.SpawnUnit() : Mhoddim.SpawnUnit();
        _soldier.transform.position = point;
        _soldier.transform.parent = transform;
        _soldier.GetComponent<Actor>().Role = Conflict.Role.Defense;
        Units.Add(_soldier);
        Conflict.Units.Add(_soldier);
        return _soldier;
    }
}