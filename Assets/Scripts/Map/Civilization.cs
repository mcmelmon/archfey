using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Civilization : MonoBehaviour {

    Installations installations;
    Map map;


    // Unity


    void Awake () {
        map = transform.GetComponentInParent<Map>();
        installations = transform.GetComponentInChildren<Installations>();
    }


    // public

}