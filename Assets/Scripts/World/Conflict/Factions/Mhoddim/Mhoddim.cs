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
            defend.SetAgilityRating(ConfigureMhoddim.agility_rating["heavy"]);
            defend.SetArmorRating(ConfigureMhoddim.armor_rating["heavy"]);
            defend.SetCorporealRating(ConfigureMhoddim.corporeal_rating["heavy"]);
            defend.SetCounter(ConfigureMhoddim.counter["heavy"]);
            defend.SetForceRating(ConfigureMhoddim.force_rating["heavy"]);
            defend.SetResistances(ConfigureMhoddim.resistances["heavy"]);
        } else if (GetComponent<Scout>() != null) {
            defend.SetAgilityRating(ConfigureMhoddim.agility_rating["scout"]);
            defend.SetArmorRating(ConfigureMhoddim.armor_rating["scout"]);
            defend.SetCorporealRating(ConfigureMhoddim.corporeal_rating["scout"]);
            defend.SetCounter(ConfigureMhoddim.counter["scout"]);
            defend.SetForceRating(ConfigureMhoddim.force_rating["scout"]);
            defend.SetResistances(ConfigureMhoddim.resistances["scout"]);
        } else if (GetComponent<Striker>() != null) {
            defend.SetAgilityRating(ConfigureMhoddim.agility_rating["striker"]);
            defend.SetArmorRating(ConfigureMhoddim.armor_rating["striker"]);
            defend.SetCorporealRating(ConfigureMhoddim.corporeal_rating["striker"]);
            defend.SetCounter(ConfigureMhoddim.counter["striker"]);
            defend.SetForceRating(ConfigureMhoddim.force_rating["striker"]);
            defend.SetResistances(ConfigureMhoddim.resistances["striker"]);
        }
    }


    private void SetHealthStats()
    {
        Health health = GetComponent<Health>();
        if (health == null) return;

        if (GetComponent<Scout>() != null) {
            health.SetStartingHealth(ConfigureMhoddim.starting_health["scout"]);
            health.SetRecoveryRate(ConfigureMhoddim.recovery_rate["scout"]);
        } else if (GetComponent<Striker>() != null) {
            health.SetStartingHealth(ConfigureMhoddim.starting_health["striker"]);
            health.SetRecoveryRate(ConfigureMhoddim.recovery_rate["striker"]);
        } else if (GetComponent<Heavy>() != null) {
            health.SetStartingHealth(ConfigureMhoddim.starting_health["heavy"]);
            health.SetRecoveryRate(ConfigureMhoddim.recovery_rate["heavy"]);
        }
    }
}