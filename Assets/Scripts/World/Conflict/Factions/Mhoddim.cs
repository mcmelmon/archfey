using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mhoddim : MonoBehaviour {

    public Mhoddim mhoddim_prefab;
    readonly Dictionary<string, float> starting_health = new Dictionary<string, float>();
    readonly Dictionary<string, float> recovery_rate = new Dictionary<string, float>();
    readonly Dictionary<GameObject, float> faction_threats = new Dictionary<GameObject, float>();


    // Unity


    private void Awake()
    {
        starting_health["scout"] = 70f;
        starting_health["striker"] = 100f;
        starting_health["heavy"] = 130f;

        recovery_rate["scout"] = 0f;
        recovery_rate["striker"] = 0f;
        recovery_rate["heavy"] = 0f;
    }


    // public


    public void AddFactionThreat(GameObject _foe, float _threat)
    {
        // TODO: differentiate between how Ghaddim and Mhoddim perceive threats

        if (!faction_threats.ContainsKey(_foe)) {
            faction_threats[_foe] = _threat;
        } else {
            faction_threats[_foe] += _threat;
        }
    }


    public bool IsFactionThreat(GameObject _sighting)
    {
        return faction_threats.ContainsKey(_sighting);
    }


    public bool SetHealthStats(GameObject unit)
    {
        Health health = unit.GetComponent<Health>();
        if (health == null) return false;

        if (unit.GetComponent<Scout>() != null) {
            health.SetStartingHealth(starting_health["scout"]);
            health.SetRecoveryRate(recovery_rate["scout"]);
        }
        else if (unit.GetComponent<Striker>() != null) {
            health.SetStartingHealth(starting_health["striker"]);
            health.SetRecoveryRate(recovery_rate["striker"]);
        }
        else if (unit.GetComponent<Heavy>() != null)
        {
            health.SetStartingHealth(starting_health["heavy"]);
            health.SetRecoveryRate(recovery_rate["heavy"]);
        } else {
            return false;
        }

        return true;
    }


    public GameObject SpawnUnit()
    {
        Mhoddim _mhoddim = Instantiate(mhoddim_prefab, mhoddim_prefab.transform.position, mhoddim_prefab.transform.rotation);
        _mhoddim.gameObject.AddComponent<Soldier>();

        return _mhoddim.gameObject;
    }
}