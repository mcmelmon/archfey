using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public enum Range { Melee = 0, Ranged = 1 };
    public enum DamageType { Blunt = 0, Piercing = 1, Slashing = 2, Poison = 3, Elemental = 4, Arcane = 5, Holy = 6 };

    // Inspector settings

    public Range range;             // melee or ranged
    public int damage_maximum;      // maximum damage on hit
    public DamageType damage_type;  // what is the nature of the damage caused by the weapon?
    public int damage_over_time;    // how much ongoing damage does the weapon cause?
    public int penetration;         // how effectively does the weapon circumvent armor?
    public int potency;             // how effectively does the weapon overcome resistance to its type?
    public float projectile_speed;

    public GameObject impact_prefab;

    public Transform melee_attack_origin;
    public float melee_attack_range;
    public float ranged_attack_range;
    public Transform ranged_attack_origin;


    // Unity


    private void OnValidate()
    {
        if (projectile_speed <= 1) projectile_speed = 10f;
    }


    // public


    public void CleanUpAmmunition()
    {
        if (range == Range.Ranged) {
            Destroy(gameObject);
        }
    }


    public void Impact() {
        GameObject _impact = Instantiate(impact_prefab, transform.parent.transform.position + new Vector3(0, 4f, 0), transform.rotation);
        _impact.name = "Impact";
        Destroy(_impact, 2f);
    }
}
