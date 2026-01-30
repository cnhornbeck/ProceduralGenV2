using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class ChunkConstructorManager
{
    // TODO: Maybe make these arrays with a reasonable size. Somehow get the users cpu specs and adjust the size accordingly.
    private static readonly List<ChunkConstructor> NoiseJobs = new();
    private static readonly List<ChunkConstructor> TextureJobs = new();
    private static readonly List<ChunkConstructor> FinishedConstructors = new();
    private static FixedSizeDeque<ChunkConstructionData> ChunksToGenerate = new(ChunkGlobals.chunksInRenderDistance);
    private static int MaxMeshTextureJobs = 15;
    private static int MaxNoiseJobs = MaxMeshTextureJobs * 2;
    private static HashSet<int2> chunksStartedThisFrame = new();
    public static Transform ParentTransform { get; set; }

    public static void AddChunkToQueue(int2 chunkSpacePosition)
    {
        ChunksToGenerate.AddFront(new ChunkConstructionData { chunkSpacePosition = chunkSpacePosition, lod = 0 });
    }

    public static HashSet<int2> StartChunkConstructionJobs()
    {

        if (NoiseJobs.Count >= MaxNoiseJobs)
        {
            return chunksStartedThisFrame;
        }

        // int count = 0;
        while (NoiseJobs.Count < MaxNoiseJobs && ChunksToGenerate.Count > 0)
        {
            ChunkConstructionData chunkConstructionData = ChunksToGenerate.PopFront();

            // Debug.Log("Count: " + count);
            // Debug.Log("NoiseJobs Count: " + NoiseJobs.Count);
            // Debug.Log("Chunks To Generate Count: " + ChunksToGenerate.Count);

            int2 chunkSpacePosition = chunkConstructionData.chunkSpacePosition;
            string chunkName = $"Terrain Chunk: ({chunkSpacePosition.x}, {chunkSpacePosition.y})";

            GameObject terrainChunk = new(chunkName)
            {
                transform = { parent = ParentTransform }
            };

            Chunk chunkComponent = terrainChunk.AddComponent<Chunk>();
            chunkComponent.SetParent(terrainChunk);
            ChunkRegistry.GetGeneratedChunksDictionary().Add(chunkConstructionData.chunkSpacePosition, chunkComponent);

            var chunkConstructor = new ChunkConstructor();
            chunkConstructor.StartNoiseJob(chunkConstructionData);
            NoiseJobs.Add(chunkConstructor);

            chunksStartedThisFrame.Add(chunkConstructionData.chunkSpacePosition);
            // count++;
        }

        return chunksStartedThisFrame;
    }

    public static bool IsQueuedForConstruction(int2 chunkSpacePosition)
    {
        if (ChunksToGenerate.Contains(new ChunkConstructionData { chunkSpacePosition = chunkSpacePosition, lod = 0 }))
        {
            return true;
        }
        return false;
    }

    public static void HandleChunkGeneration()
    {
        ProcessNoiseJobs();
        ProcessTextureJobs();
        InitializeFinishedChunks();
    }

    private static void ProcessNoiseJobs()
    {
        var completedNoiseJobs = new List<ChunkConstructor>();

        foreach (var job in NoiseJobs)
        {
            if (job.IsNoiseJobComplete())
            {
                job.StartTextureJob();
                TextureJobs.Add(job);
                completedNoiseJobs.Add(job);
            }
        }

        foreach (var job in completedNoiseJobs)
        {
            NoiseJobs.Remove(job);
        }
    }

    private static void ProcessTextureJobs()
    {
        var completedTextureJobs = new List<ChunkConstructor>();

        int count = 0;
        foreach (var job in TextureJobs)
        {
            if (count >= MaxMeshTextureJobs)
            {
                break;
            }

            if (job.IsTextureJobComplete())
            {
                job.CompleteTextureJob();
                job.CreateMesh();
                FinishedConstructors.Add(job);
                completedTextureJobs.Add(job);
            }
            count++;
        }

        foreach (var job in completedTextureJobs)
        {
            TextureJobs.Remove(job);
        }
    }

    private static void InitializeFinishedChunks()
    {
        foreach (var constructor in FinishedConstructors)
        {
            int2 chunkSpacePosition = constructor.GetWorldSpacePosition() / ChunkGlobals.WorldSpaceChunkSize;
            if (ChunkRegistry.GetGeneratedChunksDictionary().TryGetValue(chunkSpacePosition, out var chunk))
            {
                chunk.Initialize(constructor.GetMeshes(), constructor.GetTexture(), chunkSpacePosition);
            }
        }

        FinishedConstructors.Clear();
    }
}

public struct ChunkConstructionData
{
    public int2 chunkSpacePosition;
    public int lod;
}
