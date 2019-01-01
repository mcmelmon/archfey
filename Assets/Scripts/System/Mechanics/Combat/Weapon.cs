using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public enum DamageType { 
        Acid = 0,
        Bludgeoning = 1,
        Cold = 2,
        Fire = 3,
        Force = 4,
        Lightning = 5,
        Necrotic = 6,
        Piercing = 7,
        Poison = 8,
        Psychic = 9,
        Radiant = 10,
        Slashing = 11,
        Thunder = 12
    };

    // Inspector settings

    public int attack_bonus;
    public int damage_bonus;
    public int damage_die;
    public DamageType damage_type;
    public int expected_damage;
    public bool has_reach;
    public bool is_finese;
    public bool is_heavy;
    public bool is_light;
    public bool is_loaded;
    public bool is_magic;
    public bool is_silvered;
    public bool is_thrown;
    public bool is_two_handed;
    public bool is_versatile;
    public GameObject projectile_prefab;
    public int range;

}
