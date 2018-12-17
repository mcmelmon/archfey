using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mhoddim : MonoBehaviour {

    public static Dictionary<GameObject, float> faction_threats = new Dictionary<GameObject, float>();

    public Mhoddim mhoddim_prefab;


    // Unity


    // public


    public void AddFactionThreat(GameObject _foe, float _threat)
    {
        // TODO: differentiate between how Ghaddim and Mhoddim perceive threats
        // TODO: handle destroyed object as key

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
        Mhoddim _mhoddim = Instantiate(mhoddim_prefab, mhoddim_prefab.transform.position, mhoddim_prefab.transform.rotation);
        _mhoddim.gameObject.AddComponent<Soldier>();

        return _mhoddim.gameObject;
    }


    // private


    private void SetDefenseStats()
    {
        Defend defend = GetComponent<Defend>();
        if (defend == null) return;

        if (GetComponent<Heavy>() != null) {
            defend.SetAgilityRating(ConfigureMhoddim.agility_rating[Soldier.Clasification.Heavy]);
            defend.SetArmorRating(ConfigureMhoddim.armor_rating[Soldier.Clasification.Heavy]);
            defend.SetCorporealRating(ConfigureMhoddim.corporeal_rating[Soldier.Clasification.Heavy]);
            defend.SetCounter(ConfigureMhoddim.counter[Soldier.Clasification.Heavy]);
            defend.SetForceRating(ConfigureMhoddim.force_rating[Soldier.Clasification.Heavy]);
            defend.SetResistances(ConfigureMhoddim.resistances[Soldier.Clasification.Heavy]);
        } else if (GetComponent<Scout>() != null) {
            defend.SetAgilityRating(ConfigureMhoddim.agility_rating[Soldier.Clasification.Scout]);
            defend.SetArmorRating(ConfigureMhoddim.armor_rating[Soldier.Clasification.Scout]);
            defend.SetCorporealRating(ConfigureMhoddim.corporeal_rating[Soldier.Clasification.Scout]);
            defend.SetCounter(ConfigureMhoddim.counter[Soldier.Clasification.Scout]);
            defend.SetForceRating(ConfigureMhoddim.force_rating[Soldier.Clasification.Scout]);
            defend.SetResistances(ConfigureMhoddim.resistances[Soldier.Clasification.Scout]);
        } else if (GetComponent<Striker>() != null) {
            defend.SetAgilityRating(ConfigureMhoddim.agility_rating[Soldier.Clasification.Striker]);
            defend.SetArmorRating(ConfigureMhoddim.armor_rating[Soldier.Clasification.Striker]);
            defend.SetCorporealRating(ConfigureMhoddim.corporeal_rating[Soldier.Clasification.Striker]);
            defend.SetCounter(ConfigureMhoddim.counter[Soldier.Clasification.Striker]);
            defend.SetForceRating(ConfigureMhoddim.force_rating[Soldier.Clasification.Striker]);
            defend.SetResistances(ConfigureMhoddim.resistances[Soldier.Clasification.Striker]);
        }
    }


    private void SetHealthStats()
    {
        Health health = GetComponent<Health>();
        if (health == null) return;

        if (GetComponent<Heavy>() != null) {
            health.SetStartingHealth(ConfigureMhoddim.starting_health[Soldier.Clasification.Heavy]);
            health.SetRecoveryRate(ConfigureMhoddim.recovery_rate[Soldier.Clasification.Heavy]);
        } else if (GetComponent<Scout>() != null) {
            health.SetStartingHealth(ConfigureMhoddim.starting_health[Soldier.Clasification.Scout]);
            health.SetRecoveryRate(ConfigureMhoddim.recovery_rate[Soldier.Clasification.Scout]);
        } else if (GetComponent<Striker>() != null) {
            health.SetStartingHealth(ConfigureMhoddim.starting_health[Soldier.Clasification.Striker]);
            health.SetRecoveryRate(ConfigureMhoddim.recovery_rate[Soldier.Clasification.Striker]);
        }
    }
}