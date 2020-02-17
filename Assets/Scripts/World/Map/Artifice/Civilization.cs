using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Civilization : MonoBehaviour {

    // properties

    public static Civilization Instance { get; set; }
    public List<Structure> Structures { get; set; } 



    // Unity


    private void Awake()
    {
        if (Instance != null) {
            Debug.LogError("More than one civilization instance!");
            Destroy(this);
            return;
        }
        Instance = this;
        SetComponents();
    }

    private void Start() {
    }


    // public



    // private

    void SetComponents()
    {
        Structures = new List<Structure>();
    }

}
