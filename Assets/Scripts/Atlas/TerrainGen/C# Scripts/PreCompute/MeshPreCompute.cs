using UnityEngine;
using Unity.Mathematics;

public class MeshPreCompute : MonoBehaviour
{
    void Awake()
    {
        ChunkGlobals.triangleArrays = GetLODTriangleArrays();
        ChunkGlobals.uvArrays = GetLODUVArrays();
    }

    public static int[][] GetLODTriangleArrays()
    {
        int[][] triangleArrays = new int[ChunkGlobals.lodCount][];

        for (int i = 0; i < ChunkGlobals.lodCount; i++)
        {
            triangleArrays[i] = GenerateLODTriangleArray(ChunkGlobals.meshSpaceChunkSize + 1, i);
        }

        return triangleArrays;
    }

    private static int[] GenerateLODTriangleArray(int meshLengthInVertices, int lod)
    {
        int lodMeshLengthInVertices = ((meshLengthInVertices - 1) / (int)Mathf.Pow(2, lod)) + 1;

        if (lodMeshLengthInVertices < 2)
        {
            lodMeshLengthInVertices = 2;
        }

        int numTriangles = 6 * (lodMeshLengthInVertices - 1) * (lodMeshLengthInVertices - 1);
        // Create an array to hold the triangle indices
        int[] triangles = new int[numTriangles];

        int j = 0;
        for (int i = 0; i < triangles.Length;)
        {
            if (j % meshLengthInVertices < (lodMeshLengthInVertices - 1))
            {
                triangles[i++] = j;
                triangles[i++] = j + meshLengthInVertices + 1;
                triangles[i++] = j + 1;
                triangles[i++] = j;
                triangles[i++] = j + meshLengthInVertices;
                triangles[i++] = j + meshLengthInVertices + 1;
                j++;
            }
            else
            {
                j++;
            }
        }

        // print(triangles.Length);

        return triangles;
    }

    public static float2[][] GetLODUVArrays()
    {
        float2[][] uvArrays = new float2[ChunkGlobals.lodCount][];

        for (int i = 0; i < ChunkGlobals.lodCount; i++)
        {
            uvArrays[i] = GenerateLODUVArray(i);
        }

        return uvArrays;
    }

    private static float2[] GenerateLODUVArray(int lod)
    {
        int size = ChunkGlobals.meshSpaceChunkSize / (int)Mathf.Pow(2, lod) + 1;
        float2[] uvs = new float2[size * size];
        for (int i = 0, y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++, i++)
            {
                uvs[i] = new float2(x / (float)(size - 1), y / (float)(size - 1));
            }
        }
        return uvs;
    }
}
