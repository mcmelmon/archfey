using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fey : MonoBehaviour {


    // public


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

        if (GetComponent<Ent>() != null)
        {
            defend.agility_rating = ConfigureFey.agility_rating[Soldier.Clasification.Ent];
            defend.armor_rating = ConfigureFey.armor_rating[Soldier.Clasification.Ent];
            defend.constitution_rating = ConfigureFey.constitution_rating[Soldier.Clasification.Ent];
            defend.force_rating = ConfigureFey.force_rating[Soldier.Clasification.Ent];
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


    private void SetOffenseStats()
    {
        Attack attack = GetComponent<Attack>();
        if (attack == null) return;

        if (GetComponent<Ent>() != null) {
            attack.AgilityRating = ConfigureFey.agility_rating[Soldier.Clasification.Ent];
            attack.StrengthRating = ConfigureFey.strength_rating[Soldier.Clasification.Ent];
        }
    }
}
