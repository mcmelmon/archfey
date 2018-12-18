using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    public int starting_health;
    public int current_health;
    public int recovery_amount;
    Actor actor;

    // Unity


    private void Awake()
    {
        actor = GetComponent<Actor>();
    }


    private void OnValidate()
    {
        if (starting_health < 1) starting_health = 1;
        if (current_health < 0f) current_health = 0;
    }


    private void Update()
    {
        // TODO: prune damagers every now and then to be sure destroyed objects are gone
    }


    // public 


    public void ApplyDamageOverTime()
    {
        // TODO
    }


    public void LoseHealth(float amount)
    {
        current_health -= Mathf.RoundToInt(amount);
    }


    public void RecoverHealth(int amount)
    {
        if (amount == 0 || current_health == starting_health) return;

        current_health += amount;
        if (current_health > starting_health) current_health = starting_health;
    }


    public bool Persist()
    {
        if (current_health <= 0) {
            Destroy(gameObject);
            return false;
        }

        return true;
    }


    public void SetRecoveryAmount(int amount)
    {
        recovery_amount = amount;
    }


    public void SetStartingHealth(int amount)
    {
        starting_health = amount;
        current_health = amount;
    }
}
