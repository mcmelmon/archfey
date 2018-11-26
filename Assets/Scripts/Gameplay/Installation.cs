using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Installation : MonoBehaviour {
    private int current_haste;
    public int starting_haste = 100;

    public GameObject ranged_attack_weapon;
    public Transform ranged_attack_origin;
    public float ranged_attack_range = 5f;
    public GameObject target;

    // Unity


    void Update()
    {
        FindTarget();
        Attack();
        CheckHealth();
    }


    // public


    // private

    private void Attack()
    {
        if (target != null)
        {
            GameObject projectile = Instantiate(ranged_attack_weapon, ranged_attack_origin.position, ranged_attack_weapon.transform.rotation);
            Projectile _projectile = projectile.GetComponent<Projectile>();
            _projectile.transform.LookAt(target.transform);
            _projectile.transform.rotation *= Quaternion.Euler(90, 0, 0);

            if (current_haste > 0)
            {
                current_haste -= 1;
                return;
            }

            if (_projectile != null && target != null)
            {
                _projectile.Seek(target.transform);
            }

            current_haste = starting_haste;
        }
    }

    private void CheckHealth()
    {
        float health = GetComponent<Health>().current_health;
        if (health <= 0) {
            Perish();
        }
    }


    private void FindTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Enemy");
        float shortest_distance = Mathf.Infinity;
        GameObject nearest_target = null;

        foreach (var _target in targets)
        {
            float to_enemy = Vector3.Distance(transform.position, _target.transform.position);
            if (to_enemy < shortest_distance)
            {
                shortest_distance = to_enemy;
                nearest_target = _target;
            }
        }

        if (nearest_target != null && shortest_distance <= ranged_attack_range)
        {
            target = nearest_target;
        }
    }


    private void Perish()
    {
        transform.parent.gameObject.GetComponent<Installations>().listing.Remove(this);
        Destroy(gameObject);
    }
}
