using UnityEngine;
using Unity.Mathematics;

// A chunk is a single unit of terrain in the world.
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
// [RequireComponent(typeof(LODManager))]
public class Chunk : MonoBehaviour
{
    public float2 WorldSpacePosition { get; private set; }
    public float3 WorldSpaceChunkCenter { get; private set; }
    private GameObject parent;

    // Initialize the chunk with its basic properties and generate its initial content.
    public void Initialize(Mesh mesh, Texture2D texture, float2 chunkSpacePosition)
    {
        WorldSpacePosition = chunkSpacePosition * ChunkGlobals.WorldSpaceChunkSize;

        // Set the parent's position based on the world space position.
        float3 worldPosition = new(WorldSpacePosition.x, 0, WorldSpacePosition.y);
        parent.transform.position = worldPosition;

        SetVisible(false);

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        // LODManager lodManager = GetComponent<LODManager>();

        SetMesh(meshFilter, mesh);
        SetTexture(meshRenderer, texture);
        // lodManager.worldSpaceChunkCenter = worldPosition;
        // lodManager.meshes = meshes;
    }

    public void SetParent(GameObject parent)
    {
        this.parent = parent;
    }

    // Sets the visibility of the chunk.
    public void SetVisible(bool visible)
    {
        parent.SetActive(visible);
    }

    // Returns visibility status of the chunk.
    public bool IsVisible()
    {
        return parent.activeSelf;
    }

    // Sets the mesh for the chunk.
    private void SetMesh(MeshFilter meshFilter, Mesh mesh)
    {
        meshFilter.mesh = mesh;
    }

    // Sets the texture for the chunk.
    private void SetTexture(MeshRenderer meshRenderer, Texture texture)
    {
        meshRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
        // meshRenderer.material = new Material(Shader.Find("Standard"))
        {
            mainTexture = texture
        };
        meshRenderer.material.SetFloat("_Glossiness", 0.0f);
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }
}

public struct ChunkData
{
    public Mesh[] meshes;
    public Texture2D texture;
    public float3 worldSpaceChunkCenter;

    public ChunkData(float3 worldSpaceChunkCenter, Mesh[] meshes, Texture2D texture)
    {
        this.worldSpaceChunkCenter = worldSpaceChunkCenter;
        this.meshes = meshes;
        this.texture = texture;
    }
}
