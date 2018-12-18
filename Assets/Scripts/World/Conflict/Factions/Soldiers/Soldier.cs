using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour {

    public enum Clasification { Heavy = 0, Striker = 1, Scout = 2, Ent = 3 };
    public Formation formation;


    // public


    public void SetFormation(Formation _formation)
    {
        formation = _formation;
    }

}
