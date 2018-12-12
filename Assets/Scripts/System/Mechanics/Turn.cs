using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn : MonoBehaviour {

    public static float action_threshold = 6f;
    public float haste_delta = 1f;
    float current_haste;

    Health health;
    Attack attack;
    Senses senses;
    List<GameObject> sightings = new List<GameObject>();


    // Unity


    private void Awake () {
        health = GetComponent<Health>();
        attack = GetComponent<Attack>();
        senses = GetComponent<Senses>();
	}
	

	private void Update () {
        if (current_haste >= Turn.action_threshold) {
            health.RecoverHealth(health.recovery_rate * health.starting_health);
            // TODO: health.ApplyDamageOverTime
            health.PersistOrPerish();
            sightings = senses.GetSightings();
            StartCoroutine(attack.ManageAttacks());
            current_haste = 0f; 
        } else {
            current_haste += haste_delta * Time.deltaTime;
        }
    }


    // public


    public List<GameObject> GetSightings()
    {
        return sightings;
    }
}
