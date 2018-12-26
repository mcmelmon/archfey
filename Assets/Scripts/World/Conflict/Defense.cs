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

        // spawn the defense randomly, give them time to claim some ruins, then spawn offense
        int adjustment = (Faction == Conflict.Faction.Ghaddim) ? 1 : 0;

        for (int i = 0; i < 10 + adjustment; i++) {
            MapTile tile = Geography.Instance.RandomUnoccupiedTile();
            GameObject _heavy = Spawn(tile.Location);
            _heavy.AddComponent<Heavy>();
            tile.Occupier = _heavy.GetComponent<Actor>();
        }
    }


    // private


    private void SetComponents()
    {
        Units = new List<GameObject>();
    }


    private GameObject Spawn(Vector3 _point)
    {
        GameObject _soldier = (Faction == Conflict.Faction.Ghaddim) ? Ghaddim.SpawnUnit(_point) : Mhoddim.SpawnUnit(_point);
        _soldier.transform.parent = transform;
        _soldier.GetComponent<Actor>().Role = Conflict.Role.Defense;
        Units.Add(_soldier);
        Conflict.Units.Add(_soldier);
        return _soldier;
    }
}