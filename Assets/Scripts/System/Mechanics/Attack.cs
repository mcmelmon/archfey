using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {

    public List<Weapon> available_weapons;
    public int number_of_attacks;

    List<GameObject> available_targets = new List<GameObject>();
    List<GameObject> available_melee_targets = new List<GameObject>();
    List<GameObject> available_ranged_targets = new List<GameObject>();
    private readonly List<GameObject> current_melee_targets = new List<GameObject>();
    private readonly List<GameObject> current_ranged_targets = new List<GameObject>();

    Mhoddim mhoddim;
    Ghaddim ghaddim;
    Fey fey;


    // Unity


    private void Awake()
    {
        mhoddim = GetComponent<Mhoddim>();
        ghaddim = GetComponent<Ghaddim>();
        fey = GetComponent<Fey>();
    }

    private void Start () {
        IdentifyAllTargets();
    }


    private void Update () {
        StartCoroutine("ManageAttacks");
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
        IdentifyAllTargets();

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
                    current_melee_targets.Add(MeleeTarget());
                }
                else if (available_ranged_targets.Count > 0)
                {
                    current_ranged_targets.Add(RangedTarget());
                }
            }

        }
    }


    public void StrikeMeleeTargets()
    {
        foreach (var target in current_melee_targets)
        {
            foreach (var weapon in available_weapons)
            {
                if (weapon.range == Weapon.Range.Melee)
                {
                    // handle melee
                } else {
                    Weapon _ranged = Instantiate(weapon, transform.position, transform.rotation, transform.parent.transform);
                    _ranged.name = "Ranged Weapon";
                    _ranged.Target(target);
                }

            }
        }
    }


    public void StrikeRangedTargets()
    {
        foreach (var target in current_ranged_targets)
        {
            foreach (var weapon in available_weapons)
            {
                if (weapon.range == Weapon.Range.Ranged)
                {
                    Weapon _ranged = Instantiate(weapon, transform.position, transform.rotation, transform.parent.transform);
                    _ranged.name = "Ranged Weapon";
                    _ranged.Target(target);
                }
            }
        }
    }

    private void StrikeTargets()
    {
        if (current_melee_targets.Count > 0 || current_ranged_targets.Count > 0)
        {
            StrikeMeleeTargets();
            StrikeRangedTargets();
        }
    }
}
