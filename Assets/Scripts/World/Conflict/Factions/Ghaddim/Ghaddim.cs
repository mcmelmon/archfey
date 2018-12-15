using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghaddim : MonoBehaviour {

    public static Dictionary<GameObject, float> faction_threats = new Dictionary<GameObject, float>();

    public Ghaddim ghaddim_prefab;


    // Unity


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


    public void SetStats()
    {
        SetDefenseStats();
        SetHealthStats();
    }


    public GameObject SpawnUnit()
    {
        Ghaddim _ghaddim = Instantiate(ghaddim_prefab, ghaddim_prefab.transform.position, ghaddim_prefab.transform.rotation);
        _ghaddim.gameObject.AddComponent<Soldier>();

        return _ghaddim.gameObject;
    }


    // private


    private void SetDefenseStats()
    {
        Defend defend = GetComponent<Defend>();
        if (defend == null) return;

        if (GetComponent<Heavy>() != null) {
            defend.SetAgilityRating(ConfigureGhaddim.agility_rating["heavy"]);
            defend.SetArmorRating(ConfigureGhaddim.armor_rating["heavy"]);
            defend.SetCorporealRating(ConfigureGhaddim.corporeal_rating["heavy"]);
            defend.SetCounter(ConfigureGhaddim.counter["heavy"]);
            defend.SetForceRating(ConfigureGhaddim.force_rating["heavy"]);
            defend.SetResistances(ConfigureGhaddim.resistances["heavy"]);
        } else if (GetComponent<Scout>() != null) {
            defend.SetAgilityRating(ConfigureGhaddim.agility_rating["scout"]);
            defend.SetArmorRating(ConfigureGhaddim.armor_rating["scout"]);
            defend.SetCorporealRating(ConfigureGhaddim.corporeal_rating["scout"]);
            defend.SetCounter(ConfigureGhaddim.counter["scout"]);
            defend.SetForceRating(ConfigureGhaddim.force_rating["scout"]);
            defend.SetResistances(ConfigureGhaddim.resistances["scout"]);
        } else if (GetComponent<Striker>() != null) {
            defend.SetAgilityRating(ConfigureGhaddim.agility_rating["striker"]);
            defend.SetArmorRating(ConfigureGhaddim.armor_rating["striker"]);
            defend.SetCorporealRating(ConfigureGhaddim.corporeal_rating["striker"]);
            defend.SetCounter(ConfigureGhaddim.counter["striker"]);
            defend.SetForceRating(ConfigureGhaddim.force_rating["striker"]);
            defend.SetResistances(ConfigureGhaddim.resistances["striker"]);
        }
    }


    private void SetHealthStats()
    {
        Health health = GetComponent<Health>();
        if (health == null) return;

        if (GetComponent<Scout>() != null) {
            health.SetStartingHealth(ConfigureGhaddim.starting_health["scout"]);
            health.SetRecoveryRate(ConfigureGhaddim.recovery_rate["scout"]);
        } else if (GetComponent<Striker>() != null) {
            health.SetStartingHealth(ConfigureGhaddim.starting_health["striker"]);
            health.SetRecoveryRate(ConfigureGhaddim.recovery_rate["striker"]);
        } else if (GetComponent<Heavy>() != null) {
            health.SetStartingHealth(ConfigureGhaddim.starting_health["heavy"]);
            health.SetRecoveryRate(ConfigureGhaddim.recovery_rate["heavy"]);
        }
    }
}