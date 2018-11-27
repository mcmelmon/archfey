using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biosphere : MonoBehaviour {

    public List<Tree> trees = new List<Tree>();

    Map map;

    // Unity

    private void Awake()
    {
        map = transform.GetComponentInParent<Map>();
    }


    private void Start () {
        PlaceVegetation();
	}


	private void Update () {

	}


    // public

    public Map GetMap()
    {
        return map;
    }

    // private

    private void PlaceVegetation()
    {
        transform.GetComponentInChildren<Flora>().PlaceTrees();
    }
}
