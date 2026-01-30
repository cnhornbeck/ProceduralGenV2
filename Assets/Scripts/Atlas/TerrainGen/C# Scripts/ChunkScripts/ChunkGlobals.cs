using Unity.Mathematics;

public struct ChunkGlobals
{
    public const int lodCount = 1;

    // Size of chunk is measured by number of edges per chunk at highest detail LOD
    public static readonly int meshSpaceChunkSize = 64;

    // Size of the actual mesh in the scene
    public const int WorldSpaceChunkSize = 100;
    public const int heightMultiplier = 100;
    public static byte renderDistance = 25;
    public static int chunksInRenderDistance => (int)math.ceil(renderDistance * renderDistance * math.PI);
    public static int[][] triangleArrays;
    public static float2[][] uvArrays;
}
