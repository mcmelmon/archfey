using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Actor : MonoBehaviour {

    private float current_haste;
    public float starting_haste = 100f;
    public float speed = 10f;
    public Transform destination;
    public float ranged_attack_range;
    public float melee_attack_range;
    public float haste = 200f;
    bool holding;

    NavMeshAgent agent;
    Map map;

    // Unity


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        holding = false;
    }

    private void Start()
    {

    }


    private void Update()
    {

    }


    // public


    public Transform FindTarget() 
    {
        return null;
        List<Ruin> _ruins = map.GetComponentInChildren<Civilization>().GetComponentInChildren<Ruins>().ruins;
        float shortest_distance = Mathf.Infinity;
        Transform nearest_target = null;

        foreach (var _target in _ruins)
        {
            float to_enemy = Vector3.Distance(transform.position, _target.transform.position);
            if (to_enemy < shortest_distance)
            {
                shortest_distance = to_enemy;
                nearest_target = _target.transform;
            }
        }

        if (nearest_target != null)
        {
            destination = nearest_target;
        }
        else
        {
            destination = null;
        }

        return destination;
    }


    // private


    private void AttackInMelee()
    {
        holding = true;

        if (current_haste > 0) {
            current_haste--;
            return;
        }

        Health health = destination.gameObject.GetComponent<Health>();
        health.LoseHealth(1);

        current_haste = starting_haste;
    }


    private void AttackAtRange()
    {
        holding = true;
        //GameObject projectile = Instantiate(ranged_attack_weapon, ranged_attack_origin.position, ranged_attack_weapon.transform.rotation);
        //Projectile _projectile = projectile.GetComponent<Projectile>();
        //_projectile.transform.LookAt(target.transform);
        //_projectile.transform.rotation *= Quaternion.Euler(90, 0, 0);

        //if (current_haste > 0)
        //{
        //    current_haste -= 1;
        //    return;
        //}

        //if (_projectile != null && target != null)
        //{
        //    _projectile.Seek(target.transform);
        //}

        //current_haste = starting_haste;
    }


    private void EvaluateAttacks()
    {
        if (destination == null) return;

        float to_enemy = Vector3.Distance(transform.position, destination.position);

        if (to_enemy <= ranged_attack_range)
        {
            AttackAtRange();
        }
        else if (to_enemy <= melee_attack_range)
        {
            AttackInMelee();
        }
    }


    private void Move()
    {
        if (destination != null) {
            agent.SetDestination(destination.position);
        }
        else {
            holding = true;
        }
    }
}
