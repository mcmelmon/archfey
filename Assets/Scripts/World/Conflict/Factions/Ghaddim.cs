using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghaddim : MonoBehaviour {

    public Ghaddim ghaddim_prefab;
    public Dictionary<string, float> starting_health = new Dictionary<string, float>();
    public Dictionary<string, float> recovery_rate = new Dictionary<string, float>();
    readonly Dictionary<GameObject, float> faction_threats = new Dictionary<GameObject, float>();


    // Unity


    private void Awake()
    {
        // TODO: make these enums
        starting_health["scout"] = 100f;
        starting_health["striker"] = 130f;
        starting_health["heavy"] = 160f;

        recovery_rate["scout"] = 0.05f;
        recovery_rate["striker"] = 0.075f;
        recovery_rate["heavy"] = 0.1f;
    }


    // public


    public void AddFactionThreat(GameObject _foe, float _threat)
    {
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

        if (unit.GetComponent<Scout>() != null)
        {
            health.SetStartingHealth(starting_health["scout"]);
            health.SetRecoveryRate(recovery_rate["scout"]);
        }
        else if (unit.GetComponent<Striker>() != null)
        {
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
        Ghaddim _ghaddim = Instantiate(ghaddim_prefab, ghaddim_prefab.transform.position, ghaddim_prefab.transform.rotation);
        _ghaddim.gameObject.AddComponent<Soldier>();

        return _ghaddim.gameObject;
    }
}