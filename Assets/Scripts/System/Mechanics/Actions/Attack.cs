using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    // properties

    public Actor Me { get; set; }
    public int AttackRating { get; set; }
    public List<Weapon> AvailableWeapons { get; set; }
    public List<GameObject> AvailableMeleeTargets { get; set; }
    public List<GameObject> AvailableRangedTargets { get; set; }
    public List<GameObject> CurrentMeleeTargets { get; set; }
    public List<GameObject> CurrentRangedTargets { get; set; }
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
        EnemyAtMeleeOrRange();
        SelectEnemy();
        StrikeEnemy();
    }


    public bool Engaged()
    {
        return CurrentMeleeTargets.Count > 0 || CurrentRangedTargets.Count > 0;
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
        CurrentMeleeTargets.Clear();
        CurrentRangedTargets.Clear();
    }


    private void EnemyAtMeleeOrRange()
    {
        if (transform == null) return;

        ClearTargets();

        for (int i = 0; i < Me.Actions.Decider.Enemies.Count; i++) {
            Actor foe = Me.Actions.Decider.Enemies[i];

            float grounded_center_distance = Vector3.Distance(new Vector3(foe.transform.position.x, 0, foe.transform.position.z), new Vector3(transform.position.x, 0, transform.position.z));
            float combined_radius = (foe.GetComponent<CapsuleCollider>().radius * foe.transform.localScale.x) + (Me.GetComponent<CapsuleCollider>().radius * transform.localScale.x);
            float separation = grounded_center_distance - combined_radius;

            if (separation <= LongestMeleeRange()) {
                AvailableMeleeTargets.Add(foe.gameObject);
            } else if (separation <= LongestRangedRange()) {
                // targets in melee range are also potential ranged targets
                AvailableRangedTargets.Add(foe.gameObject);
            }
        }

        for (int i = 0; i < Me.Actions.Decider.Structures.Count; i++) {
            Structure structure = Me.Actions.Decider.Structures[i];

            Vector3 destination = structure.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            float separation = Vector3.Distance(destination, transform.position);

            if (separation <= LongestMeleeRange()) {
                AvailableMeleeTargets.Add(structure.gameObject);
            } else if (separation <= LongestRangedRange()) {
                // targets in melee range are also potential ranged targets
                AvailableRangedTargets.Add(structure.gameObject);
            }
        }
    }


    private float LongestMeleeRange()
    {
        foreach (var weapon in AvailableWeapons) {
            if (weapon.range == 0 && weapon.has_reach) {
                return 2f;
            } else if (weapon.range == 0) {
                return 1f;
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
            CurrentMeleeTargets.Add(TargetMelee());
        } else if (AvailableRangedTargets.Count > 0) {
            CurrentRangedTargets.Add(TargetRanged());
        }
    }


    private void SetComponents()
    {
        Me = GetComponentInParent<Actor>();
        AvailableMeleeTargets = new List<GameObject>();
        AvailableRangedTargets = new List<GameObject>();
        CurrentMeleeTargets = new List<GameObject>();
        CurrentRangedTargets = new List<GameObject>();
    }


    private void StrikeEnemy()
    {
        // The number of strikes is governed by haste and action_threshold in Update.

        // If any targets are in melee range, strike at them ahead of ranged

        if (CurrentMeleeTargets.Count == 0 && CurrentRangedTargets.Count == 0) return;

        // TODO: handle in Stealth; allow stealth to be recovered, e.g. "Vanish" and even attacking from stealth for a short while, etc.
        Stealth stealth = Me.Actions.Stealth;
        if (stealth != null) {
            stealth.Attacking = true;
            stealth.Seen = true;
        }

        // TODO: add more powerful, energy based attacks
        if (CurrentMeleeTargets.Count > 0) {
            if (EquippedRangedWeapon != null) EquippedRangedWeapon.gameObject.SetActive(false);
            GetComponent<DefaultMelee>().Strike(CurrentMeleeTargets[0]);
        } else {
            if (EquippedMeleeWeapon != null) EquippedMeleeWeapon.gameObject.SetActive(false);
            GetComponent<DefaultRange>().Strike(CurrentRangedTargets[0]);
        }
    }


    private GameObject TargetMelee()
    {
        // TODO: attack the biggest threat

        GameObject _target = null;

        if (AvailableMeleeTargets.Count > 0) {
            _target = AvailableMeleeTargets[0];
        }

        return _target;
    }


    private GameObject TargetRanged()
    {
        GameObject _target = null;

        if (AvailableRangedTargets.Count > 0) {
            _target = AvailableRangedTargets[0];
        }

        return _target;
    }
}
