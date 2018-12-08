using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise {

    public static float[,] GenerateNoiseMap(int width, int depth, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset){
        float[,] noise = new float[width, depth];

        System.Random random = new System.Random(seed);
        Vector2[] offsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offset_x = random.Next(-100000, 100000) + offset.x;
            float offset_y = random.Next(-100000, 100000) + offset.y;
            offsets[i] = new Vector2(offset_x, offset_y);
        }

        if (scale <= 0) scale = 0.001f;

        float max_height = float.MinValue;
        float min_height = float.MaxValue;
        float half_width = width / 2f;
        float half_depth = depth / 2f;

        for (int w = 0; w < width; w++) {
            for (int d = 0; d < depth; d++) {
                float amplitude = 1;
                float frequency = 1;
                float height = 0;

                for (int i = 0; i < octaves; i++) {
                    float _w = (w - half_width) / scale * frequency + offsets[i].x;
                    float _d = (d - half_depth) / scale * frequency + offsets[i].y;

                    float value = Mathf.PerlinNoise(_w, _d) * 2 - 1;  // allow noise value to go below zero
                    height += value * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (height > max_height) {
                    max_height = height;
                } else if (height < min_height) {
                    min_height = height;
                }
                noise[w, d] = height;
            }
        }

        for (int w = 0; w < width; w++) {
            for (int d = 0; d < depth; d++) {
                noise[w, d] = Mathf.InverseLerp(min_height, max_height, noise[w, d]);
            }
        }
        return noise;
    }
}