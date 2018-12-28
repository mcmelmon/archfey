using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghaddim : MonoBehaviour {

    // properties

    public static Dictionary<Weapon.DamageType, int> SuperiorWeapons { get; set; }
    public static Threat Threat { get; set; }


    // static


    public static GameObject SpawnUnit(Vector3 _point)
    {
        Ghaddim _ghaddim = Instantiate(Conflict.Instance.ghaddim_prefab, _point + new Vector3(0, 4, 0), Conflict.Instance.ghaddim_prefab.transform.rotation);
        _ghaddim.gameObject.AddComponent<Soldier>();

        return _ghaddim.gameObject;
    }


    // Unity


    private void Awake()
    {
        Threat = gameObject.AddComponent<Threat>();
        SuperiorWeapons = new Dictionary<Weapon.DamageType, int>
        {
            [Weapon.DamageType.Arcane] = 0,
            [Weapon.DamageType.Blunt] = 0,
            [Weapon.DamageType.Elemental] = 0,
            [Weapon.DamageType.Piercing] = 0,
            [Weapon.DamageType.Poison] = 2,
            [Weapon.DamageType.Slashing] = 0
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
        Defend defend = GetComponent<Defend>();
        if (defend == null) return;

        if (GetComponent<Heavy>() != null) {
            defend.AgilityRating = ConfigureGhaddim.agility_rating[Soldier.Clasification.Heavy];
            defend.ArmorRating = ConfigureGhaddim.armor_rating[Soldier.Clasification.Heavy];
            defend.ConstitutionRating = ConfigureGhaddim.constitution_rating[Soldier.Clasification.Heavy];
            defend.ForceRating = ConfigureGhaddim.force_rating[Soldier.Clasification.Heavy];
            defend.SetResistances(ConfigureGhaddim.resistances[Soldier.Clasification.Heavy]);
        } else if (GetComponent<Scout>() != null) {
            defend.AgilityRating = ConfigureGhaddim.agility_rating[Soldier.Clasification.Scout];
            defend.ArmorRating = ConfigureGhaddim.armor_rating[Soldier.Clasification.Scout];
            defend.ConstitutionRating = ConfigureGhaddim.constitution_rating[Soldier.Clasification.Scout];
            defend.ForceRating = ConfigureGhaddim.force_rating[Soldier.Clasification.Scout];
            defend.SetResistances(ConfigureGhaddim.resistances[Soldier.Clasification.Scout]);
        } else if (GetComponent<Striker>() != null) {
            defend.AgilityRating = ConfigureGhaddim.agility_rating[Soldier.Clasification.Striker];
            defend.ArmorRating = ConfigureGhaddim.armor_rating[Soldier.Clasification.Striker];
            defend.ConstitutionRating = ConfigureGhaddim.constitution_rating[Soldier.Clasification.Striker];
            defend.ForceRating = ConfigureGhaddim.force_rating[Soldier.Clasification.Striker];
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
            attack.AgilityRating = ConfigureGhaddim.agility_rating[Soldier.Clasification.Heavy];
            attack.StrengthRating = ConfigureGhaddim.strength_rating[Soldier.Clasification.Heavy];
        }
        else if (GetComponent<Scout>() != null) {
            attack.AgilityRating = ConfigureGhaddim.agility_rating[Soldier.Clasification.Scout];
            attack.StrengthRating = ConfigureGhaddim.strength_rating[Soldier.Clasification.Scout];
        } else if (GetComponent<Striker>() != null) {
            attack.AgilityRating = ConfigureGhaddim.agility_rating[Soldier.Clasification.Striker];
            attack.StrengthRating = ConfigureGhaddim.strength_rating[Soldier.Clasification.Striker];
        }
    }
}