using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    // public

    public Map GetMap()
    {
        return transform.GetComponentInChildren<Map>();
    }

}