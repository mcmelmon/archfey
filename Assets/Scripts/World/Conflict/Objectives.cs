using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Objectives : MonoBehaviour {

    // Inspector settings
    public List<Objective> objectives;

    // properties

    public static Dictionary<Conflict.Faction, List<Objective>> HeldByFaction { get; set; }
    public static Objectives Instance { get; set; }


    // Unity


    private void Awake()
    {
        if (Instance != null) {
            Debug.LogError("More than one ruins instance");
            Destroy(this);
            return;
        }
        Instance = this;
    }


    // public


    public void AccountForClaim(Conflict.Faction new_faction, Conflict.Faction previous_faction, Objective _objective)
    {
        if (!HeldByFaction[new_faction].Contains(_objective)) {
            HeldByFaction[new_faction].Add(_objective);
            HeldByFaction[previous_faction].Remove(_objective);
        }
    }


    public void PlaceObjectives()
    {
        SetComponents();
    }


    public Objective ObjectiveNearest(Vector3 location)
    {
        return FindObjectsOfType<Objective>().OrderBy(o => Vector3.Distance(o.transform.position, location)).First();
    }


    // private


    private void SetComponents()
    {
        HeldByFaction = new Dictionary<Conflict.Faction, List<Objective>>
        {
            [Conflict.Faction.Ghaddim] = new List<Objective>(),
            [Conflict.Faction.Mhoddim] = new List<Objective>(),
            [Conflict.Faction.None] = new List<Objective>()
        };
        foreach (var objective in objectives) {
            HeldByFaction[objective.Claim].Add(objective);
        }
    }
}