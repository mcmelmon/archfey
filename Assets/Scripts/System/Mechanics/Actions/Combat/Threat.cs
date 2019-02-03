using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Threat : MonoBehaviour {

    public enum TargetPreference { Damaging, Nearest, Weakest };

    // properties

    public Actor Me { get; set; }
    public Dictionary<Actor, float> Threats { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
        StartCoroutine(DecayThreat());
    }


    // public


    public void AddThreat(Actor _attacker, float _damage)
    {
        AddPersonalThreat(_attacker, _damage);
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


    public Actor Weakest()
    {
        return Threats.Count > 0 ? Threats.OrderBy(threat => threat.Key.Health.CurrentHitPoints).First().Key : null;
    }


    // private


    private void AddPersonalThreat(Actor _attacker, float _damage)
    {
        if (!Threats.ContainsKey(_attacker)) {
            Threats[_attacker] = _damage;
        } else {
            Threats[_attacker] += _damage;
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
        Threats = new Dictionary<Actor, float>();
    }
}
