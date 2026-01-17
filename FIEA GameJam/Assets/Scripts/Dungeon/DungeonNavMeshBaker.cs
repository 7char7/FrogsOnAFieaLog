using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

[RequireComponent(typeof(NavMeshSurface))]
public class DungeonNavMeshBaker : MonoBehaviour
{
    [Header("Baking Settings")]
    [SerializeField] private bool bakeOnGenerate = true;
    [SerializeField] private bool logBakeTime = true;

    private NavMeshSurface navMeshSurface;

    private void Awake()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();

        if (navMeshSurface == null)
        {
            Debug.LogError("NavMeshSurface component not found! Please add it to this GameObject.");
            return;
        }

        ConfigureNavMeshSurface();
    }

    private void ConfigureNavMeshSurface()
    {
        navMeshSurface.collectObjects = CollectObjects.Children;
        navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        navMeshSurface.layerMask = LayerMask.GetMask("Default");
    }

    public void BakeNavMesh()
    {
        if (!bakeOnGenerate)
        {
            Debug.Log("NavMesh baking is disabled.");
            return;
        }

        if (navMeshSurface == null)
        {
            Debug.LogError("Cannot bake NavMesh: NavMeshSurface is null!");
            return;
        }

        float startTime = Time.realtimeSinceStartup;

        navMeshSurface.BuildNavMesh();

        float bakeTime = Time.realtimeSinceStartup - startTime;

        if (logBakeTime)
        {
            Debug.Log($"NavMesh baked successfully in {bakeTime:F3} seconds");
        }
    }

    public void ClearNavMesh()
    {
        if (navMeshSurface != null && navMeshSurface.navMeshData != null)
        {
            navMeshSurface.RemoveData();
            Debug.Log("NavMesh cleared");
        }
    }
}
