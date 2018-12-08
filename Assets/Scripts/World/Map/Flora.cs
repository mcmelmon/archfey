using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flora : MonoBehaviour {

    public Tree tree_prefab;
    public float tree_coverage;
    public int canopy_layers;
    public GameObject canopy_prefab;
    public List<Canopy> leaves = new List<Canopy>();

    Map map;
    Geography geography;
    Biosphere biosphere;
    List<Tree> trees = new List<Tree>();
    GameObject[] canopy;
    GameObject carpet;
    int width, depth;

    [System.Serializable]
    public struct Canopy
    {
        public string name;
        public float height;
        public Color color;
    }

    // Unity


    void Awake () {
        biosphere = transform.GetComponentInParent<Biosphere>();
        map = transform.GetComponentInParent<Map>();
        geography = map.GetOrCreateGeography();
        canopy = new GameObject[canopy_layers];
        width = geography.GetResolution();
        depth = width;
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
        PlantTrees();
        CarpetForest();
        CoverForest();
    }



    // private


    private void CarpetForest()
    {
        int octaves = 4;
        float persistance = .5f;
        float lacunarity = 2;
        int seed = 5;
        float scale = 10f;
        Vector2 offset = new Vector2(0,0);

        Noise noise = new Noise();
        float[,] _carpet = noise.GenerateNoiseMap(width, depth, seed, scale, octaves, persistance, lacunarity, offset);
        carpet = CanopyLayer(_carpet);
        carpet.transform.localScale = new Vector3(75, 1, 75);
        carpet.transform.position = new Vector3(geography.GetResolution() / 2f, 0.1f, geography.GetResolution() / 2f);
    }



    private void CoverForest()
    {
        int octaves = 4;
        float persistance = .5f;
        float lacunarity= 2;

        for (int i = 0; i < canopy_layers; i++)
        {
            int seed = 63;
            float scale = 60f;
            Vector2 offset = new Vector2(Random.Range(0,20), Random.Range(0,20));

            Noise noise = new Noise();
            float[,] _canopy = noise.GenerateNoiseMap(width, depth, seed, scale, octaves, persistance, lacunarity, offset);
            canopy[i] = CanopyLayer(_canopy);
            canopy[i].transform.localScale = new Vector3(75, 1, 75);
            canopy[i].transform.position = new Vector3(geography.GetResolution() / 2f, 150f + (i * 50f), geography.GetResolution() / 2f);

        }
    }


    private GameObject CanopyLayer(float[,] _canopy)
    {
        Texture2D texture = new Texture2D(width, depth);

        Color[] colors = new Color[width * depth];
        for (int w = 0; w < width; w++) {
            for (int d = 0; d < depth; d++) {
                foreach (var layer in leaves) {
                    if (_canopy[w,d] <= layer.height) {
                        float alpha = 1f; // randomize
                        colors[w * width + d] = layer.color;
                        colors[w * width + d].a = alpha;
                        break;
                    }
                }
            }
        }

        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colors);
        texture.Apply();

        GameObject canopy_plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        canopy_plane.name = "Canopy";
        canopy_plane.transform.parent = transform;
        canopy_plane.GetComponent<Renderer>().sharedMaterial.mainTexture = texture;

        return canopy_plane;
    }


    private void PlantTrees()
    {
        int number_of_trees = Mathf.RoundToInt((geography.GetResolution()) * (tree_coverage / 100f));
        for (int i = 0; i < number_of_trees; i++) {
            Vector3 position = geography.RandomLocation();
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            Tree _tree = Instantiate(tree_prefab, position, rotation, transform);
            _tree.transform.localScale = new Vector3(1f, 1.25f, 1f) * Random.Range(0.2f, 2f);
            _tree.transform.position += new Vector3(0, _tree.transform.localScale.y - 2, 0);

            trees.Add(_tree);
        }
    }
}
