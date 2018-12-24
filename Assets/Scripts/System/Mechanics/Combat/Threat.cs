using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Threat : MonoBehaviour {

    Dictionary<GameObject, float> threats = new Dictionary<GameObject, float>();


    // Unity

    private void Awake()
    {
        StartCoroutine(DecayThreat());
    }


    // public


    public void AddThreat(GameObject _attacker, float _damage)
    {

        if (!threats.ContainsKey(_attacker) ) {
            threats[_attacker] = _damage;
        } else {
            threats[_attacker] += _damage;
        }
    }


    public GameObject BiggestThreat()
    {
        GameObject biggest_threat = null;
        float value = 0f;

        foreach (KeyValuePair<GameObject, float> threat in threats) {
            if (threat.Key == null) continue;  // don't modify the dictionary by removing while iterating
            if (threat.Value > value) {
                value = threat.Value;
                biggest_threat = threat.Key;
            }
        }

        return biggest_threat;
    }


    public Dictionary<GameObject, float> GetThreats()
    {
        return threats;
    }


    public bool IsAThreat(GameObject _unit)
    {
        return threats.ContainsKey(_unit);
    }


    public void SpreadThreat(GameObject _attacker, float _damage)
    {
        Conflict.Faction _faction = GetComponent<Actor>().Faction;

        if (_faction == Conflict.Faction.Ghaddim) {
            GetComponent<Ghaddim>().AddFactionThreat(_attacker, _damage);
        } else if (_faction == Conflict.Faction.Mhoddim) {
            GetComponent<Mhoddim>().AddFactionThreat(_attacker, _damage);
        }
    }


    // private


    private IEnumerator DecayThreat()
    {
        while (true) {
            yield return new WaitForSeconds(Turn.action_threshold);
            var keys = threats.Keys.ToArray();

            for (int i = 0; i < keys.Length; i++) {
                // use for loop to avoid modifying collection in foreach
                threats[keys[i]] -= 20;  // TODO: configure by unit; base on average weapon damage
                if (threats[keys[i]] <= 0) threats.Remove(keys[i]);
            }
        }
    }
}
