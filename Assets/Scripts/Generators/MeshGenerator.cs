using UnityEngine;
using System.Collections;

public static class MeshGenerator {

  public static MeshData GenerateTerrainmesh(float[,] heightMap, float heightScale, AnimationCurve heightCurve, int lod) {
    AnimationCurve _heightCurve = new AnimationCurve(heightCurve.keys);
    
    int borderSize = heightMap.GetLength(0);
    int meshSize = borderSize - 2;
    int vertexIndex = 0;
    int increment = (lod == 0) ? 1 : lod * 2;
    int verticesPerLine = (meshSize - 1) / increment + 1;

    float topleftX = (meshSize - 1) / -2f;
    float topleftZ = (meshSize - 1) / 2f;

    MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);

    for (int y = 0; y < borderSize; y+=increment) {
      for (int x = 0; x < borderSize; x+=increment) {
        meshData.vertices[vertexIndex] = new Vector3(topleftX + x, _heightCurve.Evaluate(heightMap[x, y]) * heightScale, topleftZ - y);
        meshData.uvs[vertexIndex] = new Vector2(x / (float)borderSize, y / (float)borderSize);

        if (x < borderSize - 1 && y < borderSize - 1) {
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

    mesh.normals = CalculateNormals();

    return mesh;
  }

  Vector3[] CalculateNormals() {
    Vector3[] vertexNormals = new Vector3[vertices.Length];
    int triCount = triangles.Length / 3;

    for (int i = 0; i < triCount; i++) {
      int triIndex = i * 3;
      int vertexIndexA = triangles[triIndex];
      int vertexIndexB = triangles[triIndex + 1];
      int vertexIndexC = triangles[triIndex + 2];

      Vector3 triNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
      vertexNormals[vertexIndexA] += triNormal;
      vertexNormals[vertexIndexB] += triNormal;
      vertexNormals[vertexIndexC] += triNormal;      
    }

    for (int i = 0; i < vertexNormals.Length; i++) {
      vertexNormals[i].Normalize();
    }

    return vertexNormals;
  }

  Vector3 SurfaceNormalFromIndices(int a, int b, int c) {
    Vector3 pA = vertices[a];
    Vector3 pB = vertices[b];
    Vector3 pC = vertices[c];

    Vector3 sideAB = pB - pA;
    Vector3 sideAC = pC - pA;

    return Vector3.Cross(sideAB, sideAC).normalized;
  }
}