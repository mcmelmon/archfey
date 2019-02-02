using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Civilization : MonoBehaviour 
{

    // Inspector settings
    public GameObject actor_prefab;

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
}