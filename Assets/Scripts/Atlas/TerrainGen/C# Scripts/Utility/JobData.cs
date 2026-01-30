using System;
using Unity.Jobs;
using Unity.Collections;

/// <summary>
/// Struct to manage job data and its disposal.
/// </summary>
/// <typeparam name="T">The type of data in the NativeArray.</typeparam>
public readonly struct JobData<T> : IDisposable where T : struct
{
    public JobHandle JobHandle { get; }
    public NativeArray<T> Data { get; }

    public JobData(JobHandle jobHandle, NativeArray<T> data)
    {
        JobHandle = jobHandle;
        Data = data;
    }

    public void Dispose()
    {
        if (Data.IsCreated)
        {
            Data.Dispose();
        }
    }
}