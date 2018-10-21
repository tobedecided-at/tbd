using UnityEngine;

public class TerrainGenerator : MonoBehaviour {

  public int depth = 20;
  public int width = 256;
  public int height = 256;

  public float scale = 1f;

	// Use this for initialization
	void Update () {
		Terrain terrain = GetComponent<Terrain>();
    terrain.terrainData = GenerateTerrain(terrain.terrainData);

	}

  TerrainData GenerateTerrain(TerrainData terrainData) {
    terrainData.heightmapResolution = width + 1;

    terrainData.size = new Vector3(width, depth, height);
    terrainData.SetHeights(0, 0, GenerateHeights());

    return terrainData;
  }

  float[,] GenerateHeights() {
    float[,] heights = new float[width, height];
    for (int x = 0; x < width; x++) {
      for (int y = 0; y< height; y++) {
        heights[x,y] = CalculatedHeight(x,y);
      }
    }
    return heights;
  }

  float CalculatedHeight(int x, int y) {
    float xCoord = (float)x / width * scale;
    float yCoord = (float)y / height * scale;

    return Mathf.PerlinNoise(xCoord, yCoord);
  }
}
