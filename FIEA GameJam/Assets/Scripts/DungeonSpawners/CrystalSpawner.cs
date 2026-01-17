using System.Collections.Generic;
using UnityEngine;

public class CrystalSpawner : MonoBehaviour
{
    [Header("Crystal Prefabs")]
    [SerializeField] private GameObject commonCrystalPrefab;
    [SerializeField] private GameObject uncommonCrystalPrefab;
    [SerializeField] private GameObject rareCrystalPrefab;
    [SerializeField] private GameObject legendaryCrystalPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private Vector2Int crystalsPerRoom = new Vector2Int(2, 5);
    [SerializeField] private float crystalHeight = 0.5f;
    [SerializeField] private float minDistanceFromWalls = 1.5f;
    [SerializeField] private float minDistanceBetweenCrystals = 2f;

    [Header("Depth-Based Spawning")]
    [SerializeField] private bool spawnCrystals = true;
    [Tooltip("Percentage chance for rare crystals in gray zone (0-100)")]
    [SerializeField] private float grayZoneRareChance = 5f;
    [Tooltip("Percentage chance for rare crystals in blue zone (0-100)")]
    [SerializeField] private float blueZoneRareChance = 20f;
    [Tooltip("Percentage chance for rare crystals in red zone (0-100)")]
    [SerializeField] private float redZoneRareChance = 40f;

    private DungeonGenerator generator;
    private Transform crystalParent;
    private List<Vector3> spawnedPositions = new List<Vector3>();

    public void Initialize(DungeonGenerator dungeonGenerator, Transform parent)
    {
        generator = dungeonGenerator;
        crystalParent = new GameObject("Crystals").transform;
        crystalParent.SetParent(parent);
        spawnedPositions.Clear();
    }

    public void SpawnCrystalsInDungeon()
    {
        if (!spawnCrystals) return;

        spawnedPositions.Clear();

        foreach (Room room in generator.Rooms)
        {
            SpawnCrystalsInRoom(room);
        }

        Debug.Log($"Spawned crystals in {generator.Rooms.Count} rooms");
    }

    private void SpawnCrystalsInRoom(Room room)
    {
        int crystalCount = Random.Range(crystalsPerRoom.x, crystalsPerRoom.y + 1);
        int maxDepth = generator.GetMaxDepth();

        for (int i = 0; i < crystalCount; i++)
        {
            Vector3 spawnPosition = GetRandomPositionInRoom(room);

            if (spawnPosition != Vector3.zero && IsValidSpawnPosition(spawnPosition))
            {
                GameObject crystalPrefab = GetCrystalPrefabForDepth(room.depth, maxDepth);

                if (crystalPrefab != null)
                {
                    GameObject crystal = Instantiate(crystalPrefab, spawnPosition, Quaternion.Euler(0, Random.Range(0f, 360f), 0), crystalParent);
                    spawnedPositions.Add(spawnPosition);
                }
            }
        }
    }

    private GameObject GetCrystalPrefabForDepth(int depth, int maxDepth)
    {
        if (maxDepth == 0) maxDepth = 1;

        if (depth <= maxDepth / 3)
        {
            return GetCrystalForZone(grayZoneRareChance);
        }
        else if (depth <= (maxDepth * 2) / 3)
        {
            return GetCrystalForZone(blueZoneRareChance);
        }
        else
        {
            return GetCrystalForZone(redZoneRareChance);
        }
    }

    private GameObject GetCrystalForZone(float rareChance)
    {
        float roll = Random.Range(0f, 100f);

        if (roll < rareChance * 0.1f && legendaryCrystalPrefab != null)
        {
            return legendaryCrystalPrefab;
        }
        else if (roll < rareChance * 0.5f && rareCrystalPrefab != null)
        {
            return rareCrystalPrefab;
        }
        else if (roll < rareChance && uncommonCrystalPrefab != null)
        {
            return uncommonCrystalPrefab;
        }
        else
        {
            return commonCrystalPrefab;
        }
    }

    private Vector3 GetRandomPositionInRoom(Room room)
    {
        int attempts = 0;
        int maxAttempts = 20;

        while (attempts < maxAttempts)
        {
            float x = Random.Range(room.Left + minDistanceFromWalls, room.Right - minDistanceFromWalls);
            float z = Random.Range(room.Bottom + minDistanceFromWalls, room.Top - minDistanceFromWalls);

            Vector3 position = new Vector3(x, 0f, z);

            if (IsValidSpawnPosition(position))
            {
                return position;
            }

            attempts++;
        }

        return Vector3.zero;
    }

    private bool IsValidSpawnPosition(Vector3 position)
    {
        foreach (Vector3 existingPosition in spawnedPositions)
        {
            if (Vector3.Distance(position, existingPosition) < minDistanceBetweenCrystals)
            {
                return false;
            }
        }

        return true;
    }

    public void ClearCrystals()
    {
        if (crystalParent != null)
        {
            DestroyImmediate(crystalParent.gameObject);
        }
        spawnedPositions.Clear();
    }

#if UNITY_EDITOR
    [ContextMenu("Auto-Assign Crystal Prefabs")]
    private void AutoAssignCrystalPrefabs()
    {
        if (commonCrystalPrefab == null)
        {
            commonCrystalPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/CommonCrystal.prefab");
        }
        if (uncommonCrystalPrefab == null)
        {
            uncommonCrystalPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UncommonCrystal.prefab");
        }
        if (rareCrystalPrefab == null)
        {
            rareCrystalPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/RareCrystal.prefab");
        }
        if (legendaryCrystalPrefab == null)
        {
            legendaryCrystalPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/LegendaryCrystal.prefab");
        }

        Debug.Log("Crystal prefabs auto-assigned!");
    }
#endif
}
