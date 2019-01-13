using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Threat : MonoBehaviour {

    // properties

    public Actor Me { get; set; }
    public static Dictionary<Actor, List<Actor>> Marks { get; set; }
    public Dictionary<Actor, float> Threats { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
        StartCoroutine(ClearMarks());
        StartCoroutine(DecayThreat());
    }


    // public


    public void AddThreat(Actor _attacker, float _damage)
    {
        AddPersonalThreat(_attacker, _damage);
        AddMark(_attacker);
    }


    public Actor BiggestThreat()
    {
        return MostDamaging() ?? Nearest();
    }


    public bool IsAThreat(Actor _unit)
    {
        return Threats.ContainsKey(_unit);
    }


    public Actor MostDamaging()
    {
        Actor most_damaging = null;
        float value = 0f;

        foreach (KeyValuePair<Actor, float> threat in Threats)
        {
            if (threat.Key == null) continue;  // don't modify the dictionary by removing while iterating
            if (threat.Value > value)
            {
                value = threat.Value;
                most_damaging = threat.Key;
            }
        }

        return most_damaging;
    }


    public Actor Nearest()
    {
        Actor nearest_enemy = null;
        float shortest_distance = float.MaxValue;
        float distance;

        for (int i = 0; i < Me.Actions.Decider.Enemies.Count; i++)
        {
            Actor enemy = Me.Actions.Decider.Enemies[i];
            if (enemy == null) continue;
            if (transform == null) break;

            distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < shortest_distance)
            {
                shortest_distance = distance;
                nearest_enemy = enemy;
            }
        }

        return nearest_enemy;
    }


    public void SpreadThreat(Actor _attacker, float _damage)
    {
        Conflict.Faction _faction = GetComponentInParent<Actor>().Faction;

        if (_faction == Conflict.Faction.Ghaddim) {
            GetComponentInParent<Ghaddim>().AddFactionThreat(_attacker, _damage);
        } else if (_faction == Conflict.Faction.Mhoddim) {
            GetComponentInParent<Mhoddim>().AddFactionThreat(_attacker, _damage);
        }
    }


    // private


    private void AddMark(Actor _attacker)
    {
        if (!Marks.ContainsKey(_attacker))
        {
            Marks[_attacker] = new List<Actor>() { Me };
        }
        else
        {
            if (!Marks[_attacker].Contains(Me)) Marks[_attacker].Add(Me);
        }
    }


    private void AddPersonalThreat(Actor _attacker, float _damage)
    {
        if (!Threats.ContainsKey(_attacker)) {
            Threats[_attacker] = _damage;
        } else {
            Threats[_attacker] += _damage;
        }
    }


    private IEnumerator ClearMarks()
    {
        while (true) {
            var keys = Marks.Keys.ToArray();

            for (int i = 0; i < keys.Length; i++)
            {
                // use for loop to avoid modifying collection in foreach
                if (Marks[keys[i]] == null) Marks.Remove(keys[i]);
            }
            yield return new WaitForSeconds(Turn.ActionThreshold);
        }
    }


    private IEnumerator DecayThreat()
    {
        while (true) {
            yield return new WaitForSeconds(Turn.ActionThreshold);
            var keys = Threats.Keys.ToArray();

            for (int i = 0; i < keys.Length; i++) {
                // use for loop to avoid modifying collection in foreach
                Threats[keys[i]]--;
                if (Threats[keys[i]] <= 0) Threats.Remove(keys[i]);
            }
        }
    }


    private void SetComponents()
    {
        Me = GetComponentInParent<Actor>();
        Marks = new Dictionary<Actor, List<Actor>>();
        Threats = new Dictionary<Actor, float>();
    }
}
