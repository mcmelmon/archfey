using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biosphere : MonoBehaviour {

    public static Biosphere biosphere_instance;
    // Unity


    private void Awake()
    {
        if (biosphere_instance != null)
        {
            Debug.LogError("More than one biosphere instance!");
            Destroy(this);
            return;
        }

        biosphere_instance = this;
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
        transform.GetComponentInChildren<Flora>().Grow();
    }
}