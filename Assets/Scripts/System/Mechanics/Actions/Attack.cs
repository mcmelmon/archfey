using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Attack : MonoBehaviour
{
    // properties

    public int AttacksPerAction { get; set; }
    public List<Weapon> AvailableWeapons { get; set; }
    public List<GameObject> AvailableMeleeTargets { get; set; }
    public List<GameObject> AvailableRangedTargets { get; set; }
    public AdditionalDamage CalculateAdditionalDamage { get; set; }
    public int CriticalRangeStart { get; set; }
    public GameObject CurrentMeleeTarget { get; set; }
    public GameObject CurrentRangedTarget { get; set; }
    public Weapon EquippedMeleeWeapon { get; set; }
    public Weapon EquippedRangedWeapon { get; set; }
    public Actor Me { get; set; }
    public bool Raging { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    // public


    public delegate int AdditionalDamage(bool is_ranged);


    public void AttackEnemiesInRange()
    {
        // TODO: attack the PrimaryThreat chosen by Decider, not just one from "available targets" (which is still important for range-finding)

        for (int i = 0; i < AttacksPerAction; i++) {
            SelectEnemy();
            StrikeEnemy();
        }
    }


    public int DefaultAdditionalDamage(bool is_ranged)
    {
        return 0;
    }


    public bool HasSurprise(Actor other_actor)
    {
        return !Me.Actions.Stealth.SpottedBy(other_actor);
    }


    public void SetEnemyRanges()
    {
        if (Me == null) return;

        ClearTargets();
        float melee_range = MeleeRange();

        if (Me.Actions.Decider.Enemies.Any()) {
            AvailableMeleeTargets.AddRange(Me.Actions.Decider.Enemies
                                           .Where(actor => actor != null && Me.SeparationFrom(actor) <= melee_range)
                                           .OrderBy(actor => actor.Health.CurrentHitPoints)
                                           .Select(actor => actor.gameObject)
                                           .Distinct()
                                           .ToList());

            AvailableRangedTargets.AddRange(Me.Actions.Decider.Enemies
                                            .Where(actor => actor != null && Me.SeparationFrom(actor) > melee_range && Me.SeparationFrom(actor) <= EquippedRangedWeapon.range)
                                            .OrderBy(actor => actor.Health.CurrentHitPoints)
                                            .Select(actor => actor.gameObject)
                                            .Distinct()
                                            .ToList());
        }

        if (Me.Actions.Decider.HostileStructures.Any()) {
            AvailableMeleeTargets.AddRange(Me.Actions.Decider.HostileStructures
                                           .Where(structure => Vector3.Distance(transform.position, structure.GetInteractionPoint(Me)) <= melee_range + Me.Actions.Movement.ReachedThreshold)
                                           .Select(structure => structure.gameObject)
                                           .Distinct()
                                           .ToList());

            AvailableRangedTargets.AddRange(Me.Actions.Decider.HostileStructures
                                            .Where(structure => Vector3.Distance(transform.position, structure.GetInteractionPoint(Me)) > melee_range && Vector3.Distance(transform.position, structure.GetInteractionPoint(Me)) <= EquippedRangedWeapon.range + Me.Actions.Movement.ReachedThreshold)
                                            .Select(structure => structure.gameObject)
                                            .Distinct()
                                            .ToList());
        }
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


    private float MeleeRange()
    {
        return EquippedMeleeWeapon == null
            ? 0f
            : EquippedMeleeWeapon.range == 0 && EquippedMeleeWeapon.has_reach
            ? Me.Actions.Movement.ReachedThreshold + 2f
            : Me.Actions.Movement.ReachedThreshold + 1f;
    }


    private void RemoveSanctuaryTargets()
    {
        if (Sanctuary.ProtectedTargets == null) return;

        List<GameObject> protected_melee_actors = AvailableMeleeTargets
            .Where(target => target.GetComponent<Actor>() != null && Sanctuary.ProtectedTargets.ContainsKey(target.GetComponent<Actor>()) && !Me.Actions.Decider.Threat.Threats.ContainsKey(target.GetComponent<Actor>()))
            .ToList();

        List<GameObject> protected_ranged_actors = AvailableRangedTargets
            .Where(target => target.GetComponent<Actor>() != null && Sanctuary.ProtectedTargets.ContainsKey(target.GetComponent<Actor>()) && !Me.Actions.Decider.Threat.Threats.ContainsKey(target.GetComponent<Actor>()))
            .ToList();

        foreach (var target in protected_melee_actors) {
            if (!Me.Actions.SavingThrow(Proficiencies.Attribute.Wisdom, Sanctuary.ProtectedTargets[target.GetComponent<Actor>()].ChallengeRating)) {
                AvailableMeleeTargets.Remove(target);
            }
        }

        foreach (var target in protected_ranged_actors) {
            if (!Me.Actions.SavingThrow(Proficiencies.Attribute.Wisdom, Sanctuary.ProtectedTargets[target.GetComponent<Actor>()].ChallengeRating)) {
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
            transform.LookAt(CurrentMeleeTarget.transform);
        } else if (AvailableRangedTargets.Count > 0) {
            CurrentRangedTarget = TargetRanged();
            transform.LookAt(CurrentRangedTarget.transform);
        }
    }


    private void SetComponents()
    {
        CalculateAdditionalDamage = DefaultAdditionalDamage;
        CriticalRangeStart = 20;
        Me = GetComponentInParent<Actor>();
        AvailableMeleeTargets = new List<GameObject>();
        AvailableRangedTargets = new List<GameObject>();
    }


    private void StrikeEnemy()
    {
        // If any targets are in melee range, strike at them ahead of ranged

        if (CurrentMeleeTarget == null && CurrentRangedTarget == null) return;

        if (CurrentMeleeTarget != null) {
            if (EquippedRangedWeapon != null) EquippedRangedWeapon.gameObject.SetActive(false);
            GetComponent<DefaultMelee>().Strike(CurrentMeleeTarget);
            Me.Actions.Stealth.Appear(); // appear after the strike to ensure sneak attack damage, etc
        } else {
            if (EquippedMeleeWeapon != null) EquippedMeleeWeapon.gameObject.SetActive(false);
            GetComponent<DefaultRange>().Strike(CurrentRangedTarget);
            Me.Actions.Stealth.Appear(); // appear after the strike to ensure sneak attack damage, etc
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
