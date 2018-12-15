using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Civilization : MonoBehaviour {

    public static Civilization civilization_instance;

    // Unity


    private void Awake()
    {
        if (civilization_instance != null)
        {
            Debug.LogError("More than one civilization instance!");
            Destroy(this);
            return;
        }

        civilization_instance = this;
    }


    // public

    public void DawnOfMhoddim()
    {
        LayRuins();
    }


    // private


    private void LayRuins()
    {
        GetComponentInChildren<Ruins>().ErectRuins();
    }
}