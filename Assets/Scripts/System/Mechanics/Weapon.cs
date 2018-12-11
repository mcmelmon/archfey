using System.Collections;
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

    Transform target;


    // TODO: differentiate between Type.Melee and Ranged

    // Unity


    void Update()
    {
        if (target != null)
        {
            target.GetComponent<Renderer>().material.color = Color.green;
            Attack();
        }
        else
        {
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
        if (_target != null) target = _target.transform;
    }


    // private


    private void Hit()
    {
        GameObject _impact = Instantiate(impact_prefab, transform.position, transform.rotation);
        _impact.name = "Impact";
        Destroy(gameObject);
        Destroy(_impact, 2f);
        Destroy(target.gameObject); // TODO: inflict damage on health instead of autokill
    }


    private void Attack()
    {
        Vector3 direction = target.position - transform.position;
        float distance = speed * Time.deltaTime;
        transform.position += distance * direction;

        if (Vector3.Distance(transform.position, target.position) <= .4f)
        {
            Hit();
        }
    }
}
