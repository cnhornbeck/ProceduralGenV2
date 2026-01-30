using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public struct ChunkRegistry
{
    private static Dictionary<int2, Chunk> generatedChunks = new();
    private static GameObject[] gameObjectPool = new GameObject[ChunkGlobals.chunksInRenderDistance];
    public static Dictionary<int2, Chunk> GetGeneratedChunksDictionary() => generatedChunks;

}
