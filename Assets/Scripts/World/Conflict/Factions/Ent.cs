using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ent : MonoBehaviour {

    public Ent SummonEnt(Vector3 _position, Transform _parent)
    {
        Ent _ent = Instantiate(this, _position, transform.rotation, _parent);
        return _ent;
    }

    //float shortest_distance = Mathf.Infinity;
    //GameObject nearest_target = null;

    //foreach (var _target in targets)
    //{
    //    float to_enemy = Vector3.Distance(transform.position, _target.transform.position);
    //    if (to_enemy < shortest_distance)
    //    {
    //        shortest_distance = to_enemy;
    //        nearest_target = _target;
    //    }
    //}

    //if (nearest_target != null && shortest_distance <= ranged_attack_range)
    //{
    //    target = nearest_target;
    //}


}
