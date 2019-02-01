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
        List<Structure> military = FindObjectsOfType<Structure>()
            .Where(s => s.owner == Conflict.Faction.Ghaddim && s.purpose == Structure.Purpose.Military && s.AttachedUnits.Count < s.entrances.Count)
            .ToList();

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


    // private


    private void SetComponents()
    {
        Units = new List<GameObject>();
    }


    private GameObject Spawn(Vector3 _point)
    {
        GameObject new_unit = Instantiate(Civilization.Instance.actor_prefab, _point, Civilization.Instance.actor_prefab.transform.rotation);
        new_unit.transform.parent = transform;
        new_unit.GetComponent<Actor>().Role = Conflict.Role.Offense;
        Units.Add(new_unit);
        return new_unit;
    }
}