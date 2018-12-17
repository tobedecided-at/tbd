using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour {
  const float scale = .5f;
  const float playerMoveThresh = 25f;
  const float sqrplayerMoveThresh = playerMoveThresh * playerMoveThresh;
	public static float maxViewDist;

  public LODInfo[] detailLevels;
  public Transform player;

  public Vector2 playerPosOld;
  public static Vector2 playerPos;

  int chunkSize;
  int chunksVisibleinViewDist;

  public Material MapMaterial;

  static MapGenerator mapGenerator;

  Dictionary<Vector2, TerrainChunk> terrainChunkDict = new Dictionary<Vector2, TerrainChunk>();
  static List<TerrainChunk> terrainChunkVis = new List<TerrainChunk>();

  void Start() {
    chunkSize = MapGenerator.chunkSize - 1;
    maxViewDist = detailLevels[detailLevels.Length - 1].visibleDistThresh;
    chunksVisibleinViewDist = Mathf.RoundToInt(maxViewDist / chunkSize);
    mapGenerator = GameObject.FindObjectOfType<MapGenerator>();

    UpdateVisibleChunks();
  }

  void Update() {
    playerPos = new Vector2(player.position.x, player.position.z) / scale;
    
    if ((playerPosOld - playerPos).sqrMagnitude > sqrplayerMoveThresh) {
      playerPosOld = playerPos;
      UpdateVisibleChunks();
    }
  }

  void UpdateVisibleChunks() {

    foreach (var chunk in terrainChunkVis) {
      chunk.SetVisible(false);
    }

    terrainChunkVis.Clear();

    int currentCoordX = Mathf.RoundToInt(playerPos.x / chunkSize);
    int currentCoordY = Mathf.RoundToInt(playerPos.y / chunkSize);

    for (int yOffset = -chunksVisibleinViewDist; yOffset <= chunksVisibleinViewDist; yOffset++) {
      for (int xOffset = -chunksVisibleinViewDist; xOffset <= chunksVisibleinViewDist; xOffset++) {
        Vector2 viewedChunkCoord = new Vector2(currentCoordX + xOffset, currentCoordY + yOffset);

        if (terrainChunkDict.ContainsKey(viewedChunkCoord)) {
          terrainChunkDict[viewedChunkCoord].UpdateChunk();
        } else {
          terrainChunkDict.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, detailLevels, transform, MapMaterial));
        }
      }
    }
  }

  public class TerrainChunk {

    static long newestChunkIndex = 0;
    long id;
    GameObject meshObject;
    Vector2 pos;
    Bounds bounds;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

    LODInfo[] details;
    LODMesh[] lodMeshes;

    MapData mapData;
    bool mapDataGot;

    int prevLod = -1;

    public TerrainChunk(Vector2 coord, int size, LODInfo[] details, Transform parent, Material mat) {
      this.details = details;
      this.id = newestChunkIndex++;
      pos = coord * size;
      bounds = new Bounds(pos, Vector2.one * size);

      Vector3 pos3 = new Vector3(pos.x, 0, pos.y);

      meshObject = new GameObject("Chunk " + id);
      meshRenderer = meshObject.AddComponent<MeshRenderer>();
      meshFilter = meshObject.AddComponent<MeshFilter>();
      meshRenderer.material = mat;

      meshObject.transform.position = pos3 * scale;
      meshObject.transform.parent = parent;
      meshObject.transform.localScale = Vector3.one * scale;
      SetVisible(false);

      lodMeshes = new LODMesh[details.Length];
      for (int i = 0; i < details.Length; i++) {
        lodMeshes[i] = new LODMesh(details[i].lod, UpdateChunk);
      }

      mapGenerator.RequestMapData(pos, OnMapDataReceived);
    }

    void OnMapDataReceived(MapData mapData) {
      this.mapData = mapData;
      mapDataGot = true;

      Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.colorMap, MapGenerator.chunkSize, MapGenerator.chunkSize);
      meshRenderer.material.mainTexture = texture;

      UpdateChunk();
    }

    public void UpdateChunk() {
      if (mapDataGot) {
        float playerDistFromEdge = Mathf.Sqrt(bounds.SqrDistance(playerPos));
        bool visible = playerDistFromEdge <= maxViewDist;

        if (visible) {
          int lod = 0;
          for (int i = 0; i < details.Length - 1; i++) {
            if (playerDistFromEdge > details[i].visibleDistThresh) {
              lod = i + 1;
            } else {
              break;
            }
          }

          if (lod != prevLod) {
            LODMesh lodMesh = lodMeshes[lod];
            if (lodMesh.hasGot) {
              prevLod = lod;
              meshFilter.mesh = lodMesh.mesh;
            }
            else if (!lodMesh.hasRequested) {
              lodMesh.RequestMesh(mapData);
            }
          }

          terrainChunkVis.Add(this);
        }
        SetVisible(visible);
      }
    }

    public void SetVisible(bool active) {
      meshObject.SetActive(active);
    }

    public bool IsVisible() {
      return meshObject.activeSelf;
    }
  }

  class LODMesh {
    public Mesh mesh;
    public bool hasRequested;
    public bool hasGot;

    int lod;
    System.Action updateCallback;

    public LODMesh(int lod, System.Action cb) {
      this.lod = lod;
      this.updateCallback = cb;
    }

    public void RequestMesh(MapData mapData) {
      hasRequested = true;
      mapGenerator.RequestMeshData(mapData, lod, OnMeshDataReceived);
    }

    void OnMeshDataReceived(MeshData meshData) {
      mesh = meshData.CreateMesh();
      hasGot = true;

      updateCallback();
    }
  }

  [System.Serializable]
  public struct LODInfo {
    public int lod;
    public float visibleDistThresh;
  }
}

