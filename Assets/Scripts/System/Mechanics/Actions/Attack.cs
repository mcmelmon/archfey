using System.Collections;
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
        // TODO: attack the PrimaryThreat chosen by Decider, not just one from "available targets" (which is still important for range-finding)

        Me.Actions.Movement.Agent.speed = Me.Actions.Movement.Speed;
        SelectEnemy();
        StrikeEnemy();
    }


    public void SetEnemyRanges()
    {
        if (Me == null) return;
        
        ClearTargets();

        AvailableMeleeTargets.AddRange(Me.Senses.Actors
                                         .Where(actor => !Me.Actions.Decider.IsFriendOrNeutral(actor) && Vector3.Distance(transform.position, actor.transform.position) < LongestMeleeRange())
                                         .OrderBy(actor => actor.Health.CurrentHitPoints)
                                         .Select(actor => actor.gameObject)
                                         .ToList());

        AvailableMeleeTargets.AddRange(Me.Senses.Structures
                                         .Where(structure => structure.alignment != Me.Alignment && Vector3.Distance(transform.position, structure.transform.position) < LongestMeleeRange())
                                         .Select(structure => structure.gameObject)
                                         .ToList());

        AvailableRangedTargets.AddRange(Me.Senses.Actors
                                          .Where(actor => !Me.Actions.Decider.IsFriendOrNeutral(actor) && Vector3.Distance(transform.position, actor.transform.position) < LongestRangedRange())
                                          .OrderBy(actor => actor.Health.CurrentHitPoints)
                                          .Select(actor => actor.gameObject)
                                          .ToList());

        AvailableRangedTargets.AddRange(Me.Senses.Structures
                                          .Where(structure => structure.alignment != Me.Alignment && Vector3.Distance(transform.position, structure.transform.position) < LongestRangedRange())
                                          .Select(structure => structure.gameObject)
                                          .ToList());
    }


    public bool Engaged()
    {
        return AvailableMeleeTargets.Count > 0 || AvailableRangedTargets.Count > 0;
    }


    public void EquipMeleeWeapon()
    {
        if (EquippedMeleeWeapon == null) {
            foreach (var weapon in AvailableWeapons) {
                if (weapon.range == 0) {
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
        if (EquippedRangedWeapon == null) {
            foreach (var weapon in AvailableWeapons) {
                if (weapon.range > 0) {
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


    private void RemoveSanctuaryTargets()
    {
        List<GameObject> protected_melee_actors = AvailableMeleeTargets
            .Where(target => target.GetComponent<Actor>() != null && Sanctuary.ProtectedTargets.ContainsKey(target.GetComponent<Actor>()) && !Me.Actions.Decider.Threat.Threats.ContainsKey(target.GetComponent<Actor>()))
            .ToList();

        List<GameObject> protected_ranged_actors = AvailableRangedTargets
            .Where(target => target.GetComponent<Actor>() != null && Sanctuary.ProtectedTargets.ContainsKey(target.GetComponent<Actor>()) && !Me.Actions.Decider.Threat.Threats.ContainsKey(target.GetComponent<Actor>()))
            .ToList();

        foreach (var target in protected_melee_actors) {
            if (!Me.Actions.RollSavingThrow(Proficiencies.Attribute.Wisdom, Sanctuary.ProtectedTargets[target.GetComponent<Actor>()].ChallengeRating)) {
                AvailableMeleeTargets.Remove(target);
            }
        }

        foreach (var target in protected_ranged_actors) {
            if (!Me.Actions.RollSavingThrow(Proficiencies.Attribute.Wisdom, Sanctuary.ProtectedTargets[target.GetComponent<Actor>()].ChallengeRating)) {
                AvailableMeleeTargets.Remove(target);
            }
        }
    }


    private void SelectEnemy()
    {
        // attack targets in melee range before those at distance

        // TODO: the Decider should pick the target, based on priority preferences

        RemoveSanctuaryTargets();

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
        return AvailableMeleeTargets.Count > 0 ? AvailableMeleeTargets[0] : null;
    }


    private GameObject TargetRanged()
    {
        return AvailableRangedTargets.Count > 0 ? AvailableRangedTargets[0] : null;
    }
}
