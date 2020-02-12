using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biosphere : MonoBehaviour {

    // properties

    public static Biosphere Instance { get; set; }


    // Unity


    private void Awake()
    {
        if (Instance != null) {
            Debug.LogError("More than one biosphere instance!");
            Destroy(this);
            return;
        }
        Instance = this;
    }


    // public


    public void Eden()
    {
        // TODO: place fauna
        PlaceVegetation();
    }


    // private

    private void PlaceVegetation()
    {
        // Flora.Instance.Grow();
    }
}