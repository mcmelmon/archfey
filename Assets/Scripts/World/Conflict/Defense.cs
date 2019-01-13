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

        foreach (var objective in Objectives.Instance.objectives) {
            if (objective.Claim == Conflict.Faction.Mhoddim) {
                for (int i = 0; i < 0; i++) {
                    Circle spawn_circle = Circle.New(objective.claim_nodes[0].transform.position, 5);
                    Vector3 _point = spawn_circle.RandomContainedPoint();
                    GameObject commoner = Spawn(new Vector3(_point.x, objective.claim_nodes[0].transform.position.y, _point.z));
                    commoner.AddComponent<Commoner>();
                }

                for (int i = 0; i < 0; i++)
                {
                    Circle spawn_circle = Circle.New(objective.claim_nodes[0].transform.position, 3);
                    Vector3 _point = spawn_circle.RandomContainedPoint();
                    GameObject guard = Spawn(new Vector3(_point.x, objective.claim_nodes[0].transform.position.y, _point.z));
                    guard.AddComponent<Guard>();
                }
            }
        }
    }


    public void Reinforce()
    {
        foreach (var objective in Objectives.Instance.objectives) {
            if (objective.Claim == Conflict.Faction.Mhoddim) {
                for (int i = 0; i < 0; i++) {
                    Circle spawn_circle = Circle.New(objective.claim_nodes[0].transform.position, 3);
                    Vector3 _point = spawn_circle.RandomContainedPoint();
                    GameObject guard = Spawn(new Vector3(_point.x, objective.claim_nodes[0].transform.position.y, _point.z));
                    guard.AddComponent<Guard>();
                }
            }
        }
    }


    // private


    private void SetComponents()
    {
        Units = new List<GameObject>();
    }


    private GameObject Spawn(Vector3 _point)
    {
        GameObject _soldier = (Faction == Conflict.Faction.Ghaddim) ? Ghaddim.SpawnUnit(_point) : Mhoddim.SpawnUnit(_point);  // Defense will "almost always" be Mhoddim...
        _soldier.transform.parent = transform;
        _soldier.GetComponent<Actor>().Role = Conflict.Role.Defense;
        Units.Add(_soldier);
        Conflict.Units.Add(_soldier);
        return _soldier;
    }
}