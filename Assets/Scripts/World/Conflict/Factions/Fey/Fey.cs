using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fey : MonoBehaviour {


    // public


    public void SetStats()
    {
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
            defend.SetAgilityRating(ConfigureFey.agility_rating["ent"]);
            defend.SetArmorRating(ConfigureFey.armor_rating["ent"]);
            defend.SetCorporealRating(ConfigureFey.corporeal_rating["ent"]);
            defend.SetCounter(ConfigureFey.counter["ent"]);
            defend.SetForceRating(ConfigureFey.force_rating["ent"]);
            defend.SetResistances(ConfigureFey.resistances["ent"]);
        }
    }


    private void SetHealthStats()
    {
        Health health = GetComponent<Health>();
        if (health == null) return;

        if (GetComponent<Ent>() != null)
        {
            health.SetStartingHealth(ConfigureFey.starting_health["ent"]);
            health.SetRecoveryRate(ConfigureFey.recovery_rate["ent"]);
        }
    }
}
