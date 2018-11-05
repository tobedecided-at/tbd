using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise {

  public enum NormalMode {Local, Global}

    public static float[,] GenerateNoisemap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalMode normalMode) {
    float[,] noiseMap = new float[mapWidth, mapHeight];

    System.Random prng = new System.Random(seed);
    Vector2[] octaveOffsets = new Vector2[octaves];

    float maxPossibleHeight = 0;
    float amplitude = 1;
    float frequency = 1;

    for (int i = 0; i < octaves; i++) {
      float offsetX = prng.Next(-100000, 100000) + offset.x;
      float offsetY = prng.Next(-100000, 100000) - offset.y;

      octaveOffsets[i] = new Vector2(offsetX, offsetY);
      maxPossibleHeight += amplitude;
      amplitude *= persistance;
    }

    scale = scale <= 0 ? 0.0001f : scale;

    float maxHeight = float.MinValue;
    float minHeight = float.MaxValue;

    float halfWidth = mapWidth / 2f;
    float halfHeight = mapHeight / 2f;

    for (int y = 0; y < mapHeight; y++) {
      for (int x = 0; x < mapWidth; x++) {

        amplitude = 1;
        frequency = 1;
        float noiseHeight = 0;

        for (int i = 0; i < octaves; i++) {
          float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
          float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;

          float perlin = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
          noiseHeight += perlin * amplitude;

          amplitude *= persistance;
          frequency *= lacunarity;
        }

        if (noiseHeight > maxHeight) {
          maxHeight = noiseHeight;
        } else if (noiseHeight < minHeight) {
          minHeight = noiseHeight;
        }

        switch (normalMode) {
          case NormalMode.Local:
            noiseMap[x,y] = Mathf.InverseLerp(minHeight, maxHeight, noiseHeight);
          break;
          case NormalMode.Global:
            float normalized = (noiseHeight + 1) / (maxPossibleHeight / .9f);
            noiseMap[x, y] = Mathf.Clamp(normalized, 0, int.MaxValue);
          break;
        }
      }
    }
    
    return noiseMap;
  }

}
