using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fey : MonoBehaviour {
    public Dictionary<string, float> starting_health = new Dictionary<string, float>();
    public Dictionary<string, float> recovery_rate = new Dictionary<string, float>();

    private void Awake()
    {
        starting_health["ent"] = 500;
        recovery_rate["ent"] = 0.15f;
    }


    public bool SetHealthStats(GameObject unit)
    {
        Health health = unit.GetComponent<Health>();
        if (health == null) return false;

        if (unit.GetComponent<Ent>() != null) {
            health.SetStartingHealth(starting_health["ent"]);
            health.SetRecoveryRate(recovery_rate["ent"]);
        } else {
            return false;
        }

        return true;
    }
}
