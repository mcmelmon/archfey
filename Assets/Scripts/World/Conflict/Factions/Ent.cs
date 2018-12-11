using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ent : MonoBehaviour {

    public Ent SummonEnt(Vector3 _position, Transform _parent)
    {
        Ent _ent = Instantiate(this, _position, transform.rotation, _parent);
        return _ent;
    }
}
