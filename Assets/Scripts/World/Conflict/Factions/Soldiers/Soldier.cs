using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour {

    public enum Clasification { Heavy = 0, Striker = 1, Scout = 2, Ent = 3 };


    // properties

    public Formation Formation { get; set; }


    // public


    public void SetFormation(Formation _formation)
    {
        Formation = _formation;
    }
}
