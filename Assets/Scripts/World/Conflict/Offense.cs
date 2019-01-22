using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Offense : MonoBehaviour
{
    // properties

    public Conflict.Faction Faction { get; set; }
    public static Offense Instance { get; set; }
    public static List<GameObject> Units { get; set; }


    // Unity


    private void Awake()
    {
        if (Instance != null) {
            Debug.LogError("More than one offense instance");
            Destroy(this);
            return;
        }
        Instance = this;
        SetComponents();
    }


    // public


    public void Reinforce()
    {
        List<Structure> residences = FindObjectsOfType<Structure>()
            .Where(s => s.owner == Conflict.Faction.Ghaddim && s.purpose == Structure.Purpose.Residential && s.AttachedUnits.Count < s.entrances.Count)
            .ToList();

        List<Structure> military = FindObjectsOfType<Structure>()
            .Where(s => s.owner == Conflict.Faction.Ghaddim && s.purpose == Structure.Purpose.Military && s.AttachedUnits.Count < s.entrances.Count)
            .ToList();

        foreach (var structure in residences) {
            foreach (var entrance in structure.entrances) {
                Vector3 location = entrance.position;
                Actor commoner;
                int roll = Random.Range(0, 3);

                // artisans will only be regenerated when storage facilities report materials available
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
                }
            }
        }

        foreach (var structure in military) {
            foreach (var entrance in structure.entrances) {
                if (structure.AttachedUnits.Count >= structure.entrances.Count) break;
                Vector3 location = entrance.position;
                GameObject gnoll = Spawn(new Vector3(location.x, Geography.Terrain.SampleHeight(location), location.z));
                gnoll.AddComponent<Gnoll>();
                structure.AttachedUnits.Add(gnoll.GetComponent<Actor>());
            }
        }
    }


    public Actor SpawnToolUser(string _tool, Transform _location)
    {
        GameObject commoner = Spawn(new Vector3(_location.position.x, Geography.Terrain.SampleHeight(_location.position), _location.position.z));
        commoner.AddComponent<Commoner>();
        commoner.GetComponent<Commoner>().Post = _location;
        commoner.GetComponent<Stats>().Tools.Add(_tool);
        Actor _actor = commoner.GetComponent<Actor>();

        return _actor;
    }


    // private


    private void SetComponents()
    {
        Units = new List<GameObject>();
    }


    private GameObject Spawn(Vector3 _point)
    {
        GameObject _soldier = (Faction == Conflict.Faction.Ghaddim) ? Ghaddim.SpawnUnit(_point) : Mhoddim.SpawnUnit(_point);  // offense will almost always be Ghaddim
        _soldier.transform.parent = transform;
        _soldier.GetComponent<Actor>().Role = Conflict.Role.Offense;
        Units.Add(_soldier);
        Conflict.Units.Add(_soldier);
        return _soldier;
    }
}