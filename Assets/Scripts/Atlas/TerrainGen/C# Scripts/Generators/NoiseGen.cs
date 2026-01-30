using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;
public struct NoiseSettings
{
    // Ratio between world space and mesh space
    public float Scale => 0.0025f;
    public float Lacunarity => 2.5f;
    public float Persistence => 0.3f;
    public int Octaves => 5;
    public int Seed => 1;
}

[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
public struct NoiseGenJob : IJobParallelFor
{
    [WriteOnly] public NativeArray<float3> vertexArray;
    [ReadOnly] public float scale;
    [ReadOnly] public float lacunarity;
    [ReadOnly] public float persistence;
    [ReadOnly] public int octaves;
    [ReadOnly] public int seed;
    [ReadOnly] public float worldSpaceChunkSize;
    [ReadOnly] public int meshLengthInVertices;
    [ReadOnly] public float heightMultiplier;
    [ReadOnly] public float worldSpaceChunkCenterX;
    [ReadOnly] public float worldSpaceChunkCenterZ;

    public void Execute(int index)
    {
        // Ensure index is within bounds before accessing the array
        if (index < 0 || index >= vertexArray.Length)
        {
            return; // Or handle this case appropriately
        }

        float stepSize = worldSpaceChunkSize / (meshLengthInVertices - 1);
        float initialCoord = -worldSpaceChunkSize / 2 + worldSpaceChunkCenterX;
        float zPosInitialCoord = -worldSpaceChunkSize / 2 + worldSpaceChunkCenterZ;

        float xPos = initialCoord + index % meshLengthInVertices * stepSize;
        float zPos = zPosInitialCoord + index / meshLengthInVertices * stepSize;

        float noiseValue = GenerateNoise(xPos, zPos);
        vertexArray[index] = new float3(xPos - worldSpaceChunkCenterX, noiseValue * heightMultiplier, zPos - worldSpaceChunkCenterZ);
    }

    private readonly float GenerateNoise(float x, float z)
    {
        float heightMap = 0;
        float temperatureMap = 0;

        float frequencyHeight = 1;
        float frequencyTemperature = 1;

        float amplitudeHeight = 1;
        float amplitudeTemperature = 1;

        float maxValue = 0; // Used for normalizing result to 0.0 - 1.0
        Random random = new((uint)seed);

        for (int i = 0; i < octaves; i++)
        {
            float randomValueHeight = 1000 * ((random.NextFloat() * 2) - 1);
            float randomValueTemperature = 1000 * ((random.NextFloat() * 2) - 1);

            heightMap += noise.snoise(new float2((x + randomValueHeight) * frequencyHeight * scale, (z + randomValueHeight) * frequencyHeight * scale)) * amplitudeHeight;
            temperatureMap += noise.snoise(new float2((x + randomValueTemperature) * frequencyTemperature * scale * 0.15f, (z + randomValueTemperature) * frequencyTemperature * scale * 0.15f)) * amplitudeTemperature;

            maxValue += amplitudeHeight;

            amplitudeHeight *= persistence;
            amplitudeTemperature *= 0.1f;

            frequencyHeight *= lacunarity;
            frequencyTemperature *= 5f;
        }

        // temperatureMap = 1f / (1f + math.exp(-15f * (temperatureMap - 0.5f)));

        float temperatureOffset = 1f / (1f + math.exp(20f * (temperatureMap - 0.5f))) + 0.2f;
        float blendedHeight = heightMap * temperatureOffset;
        maxValue *= 1.2f;
        return blendedHeight / maxValue;
    }
}

public struct NoiseGen
{
    public static JobData<float3> ScheduleNoiseGenJob(float2 worldSpacePosition)
    {
        NoiseSettings noiseSettings = new();
        NativeArray<float3> vertexArray = new((ChunkGlobals.meshSpaceChunkSize + 1) * (ChunkGlobals.meshSpaceChunkSize + 1), Allocator.TempJob);

        // Setup the job
        NoiseGenJob job = new()
        {
            vertexArray = vertexArray,
            scale = noiseSettings.Scale,
            lacunarity = noiseSettings.Lacunarity,
            persistence = noiseSettings.Persistence,
            octaves = noiseSettings.Octaves,
            seed = noiseSettings.Seed,
            worldSpaceChunkSize = ChunkGlobals.WorldSpaceChunkSize,
            meshLengthInVertices = ChunkGlobals.meshSpaceChunkSize + 1,
            heightMultiplier = ChunkGlobals.heightMultiplier,
            worldSpaceChunkCenterX = worldSpacePosition.x,
            worldSpaceChunkCenterZ = worldSpacePosition.y
        };

        int innerLoopBatchSize = math.min(64, (ChunkGlobals.meshSpaceChunkSize + 1) * (ChunkGlobals.meshSpaceChunkSize + 1));

        JobHandle JobHandle = job.Schedule(vertexArray.Length, innerLoopBatchSize);
        return new JobData<float3>(JobHandle, vertexArray);
    }

    public static NativeArray<float3> CompleteNoiseGenJob(JobData<float3> jobData)
    {
        jobData.JobHandle.Complete();
        return jobData.Data;
    }
}
