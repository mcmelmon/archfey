using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Threat : MonoBehaviour {

    public enum TargetPreference { Damaging, Nearest, Weakest };

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


    public bool IsAThreat(Actor _unit)
    {
        return Threats.ContainsKey(_unit);
    }


    public Actor MostDamaging()
    {
        return Threats.Count > 0 ? Threats.OrderBy(threat => threat.Value).Reverse().First().Key : null;
    }


    public Actor Nearest()
    {
        var enemies = Me.Senses.Actors.Where(a => !Me.Actions.Decider.IsFriendOrNeutral(a)).ToList();
        return enemies.Count > 0 ? enemies.OrderBy(enemy => Vector3.Distance(transform.position, enemy.transform.position)).First() : null;
    }


    public Actor PrimaryThreat(TargetPreference target_preference = TargetPreference.Weakest)
    {
        switch (target_preference) {
            case TargetPreference.Damaging:
                return MostDamaging() ?? Nearest();
            case TargetPreference.Nearest:
                return Nearest();
            case TargetPreference.Weakest:
                return Weakest() ?? Nearest();
        }

        return Nearest();
    }


    public void SpreadThreat(Actor _attacker, float _damage)
    {
        //Conflict.Alignment _faction = GetComponentInParent<Actor>().Alignment;

        //if (_faction == Conflict.Alignment.Evil) {
        //    GetComponentInParent<Ghaddim>().AddFactionThreat(_attacker, _damage);
        //} else if (_faction == Conflict.Alignment.Good) {
        //    GetComponentInParent<Mhoddim>().AddFactionThreat(_attacker, _damage);
        //}
    }


    public Actor Weakest()
    {
        return Threats.Count > 0 ? Threats.OrderBy(threat => threat.Key.Health.CurrentHitPoints).First().Key : null;
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
                if (keys[i] == null) { 
                    Threats.Remove(keys[i]);
                    continue;
                }
                Threats[keys[i]] -= 3;
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
