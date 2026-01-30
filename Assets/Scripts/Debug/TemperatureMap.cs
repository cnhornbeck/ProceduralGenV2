using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class TemperatureMap : MonoBehaviour
{
    [Header("Noise Settings")]
    [SerializeField] private float scale = 1.8f;
    [SerializeField] private float lacunarity = 5f;
    [SerializeField] private float persistence = 0.1f;
    [SerializeField] private int octaves = 3;
    [SerializeField] private int seed = 10;
    [SerializeField] private float temperatureSeparationFactor = 15f;

    [Header("Visualization Settings")]
    [SerializeField] private int textureSize = 96;
    [SerializeField] private Material material;
    Texture2D noiseTexture;


    private void Start()
    {
        if (material == null)
        {
            // Create a new material using the default unlit shader
            material = new Material(Shader.Find("Unlit/Texture"));
            GetComponent<MeshRenderer>().material = material;
        }

        GenerateNoiseTexture();
        CreateQuad();
    }

    // NEW: Update method to regenerate texture every frame
    private void Update()
    {
        UpdateNoiseTexture();
    }

    private void GenerateNoiseTexture()
    {
        noiseTexture = new(textureSize, textureSize, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point, // Ensures sharp edges, important for pixel art or blocky styles
            wrapMode = TextureWrapMode.Clamp // Prevents the texture from wrapping around the mesh
        };

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float xCoord = (float)x / textureSize;
                float yCoord = (float)y / textureSize;

                float noiseValue = GenerateNoise(xCoord, yCoord);
                // Convert the noise value to grayscale color
                Color color = new(noiseValue, noiseValue, noiseValue, 1);
                noiseTexture.SetPixel(x, y, color);
            }
        }

        noiseTexture.Apply();
        material.mainTexture = noiseTexture;
    }

    private void UpdateNoiseTexture()
    {
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float xCoord = (float)x / textureSize;
                float yCoord = (float)y / textureSize;

                float noiseValue = GenerateNoise(xCoord, yCoord);
                // Convert the noise value to grayscale color
                Color color = new(noiseValue, noiseValue, noiseValue, 1);
                noiseTexture.SetPixel(x, y, color);
            }
        }

        noiseTexture.Apply();
        material.mainTexture = noiseTexture;
    }

    public float GenerateNoise(float x, float z)
    {
        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float maxValue = 0;
        Random random = new Random((uint)seed);

        for (int i = 0; i < octaves; i++)
        {
            float randomValue = 1000 * ((random.NextFloat() * 2) - 1);

            total += noise.snoise(new float2(
                (x + randomValue) * frequency * scale,
                (z + randomValue) * frequency * scale)
            ) * amplitude;

            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= lacunarity;
        }

        // Normalize to 0-1 range
        return SigmoidPush((total / maxValue + 1) * 0.5f, temperatureSeparationFactor);
    }

    public static float SigmoidPush(float x, float k)
    {
        // Compute the sigmoid transformation centered at 0.5.
        // Mathf.Exp is used for exponentiation.
        float exponent = -k * (x - 0.5f);
        float result = 1f / (1f + Mathf.Exp(exponent));
        return result;
    }

    private void CreateQuad()
    {
        // Create mesh
        Mesh mesh = new();

        // Vertices
        Vector3[] vertices = new Vector3[4]
        {
            new(-0.5f, -0.5f, 0),
            new(0.5f, -0.5f, 0),
            new(-0.5f, 0.5f, 0),
            new(0.5f, 0.5f, 0)
        };

        // UVs
        Vector2[] uvs = new Vector2[4]
        {
            new(0, 0),
            new(1, 0),
            new(0, 1),
            new(1, 1)
        };

        // Triangles
        int[] triangles = new int[6]
        {
            0, 2, 1,
            2, 3, 1
        };

        // Assign to mesh
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        // Assign mesh to the GameObject
        if (!TryGetComponent<MeshFilter>(out var meshFilter))
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        if (!TryGetComponent<MeshRenderer>(out _))
        {
            gameObject.AddComponent<MeshRenderer>();
        }

        meshFilter.mesh = mesh;
    }
}
