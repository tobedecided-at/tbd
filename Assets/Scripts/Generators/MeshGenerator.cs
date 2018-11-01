using UnityEngine;
using System.Collections;

public static class MeshGenerator {

  public static MeshData GenerateTerrainmesh(float[,] heightMap, float heightScale, AnimationCurve heightCurve, int lod) {
    AnimationCurve _heightCurve = new AnimationCurve(heightCurve.keys);
    
    int width = heightMap.GetLength(0);
    int height = heightMap.GetLength(1);

    int vertexIndex = 0;
    int increment = (lod == 0) ? 1 : lod * 2;
    int verticesPerLine = (width - 1) / increment + 1;

    float topleftX = (width - 1) / -2f;
    float topleftZ = (height - 1) / 2f;

    MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);

    for (int y = 0; y < height; y+=increment) {
      for (int x = 0; x < width; x+=increment) {
        meshData.vertices[vertexIndex] = new Vector3(topleftX + x, _heightCurve.Evaluate(heightMap[x, y]) * heightScale, topleftZ - y);
        meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

        if (x < width - 1 && y < height - 1) {
          meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
          meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
        }
        vertexIndex++;
      }
    }
    return meshData;
  }
}

public class MeshData {
  public Vector3[] vertices;

  public Vector2[] uvs;

  public int[] triangles;

  int triangleIndex;

  public MeshData(int meshWidth, int meshHeight) {
    vertices = new Vector3[meshWidth * meshHeight];
    uvs = new Vector2[meshWidth * meshHeight];
    triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
  }

  public void AddTriangle(int a, int b, int c) {
    triangles[triangleIndex] = a;
    triangles[triangleIndex + 1] = b;
    triangles[triangleIndex + 2] = c;
    
    triangleIndex += 3;
  }

  public Mesh CreateMesh() {
    Mesh mesh = new Mesh();

    mesh.vertices = vertices;
    mesh.triangles = triangles;
    mesh.uv = uvs;

    mesh.RecalculateNormals();

    return mesh;
  }
}