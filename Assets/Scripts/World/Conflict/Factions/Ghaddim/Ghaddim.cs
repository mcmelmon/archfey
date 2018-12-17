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
            defend.SetAgilityRating(ConfigureGhaddim.agility_rating[Soldier.Clasification.Heavy]);
            defend.SetArmorRating(ConfigureGhaddim.armor_rating[Soldier.Clasification.Heavy]);
            defend.SetCorporealRating(ConfigureGhaddim.corporeal_rating[Soldier.Clasification.Heavy]);
            defend.SetCounter(ConfigureGhaddim.counter[Soldier.Clasification.Heavy]);
            defend.SetForceRating(ConfigureGhaddim.force_rating[Soldier.Clasification.Heavy]);
            defend.SetResistances(ConfigureGhaddim.resistances[Soldier.Clasification.Heavy]);
        } else if (GetComponent<Scout>() != null) {
            defend.SetAgilityRating(ConfigureGhaddim.agility_rating[Soldier.Clasification.Scout]);
            defend.SetArmorRating(ConfigureGhaddim.armor_rating[Soldier.Clasification.Scout]);
            defend.SetCorporealRating(ConfigureGhaddim.corporeal_rating[Soldier.Clasification.Scout]);
            defend.SetCounter(ConfigureGhaddim.counter[Soldier.Clasification.Scout]);
            defend.SetForceRating(ConfigureGhaddim.force_rating[Soldier.Clasification.Scout]);
            defend.SetResistances(ConfigureGhaddim.resistances[Soldier.Clasification.Scout]);
        } else if (GetComponent<Striker>() != null) {
            defend.SetAgilityRating(ConfigureGhaddim.agility_rating[Soldier.Clasification.Striker]);
            defend.SetArmorRating(ConfigureGhaddim.armor_rating[Soldier.Clasification.Striker]);
            defend.SetCorporealRating(ConfigureGhaddim.corporeal_rating[Soldier.Clasification.Striker]);
            defend.SetCounter(ConfigureGhaddim.counter[Soldier.Clasification.Striker]);
            defend.SetForceRating(ConfigureGhaddim.force_rating[Soldier.Clasification.Striker]);
            defend.SetResistances(ConfigureGhaddim.resistances[Soldier.Clasification.Striker]);
        }
    }


    private void SetHealthStats()
    {
        Health health = GetComponent<Health>();
        if (health == null) return;

        if (GetComponent<Heavy>() != null) {
            health.SetStartingHealth(ConfigureGhaddim.starting_health[Soldier.Clasification.Heavy]);
            health.SetRecoveryRate(ConfigureGhaddim.recovery_rate[Soldier.Clasification.Heavy]);
        } else if (GetComponent<Scout>() != null) {
            health.SetStartingHealth(ConfigureGhaddim.starting_health[Soldier.Clasification.Scout]);
            health.SetRecoveryRate(ConfigureGhaddim.recovery_rate[Soldier.Clasification.Scout]);
        } else if (GetComponent<Striker>() != null) {
            health.SetStartingHealth(ConfigureGhaddim.starting_health[Soldier.Clasification.Striker]);
            health.SetRecoveryRate(ConfigureGhaddim.recovery_rate[Soldier.Clasification.Striker]);
        } 
    }
}