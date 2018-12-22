using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghaddim : MonoBehaviour {

    // properties

    public static Threat Threat { get; set; }


    // static


    public static GameObject SpawnUnit()
    {
        Ghaddim _ghaddim = Instantiate(Conflict.Instance.ghaddim_prefab, Conflict.Instance.ghaddim_prefab.transform.position, Conflict.Instance.ghaddim_prefab.transform.rotation);
        _ghaddim.gameObject.AddComponent<Soldier>();

        return _ghaddim.gameObject;
    }


    // Unity


    private void Awake()
    {
        Threat = gameObject.AddComponent<Threat>();
    }


    // public


    public void AddFactionThreat(GameObject _foe, float _threat)
    {
        Threat.AddThreat(_foe, _threat);
    }


    public GameObject BiggestFactionThreat()
    {
        return Threat.BiggestThreat();
    }


    public bool IsFactionThreat(GameObject _sighting)
    {
        return _sighting != null && Threat.IsAThreat(_sighting);
    }


    public void SetStats()
    {
        SetDefenseStats();
        SetHealthStats();
        SetOffenseStats();
    }


    // private


    private void SetDefenseStats()
    {
        Defend defend = GetComponent<Defend>();
        if (defend == null) return;

        if (GetComponent<Heavy>() != null) {
            defend.agility_rating = ConfigureGhaddim.agility_rating[Soldier.Clasification.Heavy];
            defend.armor_rating = ConfigureGhaddim.armor_rating[Soldier.Clasification.Heavy];
            defend.corporeal_rating = ConfigureGhaddim.corporeal_rating[Soldier.Clasification.Heavy];
            defend.counter = ConfigureGhaddim.counter[Soldier.Clasification.Heavy];
            defend.force_rating = ConfigureGhaddim.force_rating[Soldier.Clasification.Heavy];
            defend.SetResistances(ConfigureGhaddim.resistances[Soldier.Clasification.Heavy]);
        } else if (GetComponent<Scout>() != null) {
            defend.agility_rating = ConfigureGhaddim.agility_rating[Soldier.Clasification.Scout];
            defend.armor_rating = ConfigureGhaddim.armor_rating[Soldier.Clasification.Scout];
            defend.corporeal_rating = ConfigureGhaddim.corporeal_rating[Soldier.Clasification.Scout];
            defend.counter = ConfigureGhaddim.counter[Soldier.Clasification.Scout];
            defend.force_rating = ConfigureGhaddim.force_rating[Soldier.Clasification.Scout];
            defend.SetResistances(ConfigureGhaddim.resistances[Soldier.Clasification.Scout]);
        } else if (GetComponent<Striker>() != null) {
            defend.agility_rating = ConfigureGhaddim.agility_rating[Soldier.Clasification.Striker];
            defend.armor_rating = ConfigureGhaddim.armor_rating[Soldier.Clasification.Striker];
            defend.corporeal_rating = ConfigureGhaddim.corporeal_rating[Soldier.Clasification.Striker];
            defend.counter = ConfigureGhaddim.counter[Soldier.Clasification.Striker];
            defend.force_rating = ConfigureGhaddim.force_rating[Soldier.Clasification.Striker];
            defend.SetResistances(ConfigureGhaddim.resistances[Soldier.Clasification.Striker]);
        }
    }


    private void SetHealthStats()
    {
        Health health = GetComponent<Health>();
        if (health == null) return;

        if (GetComponent<Heavy>() != null) {
            health.SetStartingHealth(ConfigureGhaddim.starting_health[Soldier.Clasification.Heavy]);
            health.SetRecoveryAmount(ConfigureGhaddim.recovery_amount[Soldier.Clasification.Heavy]);
        } else if (GetComponent<Scout>() != null) {
            health.SetStartingHealth(ConfigureGhaddim.starting_health[Soldier.Clasification.Scout]);
            health.SetRecoveryAmount(ConfigureGhaddim.recovery_amount[Soldier.Clasification.Scout]);
        } else if (GetComponent<Striker>() != null) {
            health.SetStartingHealth(ConfigureGhaddim.starting_health[Soldier.Clasification.Striker]);
            health.SetRecoveryAmount(ConfigureGhaddim.recovery_amount[Soldier.Clasification.Striker]);
        } 
    }


    private void SetOffenseStats()
    {
        Attack attack = GetComponent<Attack>();
        if (attack == null) return;

        if (GetComponent<Heavy>() != null) {
            attack.AgilityRating = ConfigureGhaddim.starting_health[Soldier.Clasification.Heavy];
            attack.StrengthRating = ConfigureGhaddim.starting_health[Soldier.Clasification.Heavy];
        }
        else if (GetComponent<Scout>() != null) {
            attack.AgilityRating = ConfigureGhaddim.starting_health[Soldier.Clasification.Scout];
            attack.StrengthRating = ConfigureGhaddim.starting_health[Soldier.Clasification.Scout];
        } else if (GetComponent<Striker>() != null) {
            attack.AgilityRating = ConfigureGhaddim.starting_health[Soldier.Clasification.Striker];
            attack.StrengthRating = ConfigureGhaddim.starting_health[Soldier.Clasification.Striker];
        }
    }
}