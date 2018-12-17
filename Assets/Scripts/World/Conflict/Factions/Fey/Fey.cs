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
            defend.SetAgilityRating(ConfigureFey.agility_rating[Soldier.Clasification.Ent]);
            defend.SetArmorRating(ConfigureFey.armor_rating[Soldier.Clasification.Ent]);
            defend.SetCorporealRating(ConfigureFey.corporeal_rating[Soldier.Clasification.Ent]);
            defend.SetCounter(ConfigureFey.counter[Soldier.Clasification.Ent]);
            defend.SetForceRating(ConfigureFey.force_rating[Soldier.Clasification.Ent]);
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
            health.SetRecoveryRate(ConfigureFey.recovery_rate[Soldier.Clasification.Ent]);
        }
    }
}
