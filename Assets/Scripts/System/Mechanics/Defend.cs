using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defend : MonoBehaviour {

    public bool under_attack;

    Health health;


    void Awake () {
        under_attack = false;
        health = GetComponent<Health>();
	}


	void Update () {
		
	}
}
