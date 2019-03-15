using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Attack : MonoBehaviour
{
    // properties

    public int AttacksPerAction { get; set; }
    public List<GameObject> AvailableMeleeTargets { get; set; }
    public List<GameObject> AvailableRangedTargets { get; set; }
    public AdditionalDamage CalculateAdditionalDamage { get; set; }
    public int CriticalRangeStart { get; set; }
    public GameObject CurrentMeleeTarget { get; set; }
    public GameObject CurrentRangedTarget { get; set; }
    public Armor EquippedArmor { get; set; }
    public Weapon EquippedMeleeWeapon { get; set; }
    public Weapon EquippedRangedWeapon { get; set; }
    public Weapon EquippedOffhand { get; set; }
    public Actor Me { get; set; }
    public bool Raging { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    // public


    public delegate int AdditionalDamage(bool is_ranged);


    public void AttackEnemiesInRange(GameObject player_target = null)
    {
        for (int i = 0; i < AttacksPerAction; i++) {
            if (player_target == null) SelectEnemy();
            StrikeEnemy(player_target);
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

            if (EquippedRangedWeapon != null) {
                AvailableRangedTargets.AddRange(Me.Actions.Decider.Enemies
                                                .Where(actor => actor != null && Me.SeparationFrom(actor) > melee_range && Me.SeparationFrom(actor) <= EquippedRangedWeapon.Range)
                                                .OrderBy(actor => actor.Health.CurrentHitPoints)
                                                .Select(actor => actor.gameObject)
                                                .Distinct()
                                                .ToList());
            }
        }

        if (Me.Actions.Decider.HostileStructures.Any()) {
            AvailableMeleeTargets.AddRange(Me.Actions.Decider.HostileStructures
                                           .Where(structure => Vector3.Distance(transform.position, structure.GetInteractionPoint(Me)) <= melee_range + Me.Actions.Movement.ReachedThreshold)
                                           .Select(structure => structure.gameObject)
                                           .Distinct()
                                           .ToList());

            if (EquippedRangedWeapon != null) {
                AvailableRangedTargets.AddRange(Me.Actions.Decider.HostileStructures
                                                .Where(structure => Vector3.Distance(transform.position, structure.GetInteractionPoint(Me)) > melee_range && Vector3.Distance(transform.position, structure.GetInteractionPoint(Me)) <= EquippedRangedWeapon.Range + Me.Actions.Movement.ReachedThreshold)
                                                .Select(structure => structure.gameObject)
                                                .Distinct()
                                                .ToList());
            }
        }
    }


    public bool Engaged()
    {
        return AvailableMeleeTargets.Count > 0 || AvailableRangedTargets.Count > 0;
    }


    public void EquipArmor(Armor armor)
    {
        EquippedArmor = Instantiate(armor, transform.position, transform.rotation);
        EquippedArmor.transform.parent = transform;
        EquippedArmor.name = "Armor";
        EquippedArmor.gameObject.SetActive(false);
    }


    // TODO: weapon sets; enforce offhand rules
    public void EquipMeleeWeapon(Weapon weapon)
    {    
        EquippedMeleeWeapon = Instantiate(weapon, Me.weapon_transform.position, transform.rotation);
        EquippedMeleeWeapon.transform.parent = Me.weapon_transform;
        EquippedMeleeWeapon.name = "Melee Weapon";
        EquippedMeleeWeapon.gameObject.SetActive(false);
    }


    public void EquipOffhand(Weapon weapon)
    {
        EquippedOffhand = Instantiate(weapon, Me.offhand_transform.position, transform.rotation);
        EquippedOffhand.transform.parent = Me.offhand_transform;
        EquippedOffhand.name = "Offhand";
        EquippedOffhand.gameObject.SetActive(false);
    }


    public void EquipRangedWeapon(Weapon weapon)
    {
        EquippedRangedWeapon = Instantiate(weapon, Me.weapon_transform.position, transform.rotation);
        EquippedRangedWeapon.transform.parent = Me.weapon_transform;
        EquippedRangedWeapon.name = "Ranged Weapon";
        EquippedRangedWeapon.gameObject.SetActive(false);
    }


    public void EquipShield(Armor shield)
    {
        EquippedOffhand = Instantiate(shield, Me.offhand_transform.position, transform.rotation).GetComponent<Weapon>();
        EquippedOffhand.transform.parent = Me.offhand_transform;
        EquippedOffhand.name = "Shield";
        EquippedOffhand.gameObject.SetActive(false);
    }


    public bool IsAttackable(GameObject target)
    {
        return target.GetComponent<Actor>() != null || target.GetComponent<Structure>() != null;
    }


    public bool IsWithinAttackRange(Transform target)
    {
        float separation = Vector3.Distance(target.transform.position, transform.position);
        return (EquippedRangedWeapon != null) ? separation < EquippedRangedWeapon.Range : separation < MeleeRange() + 1f;
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
            : EquippedMeleeWeapon.HasReach
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
        AttacksPerAction = 1;
        CalculateAdditionalDamage = DefaultAdditionalDamage;
        CriticalRangeStart = 20;
        Me = GetComponentInParent<Actor>();
        AvailableMeleeTargets = new List<GameObject>();
        AvailableRangedTargets = new List<GameObject>();
    }


    private void StrikeEnemy(GameObject player_target = null)
    {
        if (player_target != null) {
            TargetPlayerChoice(player_target);
            return;
        }

        if (CurrentMeleeTarget == null && CurrentRangedTarget == null) return;

        if (CurrentMeleeTarget != null) {
            EquippedMeleeWeapon.gameObject.SetActive(true);
            if (EquippedOffhand != null) EquippedOffhand.gameObject.SetActive(true);
            if (EquippedRangedWeapon != null) EquippedRangedWeapon.gameObject.SetActive(false);
            GetComponent<DefaultMelee>().Strike(CurrentMeleeTarget);
            Me.Actions.Stealth.Appear(); // appear after the strike to ensure sneak attack damage, etc
        } else {
            EquippedRangedWeapon.gameObject.SetActive(true);
            if (EquippedMeleeWeapon != null) EquippedMeleeWeapon.gameObject.SetActive(false);
            if (EquippedOffhand != null) EquippedOffhand.gameObject.SetActive(false);
            GetComponent<DefaultRange>().Strike(CurrentRangedTarget);
            Me.Actions.Stealth.Appear(); // appear after the strike to ensure sneak attack damage, etc
        }
    }


    private GameObject TargetMelee()
    {
        return AvailableMeleeTargets.Count > 0 ? AvailableMeleeTargets[0] : null;
    }


    private void TargetPlayerChoice(GameObject player_target)
    {
        if (Vector3.Distance(player_target.transform.position, transform.position) < MeleeRange() + 1) {
            EquippedMeleeWeapon.gameObject.SetActive(true);
            if (EquippedOffhand != null) EquippedOffhand.gameObject.SetActive(true);
            if (EquippedRangedWeapon != null) EquippedRangedWeapon.gameObject.SetActive(false);
            GetComponent<DefaultMelee>().Strike(player_target);
            Me.Actions.Stealth.Appear(); // appear after the strike to ensure sneak attack damage, etc
        } else if (EquippedRangedWeapon != null) {
            EquippedRangedWeapon.gameObject.SetActive(true);
            if (EquippedMeleeWeapon != null) EquippedMeleeWeapon.gameObject.SetActive(false);
            if (EquippedOffhand != null) EquippedOffhand.gameObject.SetActive(false);
            GetComponent<DefaultRange>().Strike(player_target);
            Me.Actions.Stealth.Appear(); // appear after the strike to ensure sneak attack damage, etc
        }
    }

    private GameObject TargetRanged()
    {
        return AvailableRangedTargets.Count > 0 ? AvailableRangedTargets[0] : null;
    }
}
