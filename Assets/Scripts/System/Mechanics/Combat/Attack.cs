using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {

    public int agility_rating;
    public List<Weapon> available_weapons;
    public int strength_rating;

    private readonly List<GameObject> available_melee_targets = new List<GameObject>();
    private readonly List<GameObject> available_ranged_targets = new List<GameObject>();
    private readonly List<GameObject> current_melee_targets = new List<GameObject>();
    private readonly List<GameObject> current_ranged_targets = new List<GameObject>();

    Actor actor;
    Weapon equipped_weapon;

    // Unity


    private void Awake()
    {
        actor = GetComponent<Actor>();
    }


    // public


    public bool Engaged()
    {
        return current_melee_targets.Count > 0 || current_ranged_targets.Count > 0;
    }


    public void AttackEnemiesInRange()
    {
        EnemyAtMeleeOrRange();
        SelectEnemy();
        StrikeEnemy();
    }


    // private


    private void EnemyAtMeleeOrRange()
    {
        ClearTargets();

        foreach (var enemy in actor.GetEnemies()) {
            if (enemy == null) continue;

            float grounded_center_distance = Vector3.Distance(new Vector3(enemy.transform.position.x, 0, enemy.transform.position.z), new Vector3(transform.position.x, 0, transform.position.z));
            float combined_radius = (enemy.GetComponent<CapsuleCollider>().radius * enemy.transform.localScale.x) + (GetComponent<CapsuleCollider>().radius * transform.localScale.x);
            float separation = grounded_center_distance - combined_radius;

            if (separation <= LongestMeleeRange()) {
                available_melee_targets.Add(enemy);
            } else if (separation <= LongestRangedRange()) {
                // targets in melee range are also potential ranged targets
                available_ranged_targets.Add(enemy);
            }
        }
    }


    private void ClearTargets()
    {
        available_melee_targets.Clear();
        available_ranged_targets.Clear();
        current_melee_targets.Clear();
        current_ranged_targets.Clear();
    }


    private float LongestMeleeRange()
    {
        float longest_range = float.MinValue;

        foreach (var weapon in available_weapons) {
            if (weapon.melee_attack_range > longest_range) {
                longest_range = weapon.melee_attack_range;
            }
        }

        return longest_range;
    }


    private float LongestRangedRange()
    {
        float longest_range = float.MinValue;

        foreach (var weapon in available_weapons) {
            if (weapon.ranged_attack_range > longest_range) {
                longest_range = weapon.ranged_attack_range;
            }
        }

        return longest_range;
    }


    private void SelectEnemy()
    {
        // attack targets in melee range before those at distance

        if (available_melee_targets.Count > 0)
        {
            current_melee_targets.Add(TargetMelee());
        }
        else if (available_ranged_targets.Count > 0)
        {
            current_ranged_targets.Add(TargetRanged());
        }
    }


    private void StrikeAtMelee()
    {
        if (current_melee_targets.Count == 0) return;

        GameObject _target = current_melee_targets[Random.Range(0, current_melee_targets.Count)];
        Vector3 swing_direction = _target.transform.position - transform.position;
        swing_direction.y = transform.position.y;

        foreach (var weapon in available_weapons)
        {
            if (weapon.range == Weapon.Range.Melee)
            {
                if (equipped_weapon == null)
                {
                    equipped_weapon = Instantiate(weapon, transform.Find("MeleeAttackOrigin").transform.position, transform.rotation);  // TODO: make enums
                    equipped_weapon.transform.position += 0.2f * swing_direction.normalized;  // TODO: give melee weapons a length
                    equipped_weapon.transform.parent = transform;
                    equipped_weapon.name = "Melee Weapon";
                }
                equipped_weapon.SetTarget(_target);
                equipped_weapon.Hit();
            }
            else if (weapon.range == Weapon.Range.Ranged)
            {

                // we have no available melee weapon, so use range on melee target

                Weapon _ranged = Instantiate(weapon, transform.Find("RangedAttackOrigin").transform.position, transform.rotation);  // TODO: make enums
                _ranged.transform.parent = transform;
                _ranged.name = "Ranged Weapon";
                _ranged.SetTarget(_target);
            }
        }
    }


    private void StrikeAtRanged()
    {
        if (current_ranged_targets.Count == 0) return;

        foreach (var weapon in available_weapons)
        {

            // TODO: potentially disadvantage ranged attacks against melee targets

            GameObject _target = current_ranged_targets[Random.Range(0, current_ranged_targets.Count)];

            if (weapon.range == Weapon.Range.Ranged)
            {
                Weapon _ranged = Instantiate(weapon, transform.Find("RangedAttackOrigin").transform.position, transform.rotation);  // TODO: make enums
                _ranged.transform.parent = transform;
                _ranged.name = "Ranged Weapon";
                _ranged.SetTarget(_target);
            }
        }
    }


    private void StrikeEnemy()
    {
        // The number of strikes is governed by haste and action_threshold in Update.

        // If any targets are in melee range, strike at them ahead of ranged

        if (current_melee_targets.Count == 0 && current_ranged_targets.Count == 0) return;

        // TODO: allow stealth to be recovered, e.g. "Vanish" and even attacking from stealth for a short while, etc.
        Stealth stealth = GetComponent<Stealth>();
        if (stealth != null) {
            stealth.attacking = true;
            stealth.spotted = true;
        }

        if (current_melee_targets.Count > 0) {
            StrikeAtMelee();
        } else { 
            StrikeAtRanged();
        }
    }


    private GameObject TargetMelee()
    {
        // select a random melee target
        GameObject _target = null;

        if (available_melee_targets.Count > 0)
        {
            _target = available_melee_targets[Random.Range(0, available_melee_targets.Count)];
        }

        return _target;
    }


    private GameObject TargetRanged()
    {
        // select a random ranged target

        GameObject _target = null;

        if (available_ranged_targets.Count > 0)
        {
            _target = available_ranged_targets[Random.Range(0, available_ranged_targets.Count)];
        }

        return _target;
    }
}
