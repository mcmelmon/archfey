using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandBarOne : MonoBehaviour {

    public Transform player_transform;  // TODO: name other transforms using _transform convention
    public Transform fey_transform;
    public Ent ent_prefab;

    public void EntangleUnits()
    {

    }


    public void HealUnits()
    {

    }


    public void SummonEnt()
    {
        Vector3 starting_position = new Vector3(player_transform.position.x, 0f, player_transform.position.z) + new Vector3(0, ent_prefab.transform.position.y, 0);
        Vector3 summon_position = starting_position + player_transform.TransformDirection(Vector3.forward) * 20f;
        Ent _ent = ent_prefab.SummonEnt(summon_position, fey_transform);
    }


    public void SummonRaven()
    {

    }
}
