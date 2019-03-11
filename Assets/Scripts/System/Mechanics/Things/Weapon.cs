using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    // Inspector settings

    public string weapon_name;
    public int attack_bonus;
    public int damage_bonus;
    public int dice_type;
    public int number_of_dice;
    public Weapons.DamageType damage_type;
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
