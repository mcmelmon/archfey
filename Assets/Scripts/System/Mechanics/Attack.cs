using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {

    public List<Weapon> available_weapons;
    public bool attacking;

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
    Victims victims = new Victims();

    public struct Victims {
        public bool mhoddim;
        public bool ghaddim;
    }

    // Unity


    private void Awake()
    {
        turn = GetComponent<Turn>();
        mhoddim = GetComponent<Mhoddim>();
        ghaddim = GetComponent<Ghaddim>();
        fey = GetComponent<Fey>();
        senses = GetComponent<Senses>();
        victims.mhoddim = false;
        victims.ghaddim = false;
        attacking = false;
    }


    // public


    public void DiscoverEnemies()
    {
        // This method will be called by Turn after GetSightings, so the sightings list 
        // will be populated (if anything is in range).  We set sightings in Turn because
        // we also want to know about friends.

        enemies.Clear();

        foreach (var sighting in senses.sightings)
        {
            if (sighting == gameObject) continue; // we can "sight" ourselves (potential heal target), but not an enemy
            if (IsFriendOrNeutral(sighting)) continue;
            if (!enemies.Contains(sighting)) enemies.Add(sighting);
        }

        if (enemies.Count > 0) {
            attacking = true;
        } else {
            attacking = false;
        }
    }


    public List<GameObject> GetCurrentEnemies()
    {
        return enemies;
    }


    public IEnumerator ManageAttacks()
    {
        DiscoverEnemies();

        if (enemies.Count > 0) {
            CategorizePotentialTargets();
            SelectTarget();
            StrikeTarget();
        }

        yield return null;
    }


    // private

    private void CategorizePotentialTargets()
    {
        ClearTargets();

        foreach (var target in enemies) {
            if (target == null) continue;
            float distance = Vector3.Distance(target.transform.position, transform.position);

            if ( distance <= LongestMeleeRange()) {
                available_melee_targets.Add(target);
            } else if (distance <= LongestRangedRange()) {
                // targets in melee range are also potential ranged targets
                available_ranged_targets.Add(target);
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
        if (_target == null) return true;  // null is everyone's friend, or at least not their enemy!
        if (fey != null) {
            return false; // TODO: the only fey right now is an Ent, and Ents attack
        }

        Mhoddim target_mhoddim = _target.GetComponent<Mhoddim>();
        Ghaddim target_ghaddim = _target.GetComponent<Ghaddim>();
        Fey target_fey = _target.GetComponent<Fey>();
        bool fey_foe = false;

        if (target_fey != null)
        {
            // The fey are neutral, but if a specific unit has attacked "us",
            // then it is an enemy

            bool ghaddim_victim = _target.GetComponent<Attack>().victims.ghaddim;
            bool mhoddim_victim = _target.GetComponent<Attack>().victims.mhoddim;

            fey_foe =  (mhoddim != null && mhoddim_victim) || (ghaddim != null && ghaddim_victim);
        }

        bool friend_or_neutral = (mhoddim == null && target_mhoddim == null) || (ghaddim == null && target_ghaddim == null) && !fey_foe;

        return friend_or_neutral;
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
        
        if (available_melee_targets.Count > 0) {
            current_melee_targets.Add(MeleeTarget());
        } else if (available_ranged_targets.Count > 0) {
            current_ranged_targets.Add(RangedTarget());
        }
    }


    private void SetVictims(GameObject _target)
    {
        victims.mhoddim = _target.GetComponent<Mhoddim>() != null;
        victims.ghaddim = _target.GetComponent<Ghaddim>() != null;
    }


    private void StrikeMeleeTarget()
    {
        if (current_melee_targets.Count <= 0) return;
        
        foreach (var weapon in available_weapons) {
            if (weapon.range == Weapon.Range.Melee) {
                Weapon _melee = Instantiate(weapon, transform.position, transform.rotation, transform.parent.transform);
                _melee.name = "Melee Weapon";
                GameObject _target = current_melee_targets[Random.Range(0, current_melee_targets.Count)];
                SetVictims(_target);
                _melee.Target(_target);
            } else if (weapon.range == Weapon.Range.Ranged) {
                Weapon _ranged = Instantiate(weapon, transform.position, transform.rotation, transform.parent.transform);
                _ranged.name = "Ranged Weapon";
                GameObject _target = current_melee_targets[Random.Range(0, current_melee_targets.Count)];
                SetVictims(_target);
                _ranged.Target(_target);
            }
        }
    }


    private void StrikeRangedTarget()
    {
        if (current_ranged_targets.Count <= 0) return;

        foreach (var weapon in available_weapons) {

            // TODO: potentially disadvantage ranged attacks against melee targets

            if (weapon.range == Weapon.Range.Ranged) {
                Weapon _ranged = Instantiate(weapon, transform.position, transform.rotation, transform.parent.transform);
                _ranged.name = "Ranged Weapon";
                GameObject _target = current_ranged_targets[Random.Range(0,current_ranged_targets.Count)];
                _ranged.Target(_target);
                SetVictims(_target);
                _ranged.Target(_target);
            }
        }
    }

    private void StrikeTarget()
    {
        // The number of strikes is governed by haste and action_threshold in Update.

        // If any targets are in melee range, strike at them ahead of ranged

        if (current_melee_targets.Count <= 0 && current_ranged_targets.Count <= 0) return;

        if (current_melee_targets.Count > 0) {
            StrikeMeleeTarget();
        } else if (current_ranged_targets.Count > 0) { 
            StrikeRangedTarget();
        }
    }
}
