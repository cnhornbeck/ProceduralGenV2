using UnityEngine;
using Unity.Collections;
using UnityEngine.Rendering;
using Unity.Mathematics;

// TODO - Implement Seamless LODs

public class MeshGen
{
    public static Mesh GenerateMesh(NativeArray<float3> vertices, int lodLevel)
    {
        Mesh terrainMesh = new Mesh
        {
            name = $"TerrainMesh_LOD{lodLevel}",
            indexFormat = IndexFormat.UInt32
        };

        int[] triangles = ChunkGlobals.triangleArrays[lodLevel];
        float2[] uvs = ChunkGlobals.uvArrays[0];

        int vertexCount = vertices.Length;
        int triangleIndexCount = triangles.Length;

        terrainMesh.SetVertexBufferParams(
            vertexCount,
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, stream: 0),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2, stream: 1)
        );
        terrainMesh.SetVertexBufferData(vertices, 0, 0, vertexCount, stream: 0);
        terrainMesh.SetVertexBufferData(uvs, 0, 0, vertexCount, stream: 1);

        terrainMesh.SetIndexBufferParams(triangleIndexCount, IndexFormat.UInt32);
        terrainMesh.SetIndexBufferData(triangles, 0, 0, triangleIndexCount);

        var bounds = new Bounds();
        terrainMesh.bounds = bounds;

        terrainMesh.SetSubMesh(0, new SubMeshDescriptor(0, triangleIndexCount)
        {
            bounds = bounds,
            vertexCount = vertexCount

        });

        // TODO: Implement Better Recalculate Bounds Method
        terrainMesh.RecalculateBounds();

        return terrainMesh;
    }
}
