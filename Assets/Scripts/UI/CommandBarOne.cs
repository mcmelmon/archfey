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
        Instantiate(ent_prefab, _position, player_transform.rotation, fey_transform);
    }


    public void SummonRaven()
    {

    }
}
