using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    public float starting_health;
    public float current_health;
    public float recovery_rate;


    // Unity


    private void OnValidate()
    {
        if (starting_health < 1f) starting_health = 1f;
        if (current_health < 0f) current_health = 1f;
    }


    // public 


    public void LoseHealth(float amount)
    {
        float modified_amount = amount;  // TODO: handle resistances/armore/etc

        current_health -= modified_amount;
    }


    public void RecoverHealth(float amount)
    {
        if (Mathf.Approximately(current_health, starting_health) || Mathf.Approximately(recovery_rate, 0)) return;

        current_health += amount;
        if (current_health > starting_health) current_health = starting_health;
    }


    public void PersistOrPerish()
    {
        if (current_health <= 0)
        {
            // Objects being destroyed suddenly probably lack health configuration stats...
            Destroy(gameObject);
        }
    }


    public void SetRecoveryRate(float rate)
    {
        recovery_rate = rate;
    }


    public void SetStartingHealth(float amount)
    {
        starting_health = amount;
        current_health = amount;
    }
}
