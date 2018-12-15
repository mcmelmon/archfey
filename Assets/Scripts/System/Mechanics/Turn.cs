using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn : MonoBehaviour {

    public static float action_threshold = 6f;
    public float haste_delta = 1f;
    float current_haste;

    Actor actor;
    Health health;
    Attack attack;
    Movement movement;

    // Unity


    private void Awake () {
        actor = GetComponent<Actor>();
        health = GetComponent<Health>();
        attack = GetComponent<Attack>();
        movement = GetComponent<Movement>();
    }


    private void Update () {
        if (current_haste > action_threshold) {
            ResolveCurrentHealth();
            ResolveMovement();
            ResolveFriendAndFoe();
            ResolveAttacks();
            current_haste = 0f; 
        } else {
            current_haste += haste_delta * Time.deltaTime;
        }
    }


    // private


    private void ResolveAttacks()
    {
        StartCoroutine(attack.ManageAttacks());
    }


    private void ResolveCurrentHealth()
    {
        health.RecoverHealth(health.recovery_rate * health.starting_health);
        // TODO: health.ApplyDamageOverTime
        health.PersistOrPerish();
    }


    private void ResolveFriendAndFoe()
    {
        actor.FriendAndFoe();
    }


    private void ResolveMovement()
    {
        if (actor.enemies_abound) {
            GameObject enemy = actor.GetAnEnemy();
            if (enemy != null && movement != null) {
                movement.ResetPath();
                movement.SetDestination(enemy.transform.position);
            }
        }
    }
}
