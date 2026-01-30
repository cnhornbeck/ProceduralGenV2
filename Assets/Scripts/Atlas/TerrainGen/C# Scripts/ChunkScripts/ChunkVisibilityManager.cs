using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

public class ChunkVisibilityManager : MonoBehaviour
{
    static HashSet<int2> chunksVisibleLastFrame = new();
    static HashSet<int2> chunksVisibleThisFrame = new();
    static HashSet<int2> chunksStartedThisFrame = new();

    public static void UpdateChunkVisibility(float3 cameraPos)
    {
        byte renderDistance = ChunkGlobals.renderDistance;

        chunksVisibleThisFrame = GetVisibleChunkPositionsWithinRadius(cameraPos, renderDistance);
        // Loops over the position of each chunk that should be visible
        foreach (int2 chunkSpacePosition in chunksVisibleThisFrame)
        {
            // Checks if a chunk has been generated at position
            if (ChunkRegistry.GetGeneratedChunksDictionary().TryGetValue(chunkSpacePosition, out Chunk chunk))
            {
                // Checks if chunk is not visible
                if (!chunk.IsVisible())
                {
                    // Enables the chunk
                    chunk.SetVisible(true);
                }
            }
            else if (!ChunkConstructorManager.IsQueuedForConstruction(chunkSpacePosition))
            {
                ChunkConstructorManager.AddChunkToQueue(chunkSpacePosition);
                // print("Requesting chunk generation at " + position);
            }
        }

        chunksStartedThisFrame = ChunkConstructorManager.StartChunkConstructionJobs();
        // This gets all chunks that are in chunksVisibleLastFrame that are not in visibleChunkPositions and stores that value in chunksVisibleLastFrame
        chunksVisibleLastFrame.ExceptWith(chunksVisibleThisFrame);
        chunksVisibleLastFrame.IntersectWith(chunksStartedThisFrame);
        // Goes through every position in chunksVisibleLastFrame, which now contains only chunks that were visible last frame that should NOT be visible this frame, and disables every chunk at each position
        foreach (int2 chunkSpacePosition in chunksVisibleLastFrame)
        {
            ChunkRegistry.GetGeneratedChunksDictionary()[chunkSpacePosition].SetVisible(false);
        }
        // This is that last thing to happen before the next frame so now all chunks that are visible this frame will be visible in the last frame one frame from now
        chunksVisibleLastFrame = chunksVisibleThisFrame;
    }

    private static HashSet<int2> GetVisibleChunkPositionsWithinRadius(float3 currentPositionFlt3, byte renderDistance)
    {
        ushort estimatedCapacity = (ushort)(Mathf.PI * renderDistance * renderDistance);
        HashSet<int2> visibleChunkPositionsWithinRadius = new HashSet<int2>(estimatedCapacity);

        // Get current chunk position in "chunk space"
        int currentChunkX = Mathf.RoundToInt(currentPositionFlt3.x / ChunkGlobals.WorldSpaceChunkSize);
        int currentChunkZ = Mathf.RoundToInt(currentPositionFlt3.z / ChunkGlobals.WorldSpaceChunkSize);

        float squaredRenderDistance = renderDistance * renderDistance;

        // Loop over chunk offsets in a square
        for (int xOffset = -renderDistance; xOffset <= renderDistance; xOffset++)
        {
            for (int zOffset = -renderDistance; zOffset <= renderDistance; zOffset++)
            {
                // Compute squared distance in chunk space
                float distanceSquared = xOffset * xOffset + zOffset * zOffset;

                // Check if within the circular render distance
                if (distanceSquared <= squaredRenderDistance)
                {
                    int chunkX = currentChunkX + xOffset;
                    int chunkZ = currentChunkZ + zOffset;

                    visibleChunkPositionsWithinRadius.Add(new int2(chunkX, chunkZ));
                }
            }
        }

        // foreach (int2 chunkSpacePosition in visibleChunkPositionsWithinRadius)
        // {
        //     print("Visible chunk at " + chunkSpacePosition);
        // }
        
        return visibleChunkPositionsWithinRadius;
    }
}

