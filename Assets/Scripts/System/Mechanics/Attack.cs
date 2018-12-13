using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {

    public List<Weapon> available_weapons;
    public bool enemies_abound;

    List<GameObject> enemies = new List<GameObject>();
    List<GameObject> available_melee_targets = new List<GameObject>();
    List<GameObject> available_ranged_targets = new List<GameObject>();
    private readonly List<GameObject> current_melee_targets = new List<GameObject>();  // List instead of Queue to facilitate returning all targets
    private readonly List<GameObject> current_ranged_targets = new List<GameObject>();

    Turn turn;
    Mhoddim mhoddim;
    Ghaddim ghaddim;
    Fey fey;
    Senses senses;
    Health health;
    Weapon equipped_weapon;

    // Unity


    private void Awake()
    {
        turn = GetComponent<Turn>();
        mhoddim = GetComponent<Mhoddim>();
        ghaddim = GetComponent<Ghaddim>();
        fey = GetComponent<Fey>();
        senses = GetComponent<Senses>();
        health = GetComponent<Health>();
        enemies_abound = false;
    }


    // public


    public void DiscoverEnemies()
    {
        enemies.Clear();

        foreach (KeyValuePair<GameObject, float> damager in health.GetDamagers()) {  // TODO: name "keyValue" better in other Dict iterators
            // TODO: include notion of "linking"
            if (damager.Key != null) enemies.Add(damager.Key);
        }

        foreach (var sighting in senses.GetSightings()) {
            if (sighting.tag == "Player") continue;
            if (sighting == gameObject) continue; // we can "sight" ourselves (potential heal target), but not as an enemy
            if (IsFriendOrNeutral(sighting)) continue;
            if (!enemies.Contains(sighting)) enemies.Add(sighting);
        }

        enemies_abound = enemies.Count > 0 ? true : false;
    }


    public List<GameObject> GetCurrentEnemies()
    {
        return enemies;
    }


    public GameObject GetAnEnemy()
    {
        return enemies.Count > 0 ? enemies[Random.Range(0, enemies.Count)] : null;
    }


    public IEnumerator ManageAttacks()
    {
        DiscoverEnemies();

        if (enemies_abound) {
            EnemyAtMeleeOrRange();
            SelectEnemy();
            StrikeEnemy();
        }

        yield return null;
    }


    // private

    private void EnemyAtMeleeOrRange()
    {
        ClearTargets();

        foreach (var enemy in enemies) {
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


    private bool IsFriendOrNeutral(GameObject _target)
    {
        if (_target == null) return true;  // null is everyone's friend, or at least not their enemy

        if (fey != null) return false; // Ents hate everyone

        Mhoddim target_mhoddim = _target.GetComponent<Mhoddim>();
        Ghaddim target_ghaddim = _target.GetComponent<Ghaddim>();

        bool friend_or_neutral = (mhoddim == null && target_mhoddim == null) || (ghaddim == null && target_ghaddim == null); 

        return friend_or_neutral;
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


    private GameObject MeleeTarget()
    {
        // select a random melee target
        GameObject _target = null;

        if (available_melee_targets.Count > 0) {
            _target = available_melee_targets[Random.Range(0, available_melee_targets.Count)];
        }

        return _target;
    }


    private GameObject RangedTarget()
    {
        // select a random ranged target

        GameObject _target = null;

        if (available_ranged_targets.Count > 0) {
            _target = available_ranged_targets[Random.Range(0, available_ranged_targets.Count)];
        }

        return _target;
    }


    private void SelectEnemy()
    {
        // attack targets in melee range before those at distance
        
        if (available_melee_targets.Count > 0) {
            current_melee_targets.Add(MeleeTarget());
        } else if (available_ranged_targets.Count > 0) {
            current_ranged_targets.Add(RangedTarget());
        }
    }


    private void Melee()
    {
        if (current_melee_targets.Count == 0) return;

        GameObject _target = current_melee_targets[Random.Range(0, current_melee_targets.Count)];
        Vector3 swing_direction = _target.transform.position - transform.position;
        swing_direction.y = transform.position.y;

        foreach (var weapon in available_weapons) {
            if (weapon.range == Weapon.Range.Melee) {
                if (equipped_weapon == null) {
                    equipped_weapon = Instantiate(weapon, transform.Find("MeleeAttackOrigin").transform.position, transform.rotation);  // TODO: make enums
                    equipped_weapon.transform.position += 0.2f * swing_direction.normalized;  // TODO: give melee weapons a length
                    equipped_weapon.transform.parent = transform;
                    equipped_weapon.name = "Melee Weapon";
                }
                equipped_weapon.Target(_target);
            } else if (weapon.range == Weapon.Range.Ranged) {
                Weapon _ranged = Instantiate(weapon, transform.Find("RangedAttackOrigin").transform.position, transform.rotation);  // TODO: make enums
                _ranged.transform.parent = transform;
                _ranged.name = "Ranged Weapon";
                _ranged.Target(_target);
            }
        }
    }


    private void Ranged()
    {
        if (current_ranged_targets.Count == 0) return;

        foreach (var weapon in available_weapons) {

            // TODO: potentially disadvantage ranged attacks against melee targets

            GameObject _target = current_ranged_targets[Random.Range(0, current_ranged_targets.Count)];

            if (weapon.range == Weapon.Range.Ranged) {
                Weapon _ranged = Instantiate(weapon, transform.Find("RangedAttackOrigin").transform.position, transform.rotation);  // TODO: make enums
                _ranged.transform.parent = transform;
                _ranged.name = "Ranged Weapon";
                _ranged.Target(_target);
            }
        }
    }

    private void StrikeEnemy()
    {
        // The number of strikes is governed by haste and action_threshold in Update.

        // If any targets are in melee range, strike at them ahead of ranged

        if (current_melee_targets.Count == 0 && current_ranged_targets.Count == 0) return;

        if (current_melee_targets.Count > 0) {
            Melee();
        } else { 
            Ranged();
        }
    }
}
