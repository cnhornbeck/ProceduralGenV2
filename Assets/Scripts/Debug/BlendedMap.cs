using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class BlendedMap : MonoBehaviour
{

    [Header("Visualization Settings")]
    [SerializeField] private int textureSize = 96;
    [SerializeField] private Material material;
    Texture2D noiseTexture;
    HeightMap heightMap;
    TemperatureMap temperatureMap;


    private void Start()
    {
        heightMap = new();
        temperatureMap = new();

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

    private float GenerateNoise(float x, float z)
    {
        float heightMapHeight = heightMap.GenerateNoise(x, z);
        float temperatureMapHeight = temperatureMap.GenerateNoise(x, z);

        // Blend the two noise values
        float temperatureOffset = (1 - Mathf.Pow(Mathf.Sin(temperatureMapHeight * Mathf.PI), 2) + 1) / 2;
        float blendedHeight = heightMapHeight * temperatureOffset;
        float maxValue = 1;
        return blendedHeight / maxValue;
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
