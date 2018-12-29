using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fey : MonoBehaviour {


    // properties

    public Stats Stats { get; set; }


    // Unity


    private void Awake()
    {
        Stats = GetComponent<Stats>();
    }


    // public


    public void SetStats()
    {
        SetPrimaryStats();
        SetDefenseStats();
        SetHealthStats();
    }


    // private


    private void SetDefenseStats()
    {
        Defend defend = GetComponent<Defend>();
        if (defend == null) return;

        if (GetComponent<Ent>() != null)
        {
            defend.ArmorRating = ConfigureFey.armor_rating[Soldier.Clasification.Ent];
            defend.ForceRating = ConfigureFey.force_rating[Soldier.Clasification.Ent];
            defend.SetResistances(ConfigureFey.resistances[Soldier.Clasification.Ent]);
        }
    }


    private void SetHealthStats()
    {
        Health health = GetComponent<Health>();
        if (health == null) return;

        if (GetComponent<Ent>() != null)
        {
            health.SetStartingHealth(ConfigureFey.starting_health[Soldier.Clasification.Ent]);
            health.SetRecoveryAmount(ConfigureFey.recovery_amount[Soldier.Clasification.Ent]);
        }
    }


    private void SetPrimaryStats()
    {
        if (GetComponent<Ent>() != null) {
            Stats.AgilityRating = ConfigureFey.agility_rating[Soldier.Clasification.Ent];
            Stats.ConstitutionRating = ConfigureFey.constitution_rating[Soldier.Clasification.Ent];
            Stats.IntellectRating = ConfigureFey.intellect_rating[Soldier.Clasification.Ent];
            Stats.StrengthRating = ConfigureFey.strength_rating[Soldier.Clasification.Ent];
            Stats.WillRating = ConfigureFey.will_rating[Soldier.Clasification.Ent];
        }
    }
}
