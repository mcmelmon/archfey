using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {

    public List<Weapon> available_weapons;
    public int number_of_attacks;

    List<GameObject> available_targets = new List<GameObject>();
    List<GameObject> available_melee_targets = new List<GameObject>();
    List<GameObject> available_ranged_targets = new List<GameObject>();
    private readonly Queue<GameObject> current_melee_targets = new Queue<GameObject>();
    private readonly Queue<GameObject> current_ranged_targets = new Queue<GameObject>();

    Mhoddim mhoddim;
    Ghaddim ghaddim;
    Fey fey;

    int remaining_attacks;
    float inverse_haste = 5f;
    float current_haste;

    // Unity


    private void Awake()
    {
        mhoddim = GetComponent<Mhoddim>();
        ghaddim = GetComponent<Ghaddim>();
        fey = GetComponent<Fey>();
        remaining_attacks = number_of_attacks;
        current_haste = inverse_haste;
    }

    private void Start () {
        IdentifyAllTargets();
    }


    private void Update () {
        if (current_haste <= 0)
        {
            StartCoroutine(ManageAttacks());
            remaining_attacks = number_of_attacks;  // TODO: space the attacks out as we countdown haste
            current_haste = inverse_haste;
        } else {
            current_haste -= Time.deltaTime;
        }
    }


    // public


    public void IdentifyAllTargets()
    {
        available_targets.Clear();

        if (ghaddim != null)
        {
            foreach (var target in FindObjectsOfType<Mhoddim>())
            {
                available_targets.Add(target.gameObject);
            }
        }
        else if (mhoddim != null)
        {
            foreach (var target in FindObjectsOfType<Ghaddim>())
            {
                available_targets.Add(target.gameObject);
            }
        }
        else
        {
            foreach (var target in FindObjectsOfType<Ghaddim>())
            {
                available_targets.Add(target.gameObject);
            }

            foreach (var target in FindObjectsOfType<Mhoddim>())
            {
                available_targets.Add(target.gameObject);
            }
        }
    }


    // private

    private void CategorizeAvailableTargets()
    {
        foreach (var target in available_targets)
        {
            float distance = Vector3.Distance(target.transform.position, transform.position);
            if ( distance <= LongestMeleeAttackRange()) {
                available_melee_targets.Add(target);
            } else if (distance <= LongestRangedAttackRange()) {
                available_ranged_targets.Add(target);
            }
        }
    }


    private float LongestMeleeAttackRange()
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


    private float LongestRangedAttackRange()
    {
        float longest_range = float.MinValue;

        foreach (var weapon in available_weapons)
        {
            if (weapon.ranged_attack_range > longest_range)
            {
                longest_range = weapon.ranged_attack_range;
            }
        }

        return longest_range;
    }


    IEnumerator ManageAttacks()
    {
        IdentifyAllTargets();
        CategorizeAvailableTargets();

        if (available_weapons.Count > 0)
        {
            SelectTargetForEachAttack();
            StrikeTargets();
        }

        yield return new WaitForSeconds(5f); // TODO: incorporate the notion of "haste"
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

        if (available_ranged_targets.Count > 0)
        {
            _target = available_ranged_targets[Random.Range(0, available_ranged_targets.Count)];
            available_ranged_targets.Remove(_target);
        }

        return _target;
    }


    private void SelectTargetForEachAttack()
    {
        // attack all targets in melee range before those at distance

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


    public void StrikeMeleeTarget()
    {
        if (current_melee_targets.Count <= 0) return;

        Weapon[] deployed_weapons = GetComponentsInChildren<Weapon>();

        if (deployed_weapons.Length <= 0)
        {
            foreach (var weapon in available_weapons)
            {
                if (weapon.range == Weapon.Range.Melee && GetComponentsInChildren<Weapon>() == null)
                {
                    // handle melee
                }
                else if (weapon.range == Weapon.Range.Melee)
                {
                    Weapon _ranged = Instantiate(weapon, transform.position, transform.rotation, transform.parent.transform);
                    _ranged.name = "Ranged Weapon";
                    _ranged.Target(current_melee_targets.Dequeue());
                    remaining_attacks -= 1;
                }
            }
        }
    }


    public void StrikeRangedTarget()
    {
        if (current_ranged_targets.Count <= 0) return;

        Weapon[] deployed_weapons = GetComponentsInChildren<Weapon>();

        if (deployed_weapons.Length <= 0)
        {
            foreach (var weapon in available_weapons)
            {
                if (weapon.range == Weapon.Range.Ranged)
                {
                    Weapon _ranged = Instantiate(weapon, transform.position, transform.rotation, transform.parent.transform);
                    _ranged.name = "Ranged Weapon";
                    _ranged.Target(current_ranged_targets.Dequeue());
                    remaining_attacks -= 1;
                }
            }
        }
    }

    private void StrikeTargets()
    {
        if (current_melee_targets.Count > 0 || current_ranged_targets.Count > 0 && remaining_attacks > 0)
        {
            StrikeMeleeTarget();
            StrikeRangedTarget();
        }
    }
}
