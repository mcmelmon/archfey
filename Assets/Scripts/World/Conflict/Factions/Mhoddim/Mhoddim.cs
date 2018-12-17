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
        SetOffenseStats();
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
            defend.agility_rating = ConfigureMhoddim.agility_rating[Soldier.Clasification.Heavy];
            defend.armor_rating = ConfigureMhoddim.armor_rating[Soldier.Clasification.Heavy];
            defend.corporeal_rating = ConfigureMhoddim.corporeal_rating[Soldier.Clasification.Heavy];
            defend.counter = ConfigureMhoddim.counter[Soldier.Clasification.Heavy];
            defend.force_rating = ConfigureMhoddim.force_rating[Soldier.Clasification.Heavy];
            defend.SetResistances(ConfigureMhoddim.resistances[Soldier.Clasification.Heavy]);
        } else if (GetComponent<Scout>() != null) {
            defend.agility_rating = ConfigureMhoddim.agility_rating[Soldier.Clasification.Scout];
            defend.armor_rating = ConfigureMhoddim.armor_rating[Soldier.Clasification.Scout];
            defend.corporeal_rating = ConfigureMhoddim.corporeal_rating[Soldier.Clasification.Scout];
            defend.counter = ConfigureMhoddim.counter[Soldier.Clasification.Scout];
            defend.force_rating = ConfigureMhoddim.force_rating[Soldier.Clasification.Scout];
            defend.SetResistances(ConfigureMhoddim.resistances[Soldier.Clasification.Scout]);
        } else if (GetComponent<Striker>() != null) {
            defend.agility_rating = ConfigureMhoddim.agility_rating[Soldier.Clasification.Striker];
            defend.armor_rating = ConfigureMhoddim.armor_rating[Soldier.Clasification.Striker];
            defend.corporeal_rating = ConfigureMhoddim.corporeal_rating[Soldier.Clasification.Striker];
            defend.counter = ConfigureMhoddim.counter[Soldier.Clasification.Striker];
            defend.force_rating = ConfigureMhoddim.force_rating[Soldier.Clasification.Striker];
            defend.SetResistances(ConfigureMhoddim.resistances[Soldier.Clasification.Striker]);
        }
    }


    private void SetHealthStats()
    {
        Health health = GetComponent<Health>();
        if (health == null) return;

        if (GetComponent<Heavy>() != null) {
            health.SetStartingHealth(ConfigureMhoddim.starting_health[Soldier.Clasification.Heavy]);
            health.SetRecoveryAmount(ConfigureMhoddim.recovery_amount[Soldier.Clasification.Heavy]);
        } else if (GetComponent<Scout>() != null) {
            health.SetStartingHealth(ConfigureMhoddim.starting_health[Soldier.Clasification.Scout]);
            health.SetRecoveryAmount(ConfigureMhoddim.recovery_amount[Soldier.Clasification.Scout]);
        } else if (GetComponent<Striker>() != null) {
            health.SetStartingHealth(ConfigureMhoddim.starting_health[Soldier.Clasification.Striker]);
            health.SetRecoveryAmount(ConfigureMhoddim.recovery_amount[Soldier.Clasification.Striker]);
        }
    }


    private void SetOffenseStats()
    {
        Attack attack = GetComponent<Attack>();
        if (attack == null) return;

        if (GetComponent<Heavy>() != null) {
            attack.agility_rating = ConfigureMhoddim.starting_health[Soldier.Clasification.Heavy];
            attack.strength_rating = ConfigureMhoddim.starting_health[Soldier.Clasification.Heavy];
        } else if (GetComponent<Scout>() != null) {
            attack.agility_rating = ConfigureMhoddim.starting_health[Soldier.Clasification.Scout];
            attack.strength_rating = ConfigureMhoddim.starting_health[Soldier.Clasification.Scout];
        } else if (GetComponent<Striker>() != null) {
            attack.agility_rating = ConfigureMhoddim.starting_health[Soldier.Clasification.Striker];
            attack.strength_rating = ConfigureMhoddim.starting_health[Soldier.Clasification.Striker];
        }
    }
}