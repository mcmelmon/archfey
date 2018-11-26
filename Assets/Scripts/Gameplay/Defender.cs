using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender : MonoBehaviour {
    private int current_haste;

    [Header("Configuration")]
    public float long_range = 35f;
    public float turning = 100f;
    public int attack = 1;
    public int starting_haste = 100;
    public GameObject ranged_attack_weapon;
    public Transform ranged_attack_origin;

    void Update()
    {
        GameObject foe = UpdateTargets();

        if (foe != null)
            Attack(foe);
    }

    private void Attack(GameObject _foe)
    {
        GameObject leaf_instance = Instantiate(ranged_attack_weapon, ranged_attack_origin.position, transform.rotation);
        Leaf _leaf = leaf_instance.GetComponent<Leaf>();

        if (current_haste > 0)
        {
            current_haste -= 1;
            return;
        }

        Face(_foe);

        if (_leaf != null)
        {
            _leaf.Seek(_foe.transform);
        }

        current_haste = starting_haste;
    }

    private void Face(GameObject _foe) 
    {
        Vector3 facing = _foe.transform.position - transform.position;
        Quaternion face = Quaternion.LookRotation(facing);
        Vector3 rotation = face.eulerAngles;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, face, Time.deltaTime * turning);
    }

    private GameObject UpdateTargets()
    {
        GameObject[] foes = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestTarget = null;

        foreach (var foe in foes)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, foe.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestTarget = foe;
            }
        }

        if (nearestTarget != null && shortestDistance <= long_range)
        {
            return nearestTarget;
        }
        else
        {
            return null;
        }
    }
}