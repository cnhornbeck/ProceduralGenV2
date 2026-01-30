using UnityEngine;

public class LODManager : MonoBehaviour
{
    Transform player;
    [SerializeField] int currentLOD;
    int meshLOD;
    MeshFilter meshFilter;
    public Mesh[] meshes;
    public Vector3 worldSpaceChunkCenter;

    void Start()
    {
        if (Camera.main == null)
        {
            Debug.LogError("No main camera found in the scene. Ensure there is a Camera tagged as 'MainCamera'.");
            return;
        }

        player = Camera.main.transform;
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("No MeshFilter component found on this GameObject.");
            return;
        }

        meshLOD = -1;
    }

    void Update()
    {
        if (meshes == null || meshes.Length == 0)
        {
            return;
        }

        CalculateLOD();
        SetLOD();
    }

    void SetLOD()
    {
        if (meshLOD != currentLOD)
        {
            if (currentLOD < 0 || currentLOD >= meshes.Length)
            {
                Debug.LogError($"currentLOD ({currentLOD}) is out of bounds. Meshes array size: {meshes.Length}.");
                return;
            }

            // Debug.Log($"Updating LOD: Setting meshLOD from {meshLOD} to {currentLOD}.");
            meshFilter.mesh = meshes[currentLOD];
            meshLOD = currentLOD;
        }
    }

    void CalculateLOD()
    {
        float distance = Vector3.Distance(worldSpaceChunkCenter, player.position);
        // Debug.Log($"Player distance from chunk center: {distance}");

        currentLOD = Mathf.FloorToInt((distance / 15) - 1);
        if (currentLOD >= meshes.Length)
        {
            currentLOD = meshes.Length - 1;
        }
        if (currentLOD < 0)
        {
            currentLOD = 0;
        }

        // Debug.Log($"Calculated LOD: {currentLOD}");
    }
}
