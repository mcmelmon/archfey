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
    public string faction_name;
    public Conflict.Alignment alignment;
    public List<string> allied_factions;
    public List<string> rival_factions;
    public List<FactionUnit> faction_units;

    // properties

    public static List<Actor> Units { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    // public


    public bool IsHostileTo(Faction other_faction)
    {
        if (other_faction == this) return false;

        switch (alignment) {
            case Conflict.Alignment.Evil:
                return !allied_factions.Contains(other_faction.name);
            case Conflict.Alignment.Good:
                return rival_factions.Contains(other_faction.name) || other_faction.alignment == Conflict.Alignment.Evil;
            case Conflict.Alignment.Neutral:
                return rival_factions.Contains(other_faction.name);
            case Conflict.Alignment.Unaligned:
                return true;
        }
        return true; // in case of doubt, shoot
    }


    public void Reinforce()
    {

        foreach (var structure in Residential()) {
            foreach (var entrance in structure.entrances) {
                if (structure.AttachedUnits.Count >= structure.entrances.Count) break;

                Vector3 location = entrance.position;
                Actor commoner;
                int roll = UnityEngine.Random.Range(0, 9);

                // artisans will only be regenerated when storage facilities report materials available
                // TODO: commoners really only make sense for "human-type" objectives
                switch (roll) {
                    case 0:
                        commoner = SpawnToolUser("Farmer", entrance);
                        structure.AttachedUnits.Add(commoner);
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
                Actor military_unit = Spawn(Structure.Purpose.Military, new Vector3(location.x, Geography.Terrain.SampleHeight(location), location.z));
                structure.AttachedUnits.Add(military_unit);
            }
        }


        foreach (var structure in Sacred()) {
            foreach (var entrance in structure.entrances) {
                Vector3 location = entrance.transform.position;
                Actor sacred_unit = Spawn(Structure.Purpose.Sacred, new Vector3(location.x, Geography.Terrain.SampleHeight(location), location.z));
                structure.AttachedUnits.Add(sacred_unit);
            }
        }
    }


    public Actor SpawnToolUser(string tool, Transform location)
    {
        Actor residential_unit = Spawn(Structure.Purpose.Residential, new Vector3(location.position.x, Geography.Terrain.SampleHeight(location.position), location.position.z));
        residential_unit.gameObject.AddComponent<Commoner>();
        residential_unit.GetComponent<Stats>().Tools.Add(tool);
        return residential_unit;
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
        Units = new List<Actor>();
    }


    private Actor Spawn(Structure.Purpose purpose, Vector3 location)
    {
        GameObject new_unit = Instantiate(faction_units.First(unit => unit.name == purpose.ToString()).prefab, location, Civilization.Instance.actor_prefab.transform.rotation);
        new_unit.transform.parent = transform;
        Actor actor = new_unit.GetComponent<Actor>();
        actor.Alignment = alignment;
        actor.Faction = this;
        Units.Add(actor);
        return actor;
    }
}
