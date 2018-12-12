﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public enum Range { Melee = 0, Ranged = 1 };
    public enum Type { Blunt = 0, Piercing = 1, Slashing = 2, Poison = 3, Elemental = 4, Arcane = 5 };

    public Range range;
    public Type type;               // what is the nature of the damage caused by the weapon?
    public float instant_damage;    // how much damage does the weapon cause when it hits?
    public float damage_over_time;  // how much ongoing damage does the weapon cause?
    public float potency;           // how effectively does the weapon overcome resistance?
    public float speed;

    public GameObject impact_prefab;
    public Transform ranged_attack_origin;
    public Transform melee_attack_origin;
    public float ranged_attack_range;
    public float melee_attack_range;

    GameObject target;
    Health target_health;
    Defend target_defend;


    // TODO: differentiate between Type.Melee and Ranged

    // Unity


    void Update()
    {
        if (target != null) {
            target_health = target.GetComponent<Health>();
            target_defend = target.GetComponent<Defend>();
            Aim();
        } else {
            // TODO: destroying doesn't make sense for melee
            Destroy(gameObject); // if we have no target, destroy the weapon
            return;
        }
    }

    private void OnValidate()
    {
        if (speed <= 1) speed = 10f;
    }


    // public


    public void Target(GameObject _target)
    {
        if (_target != null) target = _target;
    }


    // private


    private void Hit()
    {
        GameObject _impact = Instantiate(impact_prefab, transform.position + new Vector3(0,2f,0), transform.rotation);
        _impact.name = "Impact";
        Destroy(gameObject);  // get rid of the weapon; TODO: this makes sense for ranged, but will need to be updated for melee
        Destroy(_impact, 2f);
        target_health.LoseHealth(instant_damage); // TODO: reduce damage by resistances/defense; apply damage over time
    }


    private void Aim()
    {
        // TODO: incorporate possibility of missing due to Defend

        Vector3 direction = target.transform.position - transform.position;
        float distance = speed * Time.deltaTime;
        transform.position += distance * direction;

        if (Vector3.Distance(transform.position, target.transform.position) <= .4f)
        {
            Hit();
        }
    }
}
