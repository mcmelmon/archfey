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
    public string identifier;
    public Color colors;
    public Conflict.Alignment alignment;
    public List<Faction> allies;
    public List<Faction> rivals;


    // public


    public bool IsHostileTo(Faction other_faction)
    {
        if (other_faction == null || other_faction.identifier == identifier) return false;

        List<Faction> rival_factions = rivals.Select(rival => rival.GetComponent<Faction>()).ToList();
        bool alignment_hostility = Conflict.Instance.AlignmentAntagonisms(alignment).Contains(other_faction.alignment);
        bool rival_hostility = rivals.Any() && rival_factions.Contains(other_faction);

        return alignment_hostility || rival_hostility;
    }


    public void LoseTotalObjectiveControl()
    {
        List<Actor> faction_units = FindObjectsOfType<Actor>()
            .Where(actor => actor != null && actor.CurrentFaction == this)
            .ToList();

        for (int i = 0; i < faction_units.Count; i++) {
            faction_units[i].Actions.Decider.AchievedAllObjectives = false;
        }
    }
}
