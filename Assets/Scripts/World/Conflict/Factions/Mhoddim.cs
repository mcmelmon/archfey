﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mhoddim : MonoBehaviour {

    public Dictionary<string, float> starting_health = new Dictionary<string, float>();
    public Dictionary<string, float> recovery_rate = new Dictionary<string, float>();

    private void Awake()
    {
        starting_health["scout"] = 70f;
        starting_health["striker"] = 100f;
        starting_health["heavy"] = 130f;

        recovery_rate["scout"] = 0f;
        recovery_rate["striker"] = 0f;
        recovery_rate["heavy"] = 0f;
    }


    public bool SetHealthStats(GameObject unit)
    {
        Health health = unit.GetComponent<Health>();
        if (health == null) return false;

        if (unit.GetComponent<Scout>() != null) {
            health.SetStartingHealth(starting_health["scout"]);
            health.SetRecoveryRate(recovery_rate["scout"]);
        }
        else if (unit.GetComponent<Striker>() != null) {
            health.SetStartingHealth(starting_health["striker"]);
            health.SetRecoveryRate(recovery_rate["striker"]);
        }
        else if (unit.GetComponent<Heavy>() != null)
        {
            health.SetStartingHealth(starting_health["heavy"]);
            health.SetRecoveryRate(recovery_rate["heavy"]);
        } else {
            return false;
        }

        return true;
    }
}