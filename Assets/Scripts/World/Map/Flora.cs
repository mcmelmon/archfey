using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flora : MonoBehaviour {

    public static Flora flora_instance;

    public Tree tree_prefab;
    public float tree_coverage;
    public int canopy_layers;
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
        if (flora_instance != null) {
            Debug.LogError("More than one Flora");
            Destroy(this);
            return;
        }

        flora_instance = this;
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
        SetComponents();
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
        int seed = Random.Range(1,10);
        float scale = Random.Range(10,20);
        Vector2 offset = new Vector2(0,0);

        Noise noise = new Noise();
        float[,] _carpet = noise.GenerateNoiseMap(width, depth, seed, scale, octaves, persistance, lacunarity, offset);
        carpet = Layer(_carpet);
        carpet.transform.localScale = new Vector3(75, 1, 75);
        carpet.transform.position = new Vector3(geography.GetResolution() / 2f, 0.1f, geography.GetResolution() / 2f);
        carpet.name = "Carpet";
    }



    private void CoverForest()
    {
        int octaves = 4;
        float persistance = .5f;
        float lacunarity= 2;

        for (int i = 0; i < canopy_layers; i++)
        {
            int seed = Random.Range(11,99);
            float scale = Random.Range(30,70);
            Vector2 offset = new Vector2(Random.Range(0,20), Random.Range(0,20));

            Noise noise = new Noise();
            float[,] _canopy = noise.GenerateNoiseMap(width, depth, seed, scale, octaves, persistance, lacunarity, offset);
            canopy[i] = Layer(_canopy, true);
            canopy[i].transform.localScale = new Vector3(75, -1, 75);
            canopy[i].transform.position = new Vector3(geography.GetResolution() / 2f, 50f + (i * 25f), geography.GetResolution() / 2f);
            canopy[i].name = "Canopy";


        }
    }


    private GameObject Layer(float[,] _canopy, bool enable_transparency = false)
    {
        Texture2D texture = new Texture2D(width, depth);

        Color[] colors = new Color[width * depth];
        for (int w = 0; w < width; w++) {
            for (int d = 0; d < depth; d++) {
                foreach (var layer in leaves) {
                    if (_canopy[w,d] <= layer.height) {
                        float alpha = 1f - _canopy[w,d];
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
        canopy_plane.transform.parent = transform;
        canopy_plane.GetComponent<Renderer>().material = new Material(Shader.Find("Nature/Tree Creator Leaves Fast")) { mainTexture = texture };
        canopy_plane.GetComponent<Renderer>().material.SetFloat("_Glossiness", 1f);

        if (enable_transparency) { 
            canopy_plane.GetComponent<Renderer>().material.SetFloat("_Cutoff", 0.45f); 
        } else {
            canopy_plane.GetComponent<Renderer>().material.SetFloat("_Cutoff", 0f);
        }


        return canopy_plane;
    }


    private void PlantTrees()
    {
        int number_of_trees = Mathf.RoundToInt((geography.GetResolution()) * (tree_coverage / 100f));
        for (int i = 0; i < number_of_trees; i++) {
            Vector3 position = geography.RandomLocation();
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            Tree _tree = Instantiate(tree_prefab, position, rotation, transform);
            float scale_boost = Random.Range(0.2f, 2f);
            _tree.transform.localScale = new Vector3(scale_boost, scale_boost, scale_boost);
            _tree.transform.position += new Vector3(0, -scale_boost, 0);

            trees.Add(_tree);
        }
    }


    private void SetComponents()
    {
        biosphere = transform.GetComponentInParent<Biosphere>();
        map = transform.GetComponentInParent<Map>();
        geography = map.GetComponentInChildren<Geography>();
        canopy = new GameObject[canopy_layers];
        width = geography.GetResolution();
        depth = width;
    }
}
