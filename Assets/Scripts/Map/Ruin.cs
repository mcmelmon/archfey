using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruin : MonoBehaviour {


    // Unity


    void Update()
    {

    }


    // public


    // private

    private void Reinforce()
    {
        GameObject ally = FindAlly();
        if (ally != null)
        {
            // heal or buff ally
        }
    }

    private void CheckControl()
    { 
    
    }


    private GameObject FindAlly()
    {
        GameObject ally = new GameObject();
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

        return ally;
    }


    private void TransferControl()
    {

    }
}