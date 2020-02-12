using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    // Inspector settings

    public bool generate_map = false;


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


    // public


    // private

    private void CreateTheWorld()
    {
        // TODO: We are creating the world in scenes, not programatically
        
        // if (generate_map) GetComponentInChildren<Map>().DrawMap();
        // GetComponentInChildren<Conflict>().Hajime();
    }
}