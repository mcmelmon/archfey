using System;
using System.Collections;
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
    public string identifier;
    public Conflict.Alignment alignment;
    public List<Faction> allies;
    public List<Faction> rivals;

    // properties

    public List<Actor> Units { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
        StartCoroutine(PruneUnits());
    }


    // public


    public bool IsHostileTo(Faction other_faction)
    {
        if (other_faction == null || other_faction.identifier == identifier) return false;

        List<Faction> rival_factions = rivals.Select(rival => rival.GetComponent<Faction>()).ToList();
        bool alignment_hostility = Conflict.Instance.AlignmentAntagonisms(alignment).Contains(other_faction.alignment);
        bool rival_hostility = rivals.Any() && rival_factions.Contains(other_faction);

        return alignment_hostility || rival_hostility;
    }


    // private


    private IEnumerator PruneUnits()
    {
        while (true) {
            for (int i = 0; i < Units.Count; i++) {
                if (Units[i] == null) Units.Remove(Units[i]);
            }
            yield return null;
        }
    }


    private void SetComponents()
    {
        Units = new List<Actor>();
    }
}
