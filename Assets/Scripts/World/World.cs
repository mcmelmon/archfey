using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    // properties

    public static World Instance { get; set; }

    // Unity


    private void Awake()
    {
        if (Instance != null) {
            Debug.LogError("More than one world instance!");
            Destroy(this);
            return;
        }
        Instance = this;
    }


    private void Start()
    {
        CreateTheWorld();
    }


    // public


    // private


    private void CreateTheWorld()
    {
        GetComponentInChildren<Map>().DrawMap();
        GetComponentInChildren<Conflict>().Hajime();
    }
}