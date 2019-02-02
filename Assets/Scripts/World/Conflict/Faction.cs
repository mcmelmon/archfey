using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Faction : MonoBehaviour
{
    [Serializable]
    public struct FactionUnit {
        public string name;
        public GameObject prefab;
    }

    // Inspector settings
    public Conflict.Alignment alignment;
    public List<FactionUnit> faction_units;

    // properties

    public static List<GameObject> Units { get; set; }

    // Unity


    private void Awake()
    {
        SetComponents();
    }


    // public


    public void Reinforce()
    {

        foreach (var structure in Residential()) {
            foreach (var entrance in structure.entrances) {
                if (structure.AttachedUnits.Count >= structure.entrances.Count) break;

                Vector3 location = entrance.position;
                Actor commoner;
                int roll = UnityEngine.Random.Range(0, 3);

                // artisans will only be regenerated when storage facilities report materials available
                // TODO: commoners really only make sense for "human-type" objectives
                switch (roll) {
                    case 0:
                        commoner = SpawnToolUser("Farmer", entrance);
                        break;
                    case 1:
                        commoner = SpawnToolUser("Lumberjack", entrance);
                        structure.AttachedUnits.Add(commoner);
                        break;
                    case 2:
                        commoner = SpawnToolUser("Miner", entrance);
                        structure.AttachedUnits.Add(commoner);
                        break;
                    default:
                        // roughy 12% chance that we don't replenish commoners every reinforce
                        break;
                }
            }
        }

        foreach (var structure in Military()) {
            foreach (var entrance in structure.entrances) {
                Vector3 location = entrance.transform.position;
                GameObject military_unit = Spawn(Structure.Purpose.Military, new Vector3(location.x, Geography.Terrain.SampleHeight(location), location.z));
                Actor actor = military_unit.GetComponent<Actor>();
                actor.Alignment = alignment;
                structure.AttachedUnits.Add(actor);
            }
        }


        foreach (var structure in Sacred()) {
            foreach (var entrance in structure.entrances) {
                Vector3 location = entrance.transform.position;
                GameObject sacred_unit = Spawn(Structure.Purpose.Sacred, new Vector3(location.x, Geography.Terrain.SampleHeight(location), location.z));
                Actor actor = sacred_unit.GetComponent<Actor>();
                actor.Alignment = alignment;
                structure.AttachedUnits.Add(actor);
            }
        }
    }


    public Actor SpawnToolUser(string tool, Transform location)
    {
        GameObject residential_unit = Spawn(Structure.Purpose.Residential, new Vector3(location.position.x, Geography.Terrain.SampleHeight(location.position), location.position.z));
        residential_unit.AddComponent<Commoner>();
        residential_unit.GetComponent<Stats>().Tools.Add(tool);
        Actor actor = residential_unit.GetComponent<Actor>();
        actor.Alignment = alignment;

        return actor;
    }


    // private


    private List<Structure> Military()
    {
        return FindObjectsOfType<Structure>()
                .Where(s => s.alignment == alignment && s.purpose == Structure.Purpose.Military && s.AttachedUnits.Count < s.entrances.Count)
                .ToList();
    }


    private List<Structure> Residential()
    {
        return FindObjectsOfType<Structure>()
                .Where(s => s.alignment == alignment && s.purpose == Structure.Purpose.Residential && s.AttachedUnits.Count < s.entrances.Count)
                .ToList();
    }


    private List<Structure> Sacred()
    {
        return FindObjectsOfType<Structure>()
                .Where(s => s.alignment == alignment && s.purpose == Structure.Purpose.Sacred && s.AttachedUnits.Count < s.entrances.Count)
                .ToList();
    }


    private void SetComponents()
    {
        Units = new List<GameObject>();
    }


    private GameObject Spawn(Structure.Purpose purpose, Vector3 location)
    {
        GameObject new_unit = Instantiate(faction_units.First(unit => unit.name == purpose.ToString()).prefab, location, Civilization.Instance.actor_prefab.transform.rotation);
        new_unit.transform.parent = transform;
        Units.Add(new_unit);
        return new_unit;
    }
}
