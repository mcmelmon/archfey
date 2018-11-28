using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Civilization : MonoBehaviour {

    Installations installations;
    Map map;


    // Unity


    void Awake () {
        map = GetComponentInParent<Map>();
        installations = GetComponentInChildren<Installations>();
    }


    // public

}