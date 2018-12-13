using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn : MonoBehaviour {

    public static float action_threshold = 6f;
    public float haste_delta = 1f;
    float current_haste;

    Health health;
    Attack attack;
    Movement movement;

    // Unity


    private void Awake () {
        health = GetComponent<Health>();
        attack = GetComponent<Attack>();
        movement = GetComponent<Movement>();
    }


    private void Update () {
        if (current_haste > Turn.action_threshold) {
            ResolveCurrentHealth();
            // ResolveMovement();
            ResolveAttacks();
            current_haste = 0f; 
        } else {
            current_haste += haste_delta * Time.deltaTime;
        }
    }


    // public



    // private


    private void ResolveAttacks()
    {
        StartCoroutine(attack.ManageAttacks());
        if (attack.enemies_abound) {
            if (movement != null) {
                movement.ResetPath();
                movement.SetDestination(attack.GetAnEnemy().transform.position);
            }
        }
    }


    private void ResolveCurrentHealth()
    {
        health.RecoverHealth(health.recovery_rate * health.starting_health);
        // TODO: health.ApplyDamageOverTime
        health.PersistOrPerish();
    }
}
