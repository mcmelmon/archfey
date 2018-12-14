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
    public float ranged_speed;

    public GameObject impact_prefab;
    public Transform ranged_attack_origin;
    public Transform melee_attack_origin;
    public float ranged_attack_range;
    public float melee_attack_range;

    GameObject target;
    Health target_health;
    Defend target_defend;


    // Unity


    void Update()
    {
        if (range == Range.Ranged) Seek();
    }


    private void OnValidate()
    {
        if (ranged_speed <= 1) ranged_speed = 10f;
    }


    // public


    public void Hit()
    {
        if (target != null) {
            target_defend = target.GetComponent<Defend>();          // TODO: incorporate possibility of miss 

            Impact();
            ApplyDamage();
            CleanUpAmmunition();
        }
    }


    public void Seek()
    {
        if (range != Range.Ranged) return;

        if (target == null) {
            Destroy(gameObject);  // destroy ranged "ammunition"
            return;
        }

        float separation = float.MaxValue;
        Vector3 direction = target.transform.position - transform.position;
        float distance = ranged_speed * Time.deltaTime;
        transform.position += distance * direction;
        separation = Vector3.Distance(target.transform.position, transform.position);

        if (separation <= .5f) Hit();
    }


    public void SetTarget(GameObject _target)
    {
        target = _target;
    }


    // private


    private void ApplyDamage()
    {
        target_health = target.GetComponent<Health>();

        if (target_health != null) {
            target_health.LoseHealth(instant_damage); // TODO: reduce damage by resistances/defense; apply damage over time
            target_health.AddDamager(transform.parent.gameObject, instant_damage);
            SpreadThreat();
        }
    }


    private void CleanUpAmmunition()
    {
        if (range == Range.Ranged) {
            Destroy(gameObject);
        }
    }


    private void Impact() {
        GameObject _impact = Instantiate(impact_prefab, transform.position + new Vector3(0, 2f, 0), transform.rotation);
        _impact.name = "Impact";
        Destroy(_impact, 2f);
    }


    private void SpreadThreat()
    {
        Mhoddim mhoddim_faction = target.GetComponent<Mhoddim>();
        Ghaddim ghaddim_faction = target.GetComponent<Ghaddim>();

        if (mhoddim_faction != null) mhoddim_faction.AddFactionThreat(transform.parent.gameObject, instant_damage);
        if (ghaddim_faction != null) ghaddim_faction.AddFactionThreat(transform.parent.gameObject, instant_damage);
    }
}
