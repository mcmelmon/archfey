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

    bool holding = false;
    NavMeshAgent agent;
    Tile tile;
    Terrain terrain;
    Map map;

    // Unity


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        tile = transform.parent.gameObject.GetComponent<Tile>();
        terrain = tile.transform.parent.GetComponent<Terrain>();
        map = terrain.transform.parent.GetComponent<Map>();
    }


    private void Update()
    {
        if (destination == null) FindTarget();
        if (!holding) Move();
        EvaluateAttacks();
    }


    // public


    public Transform FindTarget() 
    {
        List<Installation> _installations = map.GetInstallations().listing;
        float shortest_distance = Mathf.Infinity;
        Transform nearest_target = null;

        foreach (var _target in _installations)
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


    public bool PathAvailable()
    {
        // TODO: calculate if the destination is reachable.
        return true;
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
