using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMap : MonoBehaviour
{
    public Renderer plane;
    public int octaves;
    public float persistance;
    public float lacunarity;
    public float scale = 27f;
    public int seed;
    public Vector2 offset;

    int width = 512;
    int depth = 512;


    private void Start()
    {
        CreateNoiseMap();
    }


    public void CreateNoiseMap()
    {
        Noise noise = new Noise();
        float[,] map = noise.GenerateNoiseMap(width, depth, seed, scale, octaves, persistance, lacunarity, offset);
        DrawNoiseMap(map);
    }


    public void DrawNoiseMap(float[,] _map)
    {
        Texture2D texture = new Texture2D(width, depth);

        Color[] colors = new Color[width * depth];
        for (int w = 0; w < width; w++)
        {
            for (int d = 0; d < depth; d++)
            {
                colors[w * width + d] = Color.Lerp(Color.black, Color.white, _map[w, d]);
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        plane.sharedMaterial.mainTexture = texture;
        plane.transform.localScale = new Vector3(width, 1, depth);
    }

}