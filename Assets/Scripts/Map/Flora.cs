using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flora : MonoBehaviour {

    public Tree tree_prefab;
    public float tree_coverage;

    Map map;
    Biosphere biosphere;


    // Unity


	void Awake () {
        biosphere = transform.GetComponentInParent<Biosphere>();
        map = transform.GetComponentInParent<Map>();
	}
	
	void Update () {
		
	}


    // public


    public void PlaceTrees()
    {
        int number_of_trees = Mathf.RoundToInt((map.GetGeography().GetResolution()) * (tree_coverage / 100f));
        for (int i = 0; i < number_of_trees; i++)
        {
            Vector3 position = map.GetGeography().RandomLocation();
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            Tree _tree = Instantiate(tree_prefab, position, rotation, biosphere.transform);
            _tree.transform.localScale = new Vector3(1f, 1.25f, 1f) * Random.Range(0.1f, 1f);
            _tree.transform.position += new Vector3(0, _tree.transform.localScale.y - 2, 0);

            biosphere.trees.Add(_tree);
        }
    }
}
