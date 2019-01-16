using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        var non_military = FindObjectsOfType<Structure>()
            .Where(s => (s.purpose == Structure.Purpose.Residential || s.purpose == Structure.Purpose.Commercial || s.purpose == Structure.Purpose.Civic) && s.owner == Conflict.Faction.Mhoddim);

        var military = FindObjectsOfType<Structure>()
            .Where(s => s.purpose == Structure.Purpose.Military && s.owner == Conflict.Faction.Mhoddim);

        foreach (var structure in non_military) {
            foreach (var entrance in structure.entrances) {
                Vector3 location = entrance.transform.position;
                GameObject commoner = Spawn(new Vector3(location.x, Geography.Terrain.SampleHeight(location), location.z));
                commoner.AddComponent<Commoner>();
            }

        }

        foreach (var structure in military) {
            Vector3 location = structure.entrances[0].transform.position;
            GameObject guard = Spawn(new Vector3(location.x, Geography.Terrain.SampleHeight(location), location.z));
            guard.AddComponent<Guard>();
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