using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour {

    public enum Template { Commoner = 0, Gnoll = 1 };


    // properties

    public Formation Formation { get; set; }


    // public


    public void SetFormation(Formation _formation)
    {
        Formation = _formation;
    }
}
