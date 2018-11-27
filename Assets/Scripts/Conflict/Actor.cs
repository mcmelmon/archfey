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
    Geography geography;


    // Unity


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        holding = false;
    }

    private void Start()
    {
        map = GetComponentInParent<Offense>().map;
        geography = map.GetGeography();
    }


    private void Update()
    {
        if (destination == null) FindTarget();
        if (!holding) Move();
        EvaluateAttacks();
        DespawnIfTrapped();
    }


    // public


    public bool PathToCenter()
    {
        NavMeshPath path = new NavMeshPath();
        bool complete = true;


        NavMesh.CalculatePath(transform.position, map.GetCenter(), NavMesh.AllAreas, path);
        complete = path.status == NavMeshPathStatus.PathComplete;
        return complete;
    }


    public Transform FindTarget() 
    {
        List<Installation> _installations = map.GetComponentInChildren<Civilization>().GetComponentInChildren<Installations>().listing;
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


    private void DespawnIfTrapped()
    {
        if (!PathToCenter()){
            transform.gameObject.SetActive(false);
            Destroy(transform.gameObject);
        } 
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
