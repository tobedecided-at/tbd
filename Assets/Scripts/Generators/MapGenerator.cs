using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;

public class MapGenerator : MonoBehaviour {

  public enum DrawMode {NoiseMap, ColorMap, Mesh, FallofMap}

  public DrawMode drawMode;

  public Noise.NormalMode normal;

  public int octaves;
  public int seed;
  [Range(0,6)]
  public int previewLod;

  public const int chunkSize = 241;
  
  public Vector2 offset;

  [Range(0,1)]
  public float persistance;
  public float lacunarity;
  public float scale;
  public float heightScale;

  public bool useFalloffMap;
  
  public AnimationCurve heightCurve;

  public bool autoUpdate;

  public TerrainType[] regions;

  float[,] falloffMap;

  Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
  Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

  void Awake() {
    falloffMap = FalloffGenerator.GenerateFalloffMap(chunkSize);
  }

  public void DrawInEditor() {
    MapDisplay display = FindObjectOfType<MapDisplay>();
    MapData mapData = GenerateMapData(Vector2.zero);

    switch (drawMode) {
      case DrawMode.NoiseMap:
        display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
      break;
      case DrawMode.ColorMap:
        display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, chunkSize, chunkSize));
      break;
      case DrawMode.Mesh:
        display.DrawMesh(MeshGenerator.GenerateTerrainmesh(mapData.heightMap, heightScale, heightCurve, previewLod), TextureGenerator.TextureFromColorMap(mapData.colorMap, chunkSize, chunkSize));
      break;
      case DrawMode.FallofMap:
        display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(chunkSize)));
      break;
    }
  }

  public void RequestMapData(Vector2 center, Action<MapData> callback) {
    ThreadStart thread = delegate {
      MapDataThread(center, callback);
    };

    new Thread(thread).Start();
  }

  void MapDataThread(Vector2 center, Action<MapData> callback) {
    MapData mapData = GenerateMapData(center);
    lock(mapDataThreadInfoQueue) {
      mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
    }
  }

  public void RequestMeshData(MapData mapData, int lod, Action<MeshData> cb) {
    ThreadStart thread = delegate {
      MeshDataThread(mapData, lod, cb);
    };

    new Thread(thread).Start();
  }

  void MeshDataThread(MapData mapData, int lod, Action<MeshData> cb) {
    MeshData meshData = MeshGenerator.GenerateTerrainmesh(mapData.heightMap, heightScale, heightCurve, lod);
    lock(meshDataThreadInfoQueue) {
      meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(cb, meshData));
    }
  }

  void Update() {
    while (mapDataThreadInfoQueue.Count > 0) {
      for (int i = 0; i < mapDataThreadInfoQueue.Count; i++) {
        MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
        threadInfo.callback(threadInfo.parameter);
      }
    }
    while (meshDataThreadInfoQueue.Count > 0) {
      for (int i = 0; i < meshDataThreadInfoQueue.Count; i++) {
        MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
        threadInfo.callback(threadInfo.parameter);
      }
    }
  }

  MapData GenerateMapData(Vector2 center) {
    float[,] noiseMap = Noise.GenerateNoisemap(chunkSize, chunkSize, seed, scale, octaves, persistance, lacunarity, center + offset, normal);
    Color[] colorMap = new Color[chunkSize * chunkSize];

    for (int y = 0; y < chunkSize; y++) {
      for (int x = 0; x < chunkSize; x++) {
        if (useFalloffMap) {
          noiseMap[x,y] = Mathf.Clamp01(noiseMap[x,y] - falloffMap[x,y]);
        }

        float height = noiseMap[x, y];

        foreach (var region in regions) {
          if (height >= region.height) {
            colorMap[y * chunkSize + x] = region.color;
          } else {
            break;
          }
        }
      }
    }
    return new MapData(noiseMap, colorMap);
  }

	void OnValidate() {
    lacunarity = lacunarity < 1 ? 1 : lacunarity;
    octaves = octaves < 1 ? 1 : octaves;
    falloffMap = FalloffGenerator.GenerateFalloffMap(chunkSize);
  }

  struct MapThreadInfo<T> {
    public readonly Action<T> callback;
    public readonly T parameter;

    public MapThreadInfo(Action<T> callback, T parameter) {
      this.callback = callback;
      this.parameter = parameter;
    }
  }
}

[System.Serializable]
public struct TerrainType {
  public float height;
  public Color color;
  public string label;
}

public struct MapData {
  public readonly float[,] heightMap;
  public readonly Color[] colorMap;

  public MapData(float[,] heightMap, Color[] colorMap) {
    this.heightMap = heightMap;
    this.colorMap = colorMap;
  }

}