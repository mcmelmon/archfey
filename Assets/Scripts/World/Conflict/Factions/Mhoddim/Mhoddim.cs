using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mhoddim : MonoBehaviour {

    // properties

    public static Dictionary<Weapon.DamageType, int> SuperiorWeapons { get; set; }
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
        SuperiorWeapons = new Dictionary<Weapon.DamageType, int>
        {
            [Weapon.DamageType.Arcane] = 4,
            [Weapon.DamageType.Blunt] = 1,
            [Weapon.DamageType.Elemental] = 3,
            [Weapon.DamageType.Piercing] = 2,
            [Weapon.DamageType.Poison] = 0,
            [Weapon.DamageType.Slashing] = 2
        };
    }


    // public


    public void AddFactionThreat(Actor _foe, float _threat)
    {
        Threat.AddThreat(_foe, _threat);
    }


    public Actor BiggestFactionThreat()
    {
        return Threat.BiggestThreat();
    }


    public bool IsFactionThreat(Actor _sighting)
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
        Defend defend = GetComponentInChildren<Defend>();
        if (defend == null) return;

        if (GetComponent<Heavy>() != null) {
            defend.AgilityRating = ConfigureMhoddim.agility_rating[Soldier.Clasification.Heavy];
            defend.ArmorRating = ConfigureMhoddim.armor_rating[Soldier.Clasification.Heavy];
            defend.ConstitutionRating = ConfigureMhoddim.constituion_rating[Soldier.Clasification.Heavy];
            defend.ForceRating = ConfigureMhoddim.force_rating[Soldier.Clasification.Heavy];
            defend.IntellectRating = ConfigureMhoddim.intellect_rating[Soldier.Clasification.Heavy];
            defend.WillRating = ConfigureMhoddim.will_rating[Soldier.Clasification.Heavy];
            defend.SetResistances(ConfigureMhoddim.resistances[Soldier.Clasification.Heavy]);
        } else if (GetComponent<Scout>() != null) {
            defend.AgilityRating = ConfigureMhoddim.agility_rating[Soldier.Clasification.Scout];
            defend.ArmorRating = ConfigureMhoddim.armor_rating[Soldier.Clasification.Scout];
            defend.ConstitutionRating = ConfigureMhoddim.constituion_rating[Soldier.Clasification.Scout];
            defend.ForceRating = ConfigureMhoddim.force_rating[Soldier.Clasification.Scout];
            defend.IntellectRating = ConfigureMhoddim.intellect_rating[Soldier.Clasification.Scout];
            defend.WillRating = ConfigureMhoddim.will_rating[Soldier.Clasification.Scout];
            defend.SetResistances(ConfigureMhoddim.resistances[Soldier.Clasification.Scout]);
        } else if (GetComponent<Striker>() != null) {
            defend.AgilityRating = ConfigureMhoddim.agility_rating[Soldier.Clasification.Striker];
            defend.ArmorRating = ConfigureMhoddim.armor_rating[Soldier.Clasification.Striker];
            defend.ConstitutionRating = ConfigureMhoddim.constituion_rating[Soldier.Clasification.Striker];
            defend.ForceRating = ConfigureMhoddim.force_rating[Soldier.Clasification.Striker];
            defend.IntellectRating = ConfigureMhoddim.intellect_rating[Soldier.Clasification.Striker];
            defend.WillRating = ConfigureMhoddim.will_rating[Soldier.Clasification.Striker];
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
        Attack attack = GetComponentInChildren<Attack>();
        if (attack == null) return;

        if (GetComponent<Heavy>() != null) {
            attack.AgilityRating = ConfigureMhoddim.agility_rating[Soldier.Clasification.Heavy];
            attack.IntellectRating = ConfigureMhoddim.intellect_rating[Soldier.Clasification.Heavy];
            attack.StrengthRating = ConfigureMhoddim.strength_rating[Soldier.Clasification.Heavy];
            attack.WillRating = ConfigureMhoddim.will_rating[Soldier.Clasification.Heavy];
        }
        else if (GetComponent<Scout>() != null) {
            attack.AgilityRating = ConfigureMhoddim.agility_rating[Soldier.Clasification.Scout];
            attack.IntellectRating = ConfigureMhoddim.intellect_rating[Soldier.Clasification.Scout];
            attack.StrengthRating = ConfigureMhoddim.strength_rating[Soldier.Clasification.Scout];
            attack.WillRating = ConfigureMhoddim.will_rating[Soldier.Clasification.Scout];
        }
        else if (GetComponent<Striker>() != null) {
            attack.AgilityRating = ConfigureMhoddim.agility_rating[Soldier.Clasification.Striker];
            attack.IntellectRating = ConfigureMhoddim.intellect_rating[Soldier.Clasification.Striker];
            attack.StrengthRating = ConfigureMhoddim.strength_rating[Soldier.Clasification.Striker];
            attack.WillRating = ConfigureMhoddim.will_rating[Soldier.Clasification.Striker];
        }
    }
}