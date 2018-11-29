using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Civilization : MonoBehaviour {

    Ruins ruins;
    Map map;


    // Unity


    void Awake () {
        map = GetComponentInParent<Map>();
        ruins = GetComponentInChildren<Ruins>();
    }


    // public

}