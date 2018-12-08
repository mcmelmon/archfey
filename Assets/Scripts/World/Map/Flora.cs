using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flora : MonoBehaviour {

    public Tree tree_prefab;
    public float tree_coverage;
    public int canopy_layers;
    public GameObject canopy_prefab;

    Map map;
    Geography geography;
    Biosphere biosphere;
    List<Tree> trees = new List<Tree>();
    GameObject[] canopy;
    int width, depth;


    // Unity


	void Awake () {
        biosphere = transform.GetComponentInParent<Biosphere>();
        map = transform.GetComponentInParent<Map>();
        geography = map.GetOrCreateGeography();
        canopy = new GameObject[canopy_layers];
        width = geography.GetResolution();
        depth = width;
    }
	
	void Update () {
		
	}


    // public


    public List<Tree> GetTrees()
    {
        return trees;
    }


    public GameObject[] GetCanopy()
    {
        return canopy;
    }


    public void Grow()
    {
        Canopy();
        Trees();
    }



    // private


    private void Canopy()
    {
        int octaves = 4;
        float persistance = .5f;
        float lacunarity= 2;

        for (int i = 0; i < canopy_layers; i++)
        {
            int seed = Random.Range(0,100);
            float scale = 40f + seed;
            Vector2 offset = new Vector2(Random.Range(0,20), Random.Range(0,20));

            float[,] _canopy = Noise.GenerateNoiseMap(width, depth, seed, scale, octaves, persistance, lacunarity, offset);
            canopy[i] = CanopyLayer(_canopy, i);
        }
    }


    private GameObject CanopyLayer(float[,] _canopy, int _layer)
    {
        Texture2D texture = new Texture2D(width, depth);

        Color[] colors = new Color[width * depth];
        for (int w = 0; w < width; w++)
        {
            for (int d = 0; d < depth; d++)
            {
                float alpha = _canopy[w, d];
                Color hue = new Color(0.3f, 0.4f, 0.6f)
                {
                    a = alpha
                };
                colors[w * width + d] = hue;
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        GameObject canopy_plane = Instantiate(canopy_prefab, transform);
        canopy_plane.GetComponent<Renderer>().sharedMaterial.mainTexture = texture;
        canopy_plane.transform.position = new Vector3(geography.GetResolution()/2f, 50 + (_layer * 50), geography.GetResolution()/2f);

        return canopy_plane;
    }


    private void Trees()
    {
        int number_of_trees = Mathf.RoundToInt((geography.GetResolution()) * (tree_coverage / 100f));
        for (int i = 0; i < number_of_trees; i++)
        {
            Vector3 position = geography.RandomLocation();
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            Tree _tree = Instantiate(tree_prefab, position, rotation, transform);
            _tree.transform.localScale = new Vector3(1f, 1.25f, 1f) * Random.Range(0.2f, 2f);
            _tree.transform.position += new Vector3(0, _tree.transform.localScale.y - 2, 0);

            trees.Add(_tree);
        }
    }
}
