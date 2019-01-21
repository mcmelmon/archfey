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


    public void Deploy()
    {
        // must be called by Conflict instead of Start to ensure Map setup complete

        //var military = FindObjectsOfType<Structure>()
        //    .Where(s => s.purpose == Structure.Purpose.Military && s.owner == Conflict.Faction.Ghaddim);

        //foreach (var structure in military) {
        //    foreach (var entrance in structure.entrances)
        //    {
        //        Vector3 location = entrance.transform.position;
        //        GameObject gnoll = Spawn(new Vector3(location.x, Geography.Terrain.SampleHeight(location), location.z));
        //        gnoll.AddComponent<Gnoll>();
        //    }
        //}
    }


    public Actor SpawnToolUser(Proficiencies.Tool _tool, Transform _entrance)
    {
        GameObject commoner = Spawn(new Vector3(_entrance.position.x, Geography.Terrain.SampleHeight(_entrance.position), _entrance.position.z));
        commoner.AddComponent<Commoner>();
        commoner.GetComponent<Stats>().Tools.Add(_tool);

        return commoner.GetComponent<Actor>();
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