using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {

    public const float action_threshold = 6f;

    public List<Weapon> available_weapons;
    public int number_of_attacks;

    List<GameObject> available_melee_targets = new List<GameObject>();
    List<GameObject> available_ranged_targets = new List<GameObject>();
    private readonly Queue<GameObject> current_melee_targets = new Queue<GameObject>();
    private readonly Queue<GameObject> current_ranged_targets = new Queue<GameObject>();

    Mhoddim mhoddim;
    Ghaddim ghaddim;
    Fey fey;
    Senses senses;

    readonly float haste_delta = 1f;      // TODO: configure by actor
    float current_haste;

    // Unity


    private void Awake()
    {
        mhoddim = GetComponent<Mhoddim>();
        ghaddim = GetComponent<Ghaddim>();
        fey = GetComponent<Fey>();
        senses = GetComponent<Senses>();
        current_haste = 0f;
    }


    private void Update () {
        if (current_haste >= action_threshold) {
            StartCoroutine(ManageAttacks());
            current_haste = 0f; 
        } else {
            current_haste += haste_delta * Time.deltaTime;
        }
    }


    // public


    // private

    private void CategorizePotentialTargets()
    {
        ClearAvailableTargets();

        foreach (var target in senses.GetSightings())
        {
            if (target == null) continue;
            if (IsFriend(target)) continue;  // TODO: we will eventually want to heal friends

            float distance = Vector3.Distance(target.transform.position, transform.position);
            if ( distance <= LongestMeleeRange()) {
                available_melee_targets.Add(target);
            } else if (distance <= LongestRangedRange()) {
                available_ranged_targets.Add(target);
            }
        }
    }


    private void ClearAvailableTargets()
    {
        available_melee_targets.Clear();
        available_ranged_targets.Clear();
    }


    private bool IsFriend(GameObject _target)
    {
        if (_target == null) return true;  // null is everyone's friend, or at least not their enemy!

        Mhoddim target_mhoddim = _target.GetComponent<Mhoddim>();
        Ghaddim target_ghaddim = _target.GetComponent<Ghaddim>();
        Fey target_fey = _target.GetComponent<Fey>();

        return mhoddim == target_mhoddim && ghaddim == target_ghaddim && fey == target_fey;
    }


    private float LongestMeleeRange()
    {
        float longest_range = float.MinValue;

        foreach (var weapon in available_weapons)
        {
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


    IEnumerator ManageAttacks()
    {
        CategorizePotentialTargets();
        SelectTarget();
        StrikeTarget();

        yield return null;
    }


    private GameObject MeleeTarget()
    {
        // select a random melee target and remove it from the list
        // TODO: allow multiple attacks on one target
        GameObject _target = null;

        if (available_melee_targets.Count > 0) {
            _target = available_melee_targets[Random.Range(0, available_melee_targets.Count)];
            available_melee_targets.Remove(_target);
        }

        return _target;
    }


    private GameObject RangedTarget()
    {
        // select a random ranged target and remove it from the list
        // TODO: allow multiple attacks on one target

        GameObject _target = null;

        if (available_ranged_targets.Count > 0) {
            _target = available_ranged_targets[Random.Range(0, available_ranged_targets.Count)];
            available_ranged_targets.Remove(_target);
        }

        return _target;
    }


    private void SelectTarget()
    {
        // attack targets in melee range before those at distance

        int number_of_targets = available_melee_targets.Count + available_ranged_targets.Count;

        if (number_of_targets > 0)
        {
            for (int i = 0; i < number_of_attacks; i++)
            {
                if (available_melee_targets.Count > 0)
                {
                    current_melee_targets.Enqueue(MeleeTarget());
                }
                else if (available_ranged_targets.Count > 0)
                {
                    current_ranged_targets.Enqueue(RangedTarget());
                }
            }

        }
    }


    private void StrikeMeleeTarget()
    {
        if (current_melee_targets.Count <= 0) return;
        
        foreach (var weapon in available_weapons) {
            if (weapon.range == Weapon.Range.Melee) {
                // handle melee
            } else if (weapon.range == Weapon.Range.Melee) {
                Weapon _ranged = Instantiate(weapon, transform.position, transform.rotation, transform.parent.transform);
                _ranged.name = "Ranged Weapon";
                _ranged.Target(current_melee_targets.Dequeue());
            }
        }
    }


    private void StrikeRangedTarget()
    {
        if (current_ranged_targets.Count <= 0) return;

        foreach (var weapon in available_weapons) {
            if (weapon.range == Weapon.Range.Ranged) {
                Weapon _ranged = Instantiate(weapon, transform.position, transform.rotation, transform.parent.transform);
                _ranged.name = "Ranged Weapon";
                _ranged.Target(current_ranged_targets.Dequeue());
            }
        }
    }

    private void StrikeTarget()
    {
        // The number of strikes is governed by haste and action_threshold in Update.

        // If any targets are in melee range, strike at them ahead of ranged

        if (current_melee_targets.Count > 0) {
            StrikeMeleeTarget();
        } else if (current_ranged_targets.Count > 0) { 
            StrikeRangedTarget();
        }
    }
}
