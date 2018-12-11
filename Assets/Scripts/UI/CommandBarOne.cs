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
        Vector3 _position = player_transform.position + player_transform.TransformDirection(Vector3.forward) * 20f;
        _position.y = ent_prefab.transform.position.y;
        Ent _ent = ent_prefab.SummonEnt(_position, fey_transform);
    }


    public void SummonRaven()
    {

    }
}
