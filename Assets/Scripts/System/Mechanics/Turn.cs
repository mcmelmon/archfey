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
    Senses senses;
    Stealth stealth;

    // Unity


    private void Awake () {
        actor = GetComponent<Actor>();
        health = GetComponent<Health>();
        attack = GetComponent<Attack>();
        movement = GetComponent<Movement>();
        senses = GetComponent<Senses>();
        stealth = GetComponent<Stealth>();

        actor.SetComponents();
    }


    private void Start () {
        StartCoroutine(ResolveTurns());
    }


    // public

    public void SetStealth(Stealth _stealth)
    {
        stealth = _stealth;
    }


    // private


    private void ResolveAttacks()
    {
        attack.ManageAttacks();
    }


    private bool Healthy()
    {
        health.RecoverHealth(health.recovery_rate * health.starting_health);
        // TODO: health.ApplyDamageOverTime
        return health.Persist();
    }


    private void ResolveFriendAndFoe()
    {
        actor.FriendAndFoe();
    }


    private void ResolveMovement()
    {
        if (movement == null) return;

        if (actor.enemies_abound) {
            GameObject enemy = actor.GetAnEnemy();
            if (enemy != null)
                movement.SetDestination(enemy.transform.position);
        }
    }


    private void ResolveRuinControl()
    {
        actor.EstablishRuinControl();
    }


    private void ResolveSightings()
    {
        senses.Sight();
    }


    private IEnumerator ResolveTurns()
    {
        while (true) {
            if (current_haste < action_threshold) {
                current_haste += haste_delta * Time.deltaTime;
            } else {
                if (Healthy()) {
                    TakeAction();
                    current_haste = 0;
                }
            }

            yield return null;
        }
    }


    private void TakeAction()
    {
        ResolveSightings();
        ResolveMovement();
        ResolveFriendAndFoe();
        ResolveAttacks();
        ResolveRuinControl();
    }
}
