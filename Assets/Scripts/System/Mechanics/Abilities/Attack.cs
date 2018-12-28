using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{

    public List<Weapon> available_weapons;


    // properties

    public Actor Actor { get; set; }
    public int AgilityRating { get; set; }
    public List<Actor> AvailableMeleeTargets { get; set; }
    public List<Actor> AvailableRangedTargets { get; set; }
    public List<Actor> CurrentMeleeTargets { get; set; }
    public List<Actor> CurrentRangedTargets { get; set; }
    public int StrengthRating { get; set; }
    public Weapon Weapon { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    // public


    public void AttackEnemiesInRange()
    {
        EnemyAtMeleeOrRange();
        SelectEnemy();
        StrikeEnemy();
    }


    public List<Weapon> AvailableWeapons()
    {
        return available_weapons;
    }


    public bool Engaged()
    {
        AttackEnemiesInRange();
        return CurrentMeleeTargets.Count > 0 || CurrentRangedTargets.Count > 0;
    }


    public Weapon GetWeapon()
    {
        return Weapon;
    }


    // private


    private void EnemyAtMeleeOrRange()
    {
        ClearTargets();

        for (int i = 0; i < Actor.Enemies.Count; i++) {
            Actor _enemy = Actor.Enemies[i];
            if (_enemy == null || transform == null) continue;

            float grounded_center_distance = Vector3.Distance(new Vector3(_enemy.transform.position.x, 0, _enemy.transform.position.z), new Vector3(transform.position.x, 0, transform.position.z));
            float combined_radius = (_enemy.GetComponent<CapsuleCollider>().radius * _enemy.transform.localScale.x) + (Actor.GetComponent<CapsuleCollider>().radius * transform.localScale.x);
            float separation = grounded_center_distance - combined_radius;

            if (separation <= LongestMeleeRange()) {
                AvailableMeleeTargets.Add(_enemy);
            } else if (separation <= LongestRangedRange()) {
                // targets in melee range are also potential ranged targets
                AvailableRangedTargets.Add(_enemy);
            }
        }
    }


    private void ClearTargets()
    {
        AvailableMeleeTargets.Clear();
        AvailableRangedTargets.Clear();
        CurrentMeleeTargets.Clear();
        CurrentRangedTargets.Clear();
    }


    private float LongestMeleeRange()
    {
        float longest_range = float.MinValue;

        foreach (var weapon in AvailableWeapons()) {
            if (weapon.melee_attack_range > longest_range) {
                longest_range = weapon.melee_attack_range;
            }
        }

        return longest_range;
    }


    private float LongestRangedRange()
    {
        float longest_range = float.MinValue;

        foreach (var weapon in AvailableWeapons()) {
            if (weapon.ranged_attack_range > longest_range) {
                longest_range = weapon.ranged_attack_range;
            }
        }

        return longest_range;
    }


    private void SelectEnemy()
    {
        // attack targets in melee range before those at distance

        if (AvailableMeleeTargets.Count > 0) {
            CurrentMeleeTargets.Add(TargetMelee());
        } else if (AvailableRangedTargets.Count > 0) {
            CurrentRangedTargets.Add(TargetRanged());
        }
    }


    private void SetComponents()
    {
        Actor = GetComponentInParent<Actor>();
        AvailableMeleeTargets = new List<Actor>();
        AvailableRangedTargets = new List<Actor>();
        CurrentMeleeTargets = new List<Actor>();
        CurrentRangedTargets = new List<Actor>();
    }


    private void StrikeAtMelee()
    {
        if (CurrentMeleeTargets.Count == 0) return;

        Actor _target = CurrentMeleeTargets[0];
        if (_target == null || transform == null) return;

        Vector3 swing_direction = _target.transform.position - transform.position;
        swing_direction.y = transform.position.y;

        foreach (var weapon in AvailableWeapons()) {
            if (weapon.range == Weapon.Range.Melee) {
                if (Weapon == null) {
                    Weapon = Instantiate(weapon, transform.parent.Find("MeleeAttackOrigin").transform.position, transform.rotation);  // TODO: make enums
                    Weapon.transform.position += 0.2f * swing_direction.normalized;  // TODO: give melee weapons a length
                    Weapon.transform.parent = transform;
                    Weapon.name = "Melee Weapon";
                }
                Weapon.SetTarget(_target);
                Weapon.Hit();
            } else if (weapon.range == Weapon.Range.Ranged) {

                // we have no available melee weapon, so use range on melee target

                Weapon _ranged = Instantiate(weapon, transform.parent.Find("RangedAttackOrigin").transform.position, transform.rotation);  // TODO: make enums
                _ranged.transform.parent = transform;
                _ranged.name = "Ranged Weapon";
                _ranged.SetTarget(_target);
            }
        }
    }


    private void StrikeAtRanged()
    {
        if (CurrentRangedTargets.Count == 0) return;

        foreach (var weapon in AvailableWeapons()) {

            // TODO: potentially disadvantage ranged attacks against melee targets

            Actor _target = CurrentRangedTargets[0];

            if (weapon.range == Weapon.Range.Ranged) {
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

        if (CurrentMeleeTargets.Count == 0 && CurrentRangedTargets.Count == 0) return;

        // TODO: allow stealth to be recovered, e.g. "Vanish" and even attacking from stealth for a short while, etc.
        Stealth stealth = Actor.Stealth;
        if (stealth != null) {
            stealth.attacking = true;
            stealth.spotted = true;
        }

        if (CurrentMeleeTargets.Count > 0) {
            StrikeAtMelee();
        } else { 
            StrikeAtRanged();
        }
    }


    private Actor TargetMelee()
    {
        // TODO: attack the biggest threat

        Actor _target = null;

        if (AvailableMeleeTargets.Count > 0) {
            _target = AvailableMeleeTargets[0];
        }

        return _target;
    }


    private Actor TargetRanged()
    {
        // TODO: attack the biggest threat

        Actor _target = null;

        if (AvailableRangedTargets.Count > 0) {
            _target = AvailableRangedTargets[0];
        }

        return _target;
    }
}
