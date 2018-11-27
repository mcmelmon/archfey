using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flora : MonoBehaviour {

    public Tree tree_prefab;
    public int tree_coverage_percent;

    Map map;
    Biosphere biosphere;


    // Unity


	void Awake () {
        biosphere = transform.parent.GetComponent<Biosphere>();
        map = biosphere.GetMap();
	}
	
	void Update () {
		
	}


    // public


    public void PlaceTrees()
    {
        int number_of_trees = Mathf.RoundToInt(map.GetTerrain().GetAllTiles().Count * (tree_coverage_percent / 100f));
        for (int i = 0; i < number_of_trees; i++)
        {
            Tile _tile = map.GetTerrain().PickRandomTile();
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            Vector3 offset = new Vector3(Random.Range(-1, 2), _tile.obstacles.Count, Random.Range(-1, 2));
            Tree _tree = Instantiate(tree_prefab, _tile.transform.position + offset, rotation);
            _tree.transform.localScale = new Vector3(.75f, 1f, .75f) * Random.Range(0.1f, 0.6f);
            _tile.trees.Add(_tree);
            biosphere.trees.Add(_tree);
        }
    }
}
