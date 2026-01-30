using UnityEngine;
using Unity.Mathematics;

public class ChunkSystem : MonoBehaviour
{
    void Start()
    {
        ChunkConstructorManager.ParentTransform = transform;
        print(Mathf.CeilToInt(ChunkGlobals.renderDistance * ChunkGlobals.renderDistance * math.PI));
    }

    void Update()
    {
        float3 cameraPos = Camera.main.transform.position;

        ChunkVisibilityManager.UpdateChunkVisibility(cameraPos);

        ChunkConstructorManager.HandleChunkGeneration();
    }
}

