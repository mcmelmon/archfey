using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public enum Range { Melee = 0, Ranged = 1 };
    public enum DamageType { Blunt = 0, Piercing = 1, Slashing = 2, Poison = 3, Elemental = 4, Arcane = 5 };

    public Range range;             // melee or ranged
    public DamageType damage_type;  // what is the nature of the damage caused by the weapon?
    public int instant_damage;    // how much damage does the weapon cause when it hits?
    public int damage_over_time;  // how much ongoing damage does the weapon cause?
    public int penetration;       // how effectively does the weapon circumvent armor?
    public int potency;           // how effectively does the weapon overcome resistance to its type?
    public float projectile_speed;

    public GameObject impact_prefab;
    public Transform ranged_attack_origin;
    public Transform melee_attack_origin;
    public float ranged_attack_range;
    public float melee_attack_range;


    // properties

    public GameObject Target { get; set; }
    public Health TargetHealth { get; set; }
    public Defend TargetDefend { get; set; }
    public Threat TargetThreat { get; set; }


    // Unity


    void Start()
    {
        if (range == Range.Melee) StartCoroutine(Seek());
    }


    private void OnValidate()
    {
        if (projectile_speed <= 1) projectile_speed = 10f;
    }


    // public


    public void Hit()
    {
        if (Target != null) {
            Impact();
            ApplyDamage();
            CleanUpAmmunition();
        }
    }


    public void SetTarget(GameObject _target)
    {
        Target = _target;
        TargetHealth = Target.GetComponent<Health>();
        TargetDefend = Target.GetComponent<Defend>();
        TargetThreat = Target.GetComponent<Threat>();
    }


    // private


    private void ApplyDamage()
    {
        if (TargetHealth != null && TargetDefend != null && this.transform.parent.gameObject != null) {
            float damage_inflicted = TargetDefend.HandleAttack(this, this.transform.parent.gameObject.GetComponent<Attack>());
            TargetHealth.LoseHealth(damage_inflicted);
            TargetThreat.AddThreat(transform.parent.gameObject, damage_inflicted);
            TargetThreat.SpreadThreat(transform.parent.gameObject, damage_inflicted);
        }
    }


    private void CleanUpAmmunition()
    {
        if (range == Range.Ranged) {
            Destroy(gameObject);
        }
    }


    private void Impact() {
        GameObject _impact = Instantiate(impact_prefab, transform.parent.transform.position + new Vector3(0, 4f, 0), transform.rotation);
        _impact.name = "Impact";
        Destroy(_impact, 2f);
    }


    private IEnumerator Seek()
    {
        while (true) {
            if (range == Range.Ranged) {
                if (Target == null) {
                    Destroy(gameObject);  // destroy ranged "ammunition"
                    yield return null;
                }

                float separation = float.MaxValue;
                Vector3 direction = Target.transform.position - transform.position;
                float distance = projectile_speed * Time.deltaTime;
                transform.position += distance * direction;
                separation = Vector3.Distance(Target.transform.position, transform.position);

                if (separation <= .5f) Hit();
            }

            yield return null;
        }
    }
}
