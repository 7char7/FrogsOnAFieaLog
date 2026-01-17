using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject shallowEnemyPrefab;
    [SerializeField] private GameObject mediumEnemyPrefab;
    [SerializeField] private GameObject deepEnemyPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private bool spawnEnemies = true;
    [SerializeField] private bool allowEnemiesInStartRoom = false;
    [SerializeField] private float enemyHeight = 0.5f;
    [SerializeField] private float minDistanceFromWalls = 1.5f;
    [SerializeField] private float minDistanceBetweenEnemies = 3f;
    [SerializeField] private float minDistanceFromPlayer = 8f;

    [Header("Zone Enemy Count Settings")]
    [Tooltip("Total enemies in gray zone (first third of dungeon)")]
    [SerializeField] private Vector2Int grayZoneEnemyCount = new Vector2Int(2, 5);
    [Tooltip("Total enemies in blue zone (middle third of dungeon)")]
    [SerializeField] private Vector2Int blueZoneEnemyCount = new Vector2Int(4, 7);
    [Tooltip("Total enemies in red zone (final third of dungeon)")]
    [SerializeField] private Vector2Int redZoneEnemyCount = new Vector2Int(6, 10);

    private DungeonGenerator generator;
    private Transform enemyParent;
    private List<Vector3> spawnedPositions = new List<Vector3>();
    private Vector3 playerSpawnPosition;
    private Room playerSpawnRoom;

    public void Initialize(DungeonGenerator dungeonGenerator, Transform parent, Vector3 playerPosition, Room playerRoom)
    {
        generator = dungeonGenerator;
        playerSpawnPosition = playerPosition;
        playerSpawnRoom = playerRoom;
        enemyParent = new GameObject("Enemies").transform;
        enemyParent.SetParent(parent);
        spawnedPositions.Clear();
    }

    public void SpawnEnemiesInDungeon()
    {
        if (!spawnEnemies) return;

        spawnedPositions.Clear();
        int maxDepth = generator.GetMaxDepth();

        List<Room> grayZoneRooms = new List<Room>();
        List<Room> blueZoneRooms = new List<Room>();
        List<Room> redZoneRooms = new List<Room>();

        foreach (Room room in generator.Rooms)
        {
            if (!allowEnemiesInStartRoom && playerSpawnRoom != null && room == playerSpawnRoom)
            {
                continue;
            }

            int zone = GetZoneForDepth(room.depth, maxDepth);
            if (zone == 0)
                grayZoneRooms.Add(room);
            else if (zone == 1)
                blueZoneRooms.Add(room);
            else
                redZoneRooms.Add(room);
        }

        int grayEnemiesSpawned = SpawnEnemiesInZone(grayZoneRooms, grayZoneEnemyCount, "Gray");
        int blueEnemiesSpawned = SpawnEnemiesInZone(blueZoneRooms, blueZoneEnemyCount, "Blue");
        int redEnemiesSpawned = SpawnEnemiesInZone(redZoneRooms, redZoneEnemyCount, "Red");

        int totalSpawned = grayEnemiesSpawned + blueEnemiesSpawned + redEnemiesSpawned;
        Debug.Log($"Spawned {totalSpawned} enemies: Gray={grayEnemiesSpawned}, Blue={blueEnemiesSpawned}, Red={redEnemiesSpawned}");
    }

    private int GetZoneForDepth(int depth, int maxDepth)
    {
        if (maxDepth == 0) maxDepth = 1;

        if (depth <= maxDepth / 3)
        {
            return 0;
        }
        else if (depth <= (maxDepth * 2) / 3)
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }

    private int SpawnEnemiesInZone(List<Room> rooms, Vector2Int enemyCountRange, string zoneName)
    {
        if (rooms.Count == 0)
        {
            Debug.Log($"{zoneName} zone has no rooms, skipping enemy spawn");
            return 0;
        }

        int totalEnemies = Random.Range(enemyCountRange.x, enemyCountRange.y + 1);
        int spawnedCount = 0;
        int maxDepth = generator.GetMaxDepth();

        List<float> roomWeights = new List<float>();
        float totalWeight = 0f;

        foreach (Room room in rooms)
        {
            float weight = room.size.x * room.size.y;
            roomWeights.Add(weight);
            totalWeight += weight;
        }

        for (int i = 0; i < totalEnemies; i++)
        {
            float randomValue = Random.Range(0f, totalWeight);
            float cumulativeWeight = 0f;
            Room selectedRoom = null;

            for (int j = 0; j < rooms.Count; j++)
            {
                cumulativeWeight += roomWeights[j];
                if (randomValue <= cumulativeWeight)
                {
                    selectedRoom = rooms[j];
                    break;
                }
            }

            if (selectedRoom == null)
            {
                selectedRoom = rooms[rooms.Count - 1];
            }

            Vector3 spawnPosition = GetRandomPositionInRoom(selectedRoom);

            if (spawnPosition != Vector3.zero && IsValidSpawnPosition(spawnPosition))
            {
                GameObject enemyPrefab = GetEnemyPrefabForDepth(selectedRoom.depth, maxDepth);

                if (enemyPrefab != null)
                {
                    GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.Euler(0, Random.Range(0f, 360f), 0), enemyParent);
                    spawnedPositions.Add(spawnPosition);
                    spawnedCount++;
                }
            }
        }

        return spawnedCount;
    }

    private GameObject GetEnemyPrefabForDepth(int depth, int maxDepth)
    {
        if (maxDepth == 0) maxDepth = 1;

        if (depth <= maxDepth / 3)
        {
            return shallowEnemyPrefab;
        }
        else if (depth <= (maxDepth * 2) / 3)
        {
            return mediumEnemyPrefab;
        }
        else
        {
            return deepEnemyPrefab;
        }
    }

    private Vector3 GetRandomPositionInRoom(Room room)
    {
        int attempts = 0;
        int maxAttempts = 30;

        while (attempts < maxAttempts)
        {
            float x = Random.Range(room.Left + minDistanceFromWalls, room.Right - minDistanceFromWalls);
            float z = Random.Range(room.Bottom + minDistanceFromWalls, room.Top - minDistanceFromWalls);

            Vector3 randomPosition = new Vector3(x, enemyHeight, z);
            Vector3 position;
            
            if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                position = hit.position;
            }
            else
            {
                position = randomPosition;
            }
            
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
        if (Vector3.Distance(position, playerSpawnPosition) < minDistanceFromPlayer)
        {
            return false;
        }

        foreach (Vector3 existingPosition in spawnedPositions)
        {
            if (Vector3.Distance(position, existingPosition) < minDistanceBetweenEnemies)
            {
                return false;
            }
        }

        return true;
    }

    public void ClearEnemies()
    {
        if (enemyParent != null)
        {
            DestroyImmediate(enemyParent.gameObject);
        }
        spawnedPositions.Clear();
    }

#if UNITY_EDITOR
    [ContextMenu("Auto-Assign Enemy Prefabs")]
    private void AutoAssignEnemyPrefabs()
    {
        if (shallowEnemyPrefab == null)
        {
            shallowEnemyPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ShallowEnemy.prefab");
        }
        if (mediumEnemyPrefab == null)
        {
            mediumEnemyPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/MediumEnemy.prefab");
        }
        if (deepEnemyPrefab == null)
        {
            deepEnemyPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/DeepEnemy.prefab");
        }

        Debug.Log("Enemy prefabs auto-assigned!");
    }
#endif
}
