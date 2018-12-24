using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mhoddim : MonoBehaviour {

    // properties

    public static Threat Threat { get; set; }


    // static


    public static GameObject SpawnUnit(Vector3 _point)
    {
        Mhoddim _mhoddim = Instantiate(Conflict.Instance.mhoddim_prefab, _point + new Vector3(0, 4, 0), Conflict.Instance.mhoddim_prefab.transform.rotation);
        _mhoddim.gameObject.AddComponent<Soldier>();

        return _mhoddim.gameObject;
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
            attack.AgilityRating = ConfigureMhoddim.agility_rating[Soldier.Clasification.Heavy];
            attack.StrengthRating = ConfigureMhoddim.strength_rating[Soldier.Clasification.Heavy];
        } else if (GetComponent<Scout>() != null) {
            attack.AgilityRating = ConfigureMhoddim.agility_rating[Soldier.Clasification.Scout];
            attack.StrengthRating = ConfigureMhoddim.strength_rating[Soldier.Clasification.Scout];
        } else if (GetComponent<Striker>() != null) {
            attack.AgilityRating = ConfigureMhoddim.agility_rating[Soldier.Clasification.Striker];
            attack.StrengthRating = ConfigureMhoddim.strength_rating[Soldier.Clasification.Striker];
        }
    }
}