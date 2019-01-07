using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Civilization : MonoBehaviour {

    // properties

    public static Civilization Instance { get; set; }


    // Unity


    private void Awake()
    {
        if (Instance != null) {
            Debug.LogError("More than one civilization instance!");
            Destroy(this);
            return;
        }
        Instance = this;
    }


    // public

    public void DawnOfMhoddim()
    {
        LayRuins();
    }


    // private


    private void LayRuins()
    {
        Objectives.Instance.PlaceObjectives();
    }
}