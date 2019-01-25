using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flora : MonoBehaviour {

    // Inspector settings

    public Tree tree_prefab;
    public float tree_coverage;
    public int canopy_layers;
    public List<Canopy> leaves = new List<Canopy>();

    [System.Serializable]
    public struct Canopy
    {
        public string name;
        public float height;
        public Color color;
    }


    // properties

    public GameObject Carpet { get; set; }
    public int Depth { get; set; }
    public GameObject[] ForestLayers { get; set; }
    public static Flora Instance { get; set; }
    public List<Tree> Trees { get; set; }
    public int Width { get; set; }


    // Unity


    void Awake () {
        if (Instance != null) {
            Debug.LogError("More than one Flora");
            Destroy(this);
            return;
        }
        Instance = this;
    }


    // public


    public void Grow()
    {
        SetComponents();
        PlantTrees();
        //CarpetForest();
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
        float[,] _carpet = noise.GenerateNoiseMap(Width, Depth, seed, scale, octaves, persistance, lacunarity, offset);
        Carpet = Layer(_carpet);
        Carpet.transform.localScale = new Vector3(75, 1, 75);
        Carpet.transform.position = new Vector3(Geography.Instance.GetResolution() / 2f, 100.1f, Geography.Instance.GetResolution() / 2f);
        Carpet.name = "Carpet";
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
            float[,] _canopy = noise.GenerateNoiseMap(Width, Depth, seed, scale, octaves, persistance, lacunarity, offset);
            ForestLayers[i] = Layer(_canopy, true);
            ForestLayers[i].transform.localScale = new Vector3(75, -1, 75);
            ForestLayers[i].transform.position = new Vector3(Geography.Instance.GetResolution() / 2f, 170f + (i * 50f), Geography.Instance.GetResolution() / 2f);
            ForestLayers[i].name = "Canopy";
        }
    }


    private GameObject Layer(float[,] _canopy, bool enable_transparency = false)
    {
        Texture2D texture = new Texture2D(Width, Depth);

        Color[] colors = new Color[Width * Depth];
        for (int w = 0; w < Width; w++) {
            for (int d = 0; d < Depth; d++) {
                foreach (var layer in leaves) {
                    if (_canopy[w,d] <= layer.height) {
                        float alpha = 1f - _canopy[w,d];
                        colors[w * Width + d] = layer.color;
                        colors[w * Width + d].a = alpha;
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
        int number_of_trees = Mathf.RoundToInt((Geography.Instance.GetResolution()) * (tree_coverage / 100f));

    }


    private void SetComponents()
    {
        Depth = Geography.Instance.GetResolution();
        ForestLayers = new GameObject[canopy_layers];
        Trees = new List<Tree>();
        Width = Depth;
    }
}
