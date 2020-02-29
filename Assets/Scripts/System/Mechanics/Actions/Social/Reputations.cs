using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reputations : MonoBehaviour
{
    // properties

    public Actor Me { get; set; }
    public Dictionary<Faction, int> Factions { get; set; }
    public Dictionary<Actor, int> Individuals { get; set; }
    public Dictionary<Plot, int> Plots { get; set; } 

    // Unity

    private void Awake() {
        SetComponents();
    }

    public void ChangeReputationFor(Actor _actor, int _amount)
    {
        if (Individuals.ContainsKey(_actor)) {
            Individuals[_actor] += _amount;
        } else {
            Individuals[_actor] = _amount;
        }
    }

    public void ChangeReputationFor(Faction _faction, int _amount)
    {
        if (Factions.ContainsKey(_faction)) {
            Factions[_faction] += _amount;
        } else {
            Factions[_faction] = _amount;
        }
    }

    public void ChangeReputationFor(Plot _plot, int _amount)
    {
        if (Plots.ContainsKey(_plot)) {
            Plots[_plot] += _amount;
        } else {
            Plots[_plot] = _amount;
        }
    }

    public int GetReputationFor(Actor _actor)
    {
        if (!Individuals.ContainsKey(_actor)) {
            ChangeReputationFor(_actor, 0);
        }

        return Individuals[_actor];
    }

    public int GetReputationFor(Faction _faction)
    {
        if (!Factions.ContainsKey(_faction)) {
            ChangeReputationFor(_faction, 0);
        }

        return Factions[_faction];
    }

    public int GetReputationFor(Plot _plot)
    {
        if (!Plots.ContainsKey(_plot)) {
            ChangeReputationFor(_plot, 0);
        }

        return Plots[_plot];
    }

    // private

    private void SetComponents()
    {
        Me = GetComponent<Actor>();
        Factions = new Dictionary<Faction, int>();
        Individuals = new Dictionary<Actor, int>();
        Plots = new Dictionary<Plot, int>();
    }
}
