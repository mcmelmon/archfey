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

        var farmers = FindObjectsOfType<Structure>()
            .Where(s => s.Wants().Contains(Resources.Type.Farm) && s.owner == Conflict.Faction.Mhoddim);

        var miners = FindObjectsOfType<Structure>()
            .Where(s => (s.Wants().Contains(Resources.Type.Copper) || s.Wants().Contains(Resources.Type.Iron) || s.Wants().Contains(Resources.Type.Gold) && s.owner == Conflict.Faction.Mhoddim));

        var woodcutters = FindObjectsOfType<Structure>()
            .Where(s => s.Wants().Contains(Resources.Type.Lumber) && s.owner == Conflict.Faction.Mhoddim);

        var military = FindObjectsOfType<Structure>()
            .Where(s => s.purpose == Structure.Purpose.Military && s.owner == Conflict.Faction.Mhoddim);

        foreach (var structure in farmers) {
            foreach (var entrance in structure.entrances) {
                Vector3 location = entrance.transform.position;
                GameObject commoner = Spawn(new Vector3(location.x, Geography.Terrain.SampleHeight(location), location.z));
                commoner.AddComponent<Commoner>();
                commoner.GetComponent<Stats>().Skills.Add(Characters.SkillAttributes.First(sa => sa.skill == Characters.Skill.Farmer));
            }
        }

        foreach (var structure in miners) {
            foreach (var entrance in structure.entrances) {
                Vector3 location = entrance.transform.position;
                GameObject commoner = Spawn(new Vector3(location.x, Geography.Terrain.SampleHeight(location), location.z));
                commoner.AddComponent<Commoner>();
                commoner.GetComponent<Stats>().Skills.Add(Characters.SkillAttributes.First(sa => sa.skill == Characters.Skill.Miner));
            }
        }

        foreach (var structure in woodcutters) {
            foreach (var entrance in structure.entrances) {
                Vector3 location = entrance.transform.position;
                GameObject commoner = Spawn(new Vector3(location.x, Geography.Terrain.SampleHeight(location), location.z));
                commoner.AddComponent<Commoner>();
                commoner.gameObject.GetComponent<Stats>().Skills.Add(Characters.SkillAttributes.First(sa => sa.skill == Characters.Skill.Woodcutter));
            }
        }

        foreach (var structure in military) {
            foreach (var entrance in structure.entrances) {
                Vector3 location = entrance.transform.position;
                GameObject guard = Spawn(new Vector3(location.x, Geography.Terrain.SampleHeight(location), location.z));
                guard.AddComponent<Guard>();
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