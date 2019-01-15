﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Attack : MonoBehaviour
{
    // properties

    public Actor Me { get; set; }
    public int AttackRating { get; set; }
    public List<Weapon> AvailableWeapons { get; set; }
    public List<GameObject> AvailableMeleeTargets { get; set; }
    public List<GameObject> AvailableRangedTargets { get; set; }
    public GameObject CurrentMeleeTarget { get; set; }
    public GameObject CurrentRangedTarget { get; set; }
    public Weapon EquippedMeleeWeapon { get; set; }
    public Weapon EquippedRangedWeapon { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    // public


    public void AttackEnemiesInRange()
    {
        SelectEnemy();
        StrikeEnemy();
    }


    public void EnemyAtMeleeOrRange()
    {
        if (transform == null) return;

        ClearTargets();

        AvailableMeleeTargets.AddRange(Me.Senses.Actors
                                         .Where(actor => !Me.Actions.Decider.IsFriendOrNeutral(actor) && Vector3.Distance(transform.position, actor.transform.position) < LongestMeleeRange())
                                         .OrderBy(actor => actor.Health.CurrentHitPoints)
                                         .Reverse()
                                         .Select(actor => actor.gameObject)
                                         .ToList());

        AvailableMeleeTargets.AddRange(Me.Senses.Structures
                                         .Where(structure => structure.owner != Me.Faction && Vector3.Distance(transform.position, structure.transform.position) < LongestMeleeRange())
                                         .Select(structure => structure.gameObject)
                                         .ToList());

        AvailableRangedTargets.AddRange(Me.Senses.Actors
                                          .Where(actor => !Me.Actions.Decider.IsFriendOrNeutral(actor) && Vector3.Distance(transform.position, actor.transform.position) < LongestRangedRange())
                                          .OrderBy(actor => actor.Health.CurrentHitPoints)
                                          .Reverse()
                                          .Select(actor => actor.gameObject)
                                          .ToList());

        AvailableRangedTargets.AddRange(Me.Senses.Structures
                                          .Where(structure => structure.owner != Me.Faction && Vector3.Distance(transform.position, structure.transform.position) < LongestRangedRange())
                                          .Select(structure => structure.gameObject)
                                          .ToList());
    }


    public bool Engaged()
    {
        return AvailableMeleeTargets.Count > 0 || AvailableRangedTargets.Count > 0;
    }


    public void EquipMeleeWeapon()
    {
        if (EquippedMeleeWeapon == null)
        {
            foreach (var weapon in AvailableWeapons)
            {
                if (weapon.range == 0)
                {
                    EquippedMeleeWeapon = Instantiate(weapon, transform.Find("AttackOrigin").transform.position, transform.rotation);
                    EquippedMeleeWeapon.transform.position += 0.2f * Vector3.forward;
                    EquippedMeleeWeapon.transform.parent = transform;
                    EquippedMeleeWeapon.name = "Melee Weapon";
                    EquippedMeleeWeapon.gameObject.SetActive(false);
                    break;  // TODO: pick best available melee weapon based on resistances, etc.
                }
            }
        }
    }


    public void EquipRangedWeapon()
    {
        if (EquippedRangedWeapon == null)
        {
            foreach (var weapon in AvailableWeapons)
            {
                if (weapon.range > 0)
                {
                    EquippedRangedWeapon = Instantiate(weapon, transform.Find("AttackOrigin").transform.position, transform.rotation);
                    EquippedRangedWeapon.transform.position += 0.2f * Vector3.forward;
                    EquippedRangedWeapon.transform.parent = transform;
                    EquippedRangedWeapon.name = "Ranged Weapon";
                    EquippedRangedWeapon.gameObject.SetActive(false);
                    break;
                }
            }
        }
    }


    // private


    private void ClearTargets()
    {
        AvailableMeleeTargets.Clear();
        AvailableRangedTargets.Clear();
        CurrentMeleeTarget = null;
        CurrentRangedTarget = null;
    }


    private float LongestMeleeRange()
    {
        foreach (var weapon in AvailableWeapons) {
            if (weapon.range == 0 && weapon.has_reach) {
                return 3f;
            } else if (weapon.range == 0) {
                return 2f;
            }
        }

        return 0f;
    }


    private float LongestRangedRange()
    {
        // TODO: select the weapon appropriate for the range

        float longest_range = float.MinValue;

        foreach (var weapon in AvailableWeapons) {
            if (weapon.range > longest_range) {
                longest_range = weapon.range;
            }
        }

        return longest_range;
    }


    private void SelectEnemy()
    {
        // attack targets in melee range before those at distance

        if (AvailableMeleeTargets.Count > 0) {
            CurrentMeleeTarget = TargetMelee();
        } else if (AvailableRangedTargets.Count > 0) {
            CurrentRangedTarget = TargetRanged();
        }
    }


    private void SetComponents()
    {
        Me = GetComponentInParent<Actor>();
        AvailableMeleeTargets = new List<GameObject>();
        AvailableRangedTargets = new List<GameObject>();
    }


    private void StrikeEnemy()
    {
        // The number of strikes is governed by haste and action_threshold in Update.

        // If any targets are in melee range, strike at them ahead of ranged

        if (CurrentMeleeTarget == null && CurrentRangedTarget == null) return;

        // TODO: handle in Stealth; allow stealth to be recovered, e.g. "Vanish" and even attacking from stealth for a short while, etc.
        Stealth stealth = Me.Actions.Stealth;
        if (stealth != null) {
            stealth.Attacking = true;
            stealth.Seen = true;
        }

        // TODO: add more powerful, energy based attacks
        if (CurrentMeleeTarget != null) {
            if (EquippedRangedWeapon != null) EquippedRangedWeapon.gameObject.SetActive(false);
            GetComponent<DefaultMelee>().Strike(CurrentMeleeTarget);
        } else {
            if (EquippedMeleeWeapon != null) EquippedMeleeWeapon.gameObject.SetActive(false);
            GetComponent<DefaultRange>().Strike(CurrentRangedTarget);
        }
    }


    private GameObject TargetMelee()
    {
        if (AvailableMeleeTargets.Count > 0) {
            return AvailableMeleeTargets[0];
        }

        return null;
    }


    private GameObject TargetRanged()
    {
        if (AvailableRangedTargets.Count > 0)
        {
            return AvailableRangedTargets[0];
        }

        return null;
    }
}
