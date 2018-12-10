using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruin : MonoBehaviour {

    public static float minimum_ruin_proximity = 15f;

    readonly List<GameObject> allies = new List<GameObject>();


    // Unity


    void Update()
    {

    }


    // public


    // private

    private void Reinforce()
    {
        GameObject ally = FindAlly();
        if (ally != null)
        {
            // heal or buff ally
        }
    }

    private void CheckControl()
    { 
        // TODO: implement control mechanism
    }


    private GameObject FindAlly()
    {
        GameObject ally = new GameObject();
        return ally;
    }


    private void TransferControl()
    {

    }
}