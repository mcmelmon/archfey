using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    public static World world_instance;


    // Unity


    private void Awake()
    {
        if (world_instance != null)
        {
            Debug.LogError("More than one world instance!");
            Destroy(this);
            return;
        }

        world_instance = this;
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