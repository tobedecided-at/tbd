using UnityEngine;
using System.Collections;

public static class MeshGenerator {

  public static MeshData GenerateTerrainmesh(float[,] heightMap, float heightScale, AnimationCurve heightCurve, int lod) {
    AnimationCurve _heightCurve = new AnimationCurve(heightCurve.keys);

    int increment = (lod == 0) ? 1 : lod * 2;
    
    int borderSize = heightMap.GetLength(0);
    int meshSize = borderSize - 2 * increment;
    int meshSizeOriginal = borderSize - 2;

    float topleftX = (meshSizeOriginal - 1) / -2f;
    float topleftZ = (meshSizeOriginal - 1) / 2f;

    int verticesPerLine = (meshSize - 1) / increment + 1;

    MeshData meshData = new MeshData(verticesPerLine);

    int[,] vertexIndicesMap = new int[borderSize, borderSize];
    int meshVertexIndex = 0;
    int borderVertexIndex = -1;
    
    for (int y = 0; y < borderSize; y+=increment) {
      for (int x = 0; x < borderSize; x+=increment) {
        bool isBorderVertex = (y == 0 || y == borderSize - 1 || x == 0 || x == borderSize - 1);

        if (isBorderVertex) {
          vertexIndicesMap[x, y] = borderVertexIndex;
          borderVertexIndex--;
        } else {
          vertexIndicesMap[x, y] = meshVertexIndex;
          meshVertexIndex++;
        }
      }
    }

    for (int y = 0; y < borderSize; y+=increment) {
      for (int x = 0; x < borderSize; x+=increment) {
        
        int vertexIndex = vertexIndicesMap[x, y];
        Vector2 percent = new Vector2((x - increment) / (float)meshSize, (y - increment) / (float)meshSize);
        float height = _heightCurve.Evaluate(heightMap[x, y]) * heightScale;
        Vector3 vertexPos = new Vector3(topleftX + percent.x * meshSizeOriginal, height, topleftZ - percent.y * meshSizeOriginal);

        meshData.AddVertex(vertexPos, percent, vertexIndex);

        if (x < borderSize - 1 && y < borderSize - 1) {
          int a = vertexIndicesMap[x, y];
          int b = vertexIndicesMap[x + increment, y];
          int c = vertexIndicesMap[x, y + increment];
          int d = vertexIndicesMap[x + increment, y + increment];

          meshData.AddTriangle(a,d,c);
          meshData.AddTriangle(d,a,b);
        }
        vertexIndex++;
      }
    }
    
    meshData.BakeNormals();

    return meshData;
  }
}

public class MeshData {
  Vector3[] vertices;

  Vector2[] uvs;

  int[] triangles;

  Vector3[] borderVertices;
  int[] borderTriangles;

  int borderTriangleIndex;
  int triangleIndex;
  Vector3[] bakedNormals;

  public MeshData(int verticesPerLine) {
    vertices = new Vector3[verticesPerLine * verticesPerLine];
    uvs = new Vector2[verticesPerLine * verticesPerLine];
    triangles = new int[(verticesPerLine - 1) * (verticesPerLine - 1) * 6];

    borderVertices = new Vector3[verticesPerLine * 4 + 4];
    borderTriangles = new int[24 * verticesPerLine];
  }

  public void BakeNormals() {
    bakedNormals = CalculateNormals();
  }

  public void AddVertex(Vector3 pos, Vector2 uv, int index) {
    if (index < 0) {
      borderVertices[-index - 1] = pos;
    } else {
      vertices[index] = pos;
      uvs[index] = uv;
    }
  }

  public void AddTriangle(int a, int b, int c) {
    if (a < 0 || b < 0 || c < 0) {
      borderTriangles[borderTriangleIndex] = a;
      borderTriangles[borderTriangleIndex + 1] = b;
      borderTriangles[borderTriangleIndex + 2] = c;
      
      borderTriangleIndex += 3;
    } else {
      triangles[triangleIndex] = a;
      triangles[triangleIndex + 1] = b;
      triangles[triangleIndex + 2] = c;
      
      triangleIndex += 3;
    }
  }

  public Mesh CreateMesh() {
    Mesh mesh = new Mesh();

    mesh.vertices = vertices;
    mesh.triangles = triangles;
    mesh.uv = uvs;

    mesh.normals = bakedNormals;

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

    int borTriCount = borderTriangles.Length / 3;
    for (int i = 0; i < borTriCount; i++) {
      int triIndex = i * 3;
      int vertexIndexA = borderTriangles[triIndex];
      int vertexIndexB = borderTriangles[triIndex + 1];
      int vertexIndexC = borderTriangles[triIndex + 2];

      Vector3 triNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
      if (vertexIndexA >= 0) {
        vertexNormals[vertexIndexA] += triNormal;
      }
      if (vertexIndexB >= 0) {
        vertexNormals[vertexIndexB] += triNormal;
      }
      if (vertexIndexC >= 0) {
        vertexNormals[vertexIndexC] += triNormal;      
      }
    }

    for (int i = 0; i < vertexNormals.Length; i++) {
      vertexNormals[i].Normalize();
    }

    return vertexNormals;
  }

  Vector3 SurfaceNormalFromIndices(int a, int b, int c) {
    Vector3 pA = (a < 0) ? borderVertices[-a - 1] : vertices[a];
    Vector3 pB = (b < 0) ? borderVertices[-b - 1] : vertices[b];
    Vector3 pC = (c < 0) ? borderVertices[-c - 1] : vertices[c];

    Vector3 sideAB = pB - pA;
    Vector3 sideAC = pC - pA;

    return Vector3.Cross(sideAB, sideAC).normalized;
  }
}