using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biosphere : MonoBehaviour {


    // Unity


    private void Start () {
        PlaceVegetation();
	}


    // private

    private void PlaceVegetation()
    {
        transform.GetComponentInChildren<Flora>().Grow();
    }
}