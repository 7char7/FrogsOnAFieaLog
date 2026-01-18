using UnityEngine;

[RequireComponent(typeof(DungeonGenerator))]
[RequireComponent(typeof(CrystalSpawner))]
[RequireComponent(typeof(EnemySpawner))]
[RequireComponent(typeof(DungeonNavMeshBaker))]
public class DungeonVisualizer : MonoBehaviour
{
    [Header("Visualization Settings")]
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject exitWallPrefab;
    [SerializeField] private GameObject ceilingPrefab;
    [SerializeField] private float tileSize = 1f;
    [SerializeField] private bool generateWalls = true;
    [SerializeField] private float wallHeight = 3f;
    [SerializeField] private bool generateCeiling = true;

    [Header("Depth Materials")]
    [SerializeField] private Material floorMaterial_Base;
    [SerializeField] private Material wallMaterial_Base;
    [SerializeField] private Material ceilingMaterial_Base;
    [SerializeField] private Material floorMaterial_Blue;
    [SerializeField] private Material wallMaterial_Blue;
    [SerializeField] private Material ceilingMaterial_Blue;
    [SerializeField] private Material floorMaterial_Red;
    [SerializeField] private Material wallMaterial_Red;
    [SerializeField] private Material ceilingMaterial_Red;

    [Header("Exit Wall Material")]
    [SerializeField] private Material exitWallMaterial;

    [Header("Player Spawning")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private bool spawnPlayer = true;
    [SerializeField] private float playerSpawnHeight = 1.5f;
    [SerializeField] private SpawnRoomType spawnRoomType = SpawnRoomType.First;

    [Header("Generation")]
    [SerializeField] private bool generateOnStart = true;

    private DungeonGenerator generator;
    private CrystalSpawner crystalSpawner;
    private EnemySpawner enemySpawner;
    private DungeonNavMeshBaker navMeshBaker;
    private Transform dungeonParent;
    private GameObject spawnedPlayer;
    private int[,] depthGrid;

    public enum SpawnRoomType
    {
        First,
        Random,
        Largest
    }

#if UNITY_EDITOR
    [ContextMenu("Auto-Assign Prefabs")]
    private void AutoAssignPrefabs()
    {
        if (floorPrefab == null)
        {
            floorPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/DungeonParts/FloorTile.prefab");
        }

        if (wallPrefab == null)
        {
            wallPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/DungeonParts/WallTile.prefab");
        }

        if (exitWallPrefab == null)
        {
            exitWallPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/DungeonParts/ExitWall.prefab");
        }

        if (ceilingPrefab == null)
        {
            ceilingPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/DungeonParts/CeilingTile.prefab");
        }

        if (playerPrefab == null)
        {
            playerPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/PlayerPrefab.prefab");
        }

        if (floorMaterial_Base == null)
        {
            floorMaterial_Base = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/FloorMaterial.mat");
        }
        if (wallMaterial_Base == null)
        {
            wallMaterial_Base = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/WallMaterial.mat");
        }
        if (ceilingMaterial_Base == null)
        {
            ceilingMaterial_Base = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/CeilingMaterial.mat");
        }

        if (floorMaterial_Blue == null)
        {
            floorMaterial_Blue = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/FloorMaterial_Blue.mat");
        }
        if (wallMaterial_Blue == null)
        {
            wallMaterial_Blue = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/WallMaterial_Blue.mat");
        }
        if (ceilingMaterial_Blue == null)
        {
            ceilingMaterial_Blue = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/CeilingMaterial_Blue.mat");
        }

        if (floorMaterial_Red == null)
        {
            floorMaterial_Red = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/FloorMaterial_Red.mat");
        }
        if (wallMaterial_Red == null)
        {
            wallMaterial_Red = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/WallMaterial_Red.mat");
        }
        if (ceilingMaterial_Red == null)
        {
            ceilingMaterial_Red = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/CeilingMaterial_Red.mat");
        }

        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log("Prefabs auto-assigned successfully!");
    }
#endif

    private void Start()
    {
        Debug.Log("[DungeonVisualizer] START - Script version with exit wall debugging ACTIVE");
        
        generator = GetComponent<DungeonGenerator>();
        crystalSpawner = GetComponent<CrystalSpawner>();
        enemySpawner = GetComponent<EnemySpawner>();
        navMeshBaker = GetComponent<DungeonNavMeshBaker>();

        ValidatePrefabReferences();

        if (generateOnStart)
        {
            GenerateAndVisualize();
        }
    }

    private void ValidatePrefabReferences()
    {
        bool hasErrors = false;

        if (floorPrefab == null)
        {
            Debug.LogError("[DungeonVisualizer] Floor prefab is NULL! Dungeon generation will fail.");
            hasErrors = true;
        }

        if (wallPrefab == null)
        {
            Debug.LogError("[DungeonVisualizer] Wall prefab is NULL! Dungeon generation will fail.");
            hasErrors = true;
        }

        if (exitWallPrefab == null)
        {
            Debug.LogError("[DungeonVisualizer] Exit wall prefab is NULL! Attempting auto-recovery...");
            
#if UNITY_EDITOR
            exitWallPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/DungeonParts/ExitWall.prefab");
            
            if (exitWallPrefab != null)
            {
                Debug.LogWarning("[DungeonVisualizer] Exit wall prefab auto-recovered! Saving scene is recommended.");
                UnityEditor.EditorUtility.SetDirty(this);
            }
            else
            {
                Debug.LogError("[DungeonVisualizer] Auto-recovery failed! Exit wall prefab not found at expected path.");
                hasErrors = true;
            }
#else
            hasErrors = true;
#endif
        }

        if (playerPrefab == null && spawnPlayer)
        {
            Debug.LogWarning("[DungeonVisualizer] Player prefab is NULL but spawnPlayer is true!");
        }

        if (hasErrors)
        {
            Debug.LogError("[DungeonVisualizer] CRITICAL: Missing prefab references! Right-click on DungeonVisualizer component and select 'Auto-Assign Prefabs' to fix.");
        }
    }

    public void GenerateAndVisualize()
    {
        ClearVisualization();
        generator.GenerateDungeon();
        VisualizeDungeon();

        Vector3 playerPosition = Vector3.zero;
        Room playerSpawnRoom = null;

        if (spawnPlayer && playerPrefab != null)
        {
            playerSpawnRoom = GetSpawnRoom();
            playerPosition = SpawnPlayerInRoom(playerSpawnRoom);
        }

        if (crystalSpawner != null)
        {
            crystalSpawner.Initialize(generator, dungeonParent, playerPosition);
            crystalSpawner.SpawnCrystalsInDungeon();
        }

        if (navMeshBaker != null)
        {
            navMeshBaker.BakeNavMesh();
        }

        if (enemySpawner != null)
        {
            enemySpawner.Initialize(generator, dungeonParent, playerPosition, playerSpawnRoom);
            enemySpawner.SpawnEnemiesInDungeon();
        }
    }

    private void ClearVisualization()
    {
        if (navMeshBaker != null)
        {
            navMeshBaker.ClearNavMesh();
        }

        if (dungeonParent != null)
        {
            DestroyImmediate(dungeonParent.gameObject);
        }

        if (spawnedPlayer != null)
        {
            DestroyImmediate(spawnedPlayer);
        }

        if (crystalSpawner != null)
        {
            crystalSpawner.ClearCrystals();
        }

        if (enemySpawner != null)
        {
            enemySpawner.ClearEnemies();
        }

        dungeonParent = new GameObject("Dungeon").transform;
        dungeonParent.SetParent(transform);
    }

    private void VisualizeDungeon()
    {
        bool[,] grid = generator.DungeonGrid;
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        CreateDepthGrid(width, height);

        Transform floorParent = new GameObject("Floors").transform;
        floorParent.SetParent(dungeonParent);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y])
                {
                    if (floorPrefab != null)
                    {
                        Vector3 position = new Vector3(x * tileSize, 0, y * tileSize);
                        GameObject floor = Instantiate(floorPrefab, position, Quaternion.identity, floorParent);
                        floor.transform.localScale = new Vector3(tileSize, tileSize, tileSize);
                        floor.tag = "Ground";

                        Material floorMat = GetMaterialForDepth(depthGrid[x, y], floorMaterial_Base, floorMaterial_Blue, floorMaterial_Red);
                        if (floorMat != null)
                        {
                            MeshRenderer renderer = floor.GetComponentInChildren<MeshRenderer>();
                            if (renderer != null)
                            {
                                renderer.material = floorMat;
                            }
                        }
                    }
                }
            }
        }

        if (generateCeiling && ceilingPrefab != null)
        {
            GenerateCeiling(grid, width, height);
        }

        if (generateWalls && wallPrefab != null)
        {
            GenerateWalls(grid, width, height);
        }
    }

    private void GenerateWalls(bool[,] grid, int width, int height)
    {
        Transform wallParent = new GameObject("Walls").transform;
        wallParent.SetParent(dungeonParent);

        Debug.Log($"[DungeonVisualizer] GenerateWalls called. exitWallPrefab is {(exitWallPrefab == null ? "NULL" : "assigned")}");

        if (generator.Rooms == null || generator.Rooms.Count == 0)
        {
            Debug.LogError("CRITICAL: No rooms generated! Exit wall cannot be placed!");
            return;
        }

        Room startRoom = generator.Rooms[0];
        Vector2Int exitWallPosition = FindBackWallPosition(startRoom);
        
        Debug.Log($"Exit wall WILL be placed in starting room at position: {exitWallPosition}, room bounds: ({startRoom.Left}, {startRoom.Bottom}) to ({startRoom.Right}, {startRoom.Top})");
        Debug.Log($"[DungeonVisualizer] Checking grid at exit position: grid[{exitWallPosition.x}, {exitWallPosition.y}] = {(exitWallPosition.x < width && exitWallPosition.y < height ? grid[exitWallPosition.x, exitWallPosition.y].ToString() : "OUT OF BOUNDS")}");
        if (exitWallPosition.y > 0 && exitWallPosition.x < width && exitWallPosition.y < height)
        {
            Debug.Log($"[DungeonVisualizer] Grid below exit position: grid[{exitWallPosition.x}, {exitWallPosition.y - 1}] = {grid[exitWallPosition.x, exitWallPosition.y - 1]}");
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y])
                {
                    if (x == 0 || !grid[x - 1, y])
                    {
                        bool isExitWallLeft = IsExitWallAtPosition(exitWallPosition, x, y, WallDirection.Left);
                        CreateWall(x * tileSize - tileSize / 2, y * tileSize, Vector3.forward, wallParent, x, y, isExitWallLeft);
                    }
                    if (x == width - 1 || !grid[x + 1, y])
                    {
                        bool isExitWallRight = IsExitWallAtPosition(exitWallPosition, x, y, WallDirection.Right);
                        CreateWall(x * tileSize + tileSize / 2, y * tileSize, Vector3.forward, wallParent, x, y, isExitWallRight);
                    }
                    
                    bool isExitWallBottom = IsExitWallAtPosition(exitWallPosition, x, y, WallDirection.Bottom);
                    if (y == 0 || !grid[x, y - 1] || isExitWallBottom)
                    {
                        if (isExitWallBottom)
                        {
                            Debug.Log($"[DungeonVisualizer] FOUND EXIT WALL POSITION! Creating exit wall at grid ({x}, {y}) - FORCING creation even if blocked");
                        }
                        CreateWall(x * tileSize, y * tileSize - tileSize / 2, Vector3.right, wallParent, x, y, isExitWallBottom);
                    }
                    
                    if (y == height - 1 || !grid[x, y + 1])
                    {
                        bool isExitWallTop = IsExitWallAtPosition(exitWallPosition, x, y, WallDirection.Top);
                        CreateWall(x * tileSize, y * tileSize + tileSize / 2, Vector3.right, wallParent, x, y, isExitWallTop);
                    }
                }
            }
        }
    }

    private void CreateWall(float x, float z, Vector3 direction, Transform parent, int gridX, int gridY, bool isExitWall = false)
    {
        Vector3 position = new Vector3(x, wallHeight / 2, z);
        Quaternion rotation = Quaternion.LookRotation(direction);

        GameObject prefabToUse;
        
        if (isExitWall)
        {
            if (exitWallPrefab == null)
            {
                Debug.LogError($"CRITICAL: Exit wall prefab is NULL! Falling back to regular wall prefab. Check DungeonVisualizer component in scene.");
                prefabToUse = wallPrefab;
            }
            else
            {
                prefabToUse = exitWallPrefab;
            }
        }
        else
        {
            prefabToUse = wallPrefab;
        }
        
        if (prefabToUse == null)
        {
            Debug.LogError($"CRITICAL: {(isExitWall ? "Exit wall" : "Wall")} prefab is NULL! Cannot create wall.");
            return;
        }
        
        GameObject wall = Instantiate(prefabToUse, position, rotation, parent);
        wall.transform.localScale = new Vector3(tileSize, wallHeight, tileSize);

        if (isExitWall)
        {
            wall.name = "ExitWall";
            ConfigureExitWallCollider(wall);
            Debug.Log($"EXIT WALL CREATED at position: {position}, grid coords: ({gridX}, {gridY})");
        }

        Material wallMat;
        if (isExitWall && exitWallMaterial != null)
        {
            wallMat = exitWallMaterial;
        }
        else
        {
            wallMat = GetMaterialForDepth(depthGrid[gridX, gridY], wallMaterial_Base, wallMaterial_Blue, wallMaterial_Red);
        }

        if (wallMat != null)
        {
            MeshRenderer renderer = wall.GetComponentInChildren<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = wallMat;
            }
        }
    }

    private void GenerateCeiling(bool[,] grid, int width, int height)
    {
        Transform ceilingParent = new GameObject("Ceiling").transform;
        ceilingParent.SetParent(dungeonParent);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y])
                {
                    Vector3 position = new Vector3(x * tileSize, wallHeight, y * tileSize);
                    GameObject ceiling = Instantiate(ceilingPrefab, position, Quaternion.identity, ceilingParent);
                    ceiling.transform.localScale = new Vector3(tileSize, tileSize, tileSize);

                    Material ceilingMat = GetMaterialForDepth(depthGrid[x, y], ceilingMaterial_Base, ceilingMaterial_Blue, ceilingMaterial_Red);
                    if (ceilingMat != null)
                    {
                        MeshRenderer renderer = ceiling.GetComponentInChildren<MeshRenderer>();
                        if (renderer != null)
                        {
                            renderer.material = ceilingMat;
                        }
                    }
                }
            }
        }
    }

    private void CreateDepthGrid(int width, int height)
    {
        depthGrid = new int[width, height];

        foreach (Room room in generator.Rooms)
        {
            for (int x = room.Left; x < room.Right; x++)
            {
                for (int y = room.Bottom; y < room.Top; y++)
                {
                    if (x >= 0 && x < width && y >= 0 && y < height)
                    {
                        depthGrid[x, y] = room.depth;
                    }
                }
            }
        }

        foreach (Corridor corridor in generator.Corridors)
        {
            Vector2Int start = corridor.start;
            Vector2Int end = corridor.end;
            int halfWidth = corridor.width / 2;

            int minX = Mathf.Min(start.x, end.x);
            int maxX = Mathf.Max(start.x, end.x);
            int minY = Mathf.Min(start.y, end.y);
            int maxY = Mathf.Max(start.y, end.y);

            for (int x = minX - halfWidth; x <= maxX + halfWidth; x++)
            {
                for (int y = minY - halfWidth; y <= maxY + halfWidth; y++)
                {
                    if (x >= 0 && x < width && y >= 0 && y < height)
                    {
                        int depth = GetNearestRoomDepth(new Vector2Int(x, y));
                        depthGrid[x, y] = depth;
                    }
                }
            }
        }
    }

    private int GetNearestRoomDepth(Vector2Int position)
    {
        foreach (Room room in generator.Rooms)
        {
            if (room.Contains(position))
            {
                return room.depth;
            }
        }

        float minDistance = float.MaxValue;
        int closestDepth = 0;

        foreach (Room room in generator.Rooms)
        {
            float distance = Vector2Int.Distance(position, room.Center);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestDepth = room.depth;
            }
        }

        return closestDepth;
    }

    private Material GetMaterialForDepth(int depth, Material baseMat, Material blueMat, Material redMat)
    {
        int maxDepth = generator.GetMaxDepth();

        if (maxDepth == 0)
        {
            return baseMat;
        }

        if (depth <= maxDepth / 3)
        {
            return baseMat;
        }
        else if (depth <= (maxDepth * 2) / 3)
        {
            return blueMat;
        }
        else
        {
            return redMat;
        }
    }

    private Vector3 SpawnPlayerInRoom(Room spawnRoom)
    {
        if (generator.Rooms == null || generator.Rooms.Count == 0)
        {
            Debug.LogWarning("Cannot spawn player: No rooms generated!");
            return Vector3.zero;
        }

        Vector3 spawnPosition = new Vector3(
            spawnRoom.Center.x * tileSize,
            playerSpawnHeight,
            spawnRoom.Center.y * tileSize
        );

        spawnedPlayer = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        spawnedPlayer.name = "Player";

        Debug.Log($"Player spawned in room at depth {spawnRoom.depth}, position: {spawnPosition}");

        return spawnPosition;
    }

    private Room GetSpawnRoom()
    {
        switch (spawnRoomType)
        {
            case SpawnRoomType.First:
                return generator.Rooms[0];

            case SpawnRoomType.Random:
                return generator.Rooms[Random.Range(0, generator.Rooms.Count)];

            case SpawnRoomType.Largest:
                Room largest = generator.Rooms[0];
                int largestArea = largest.size.x * largest.size.y;

                foreach (Room room in generator.Rooms)
                {
                    int area = room.size.x * room.size.y;
                    if (area > largestArea)
                    {
                        largest = room;
                        largestArea = area;
                    }
                }
                return largest;

            default:
                return generator.Rooms[0];
        }
    }

    public Vector3 GetPlayerSpawnPosition()
    {
        if (generator.Rooms == null || generator.Rooms.Count == 0)
        {
            return Vector3.zero;
        }

        Room spawnRoom = GetSpawnRoom();
        return new Vector3(
            spawnRoom.Center.x * tileSize,
            playerSpawnHeight,
            spawnRoom.Center.y * tileSize
        );
    }

    private void ConfigureExitWallCollider(GameObject exitWall)
    {
        BoxCollider[] colliders = exitWall.GetComponentsInChildren<BoxCollider>();
        
        foreach (BoxCollider collider in colliders)
        {
            if (collider.gameObject == exitWall)
            {
                collider.isTrigger = true;
                collider.center = Vector3.zero;
                collider.size = new Vector3(1.5f, 3f, 1.5f);
            }
            else
            {
                collider.enabled = false;
            }
        }
    }

    private enum WallDirection
    {
        Left,
        Right,
        Top,
        Bottom
    }

    private Vector2Int FindBackWallPosition(Room room)
    {
        return new Vector2Int(room.Center.x, room.Bottom);
    }

    private bool IsExitWallAtPosition(Vector2Int exitWallPos, int x, int y, WallDirection direction)
    {
        if (direction == WallDirection.Bottom)
        {
            bool isMatch = x == exitWallPos.x && y == exitWallPos.y;
            if (isMatch)
            {
                Debug.Log($"[DungeonVisualizer] EXIT WALL MATCH! x={x}, y={y}, exitWallPos={exitWallPos}, direction={direction}");
            }
            return isMatch;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        if (generator == null)
        {
            generator = GetComponent<DungeonGenerator>();
        }

        if (generator.Rooms == null || generator.Rooms.Count == 0)
        {
            return;
        }

        Gizmos.color = Color.green;
        foreach (Room room in generator.Rooms)
        {
            Vector3 center = new Vector3(
                (room.Left + room.Right) / 2f * tileSize,
                0.1f,
                (room.Bottom + room.Top) / 2f * tileSize
            );
            Vector3 size = new Vector3(
                room.size.x * tileSize,
                0.1f,
                room.size.y * tileSize
            );
            Gizmos.DrawWireCube(center, size);
        }

        Gizmos.color = Color.yellow;
        foreach (Corridor corridor in generator.Corridors)
        {
            Vector3 start = new Vector3(corridor.start.x * tileSize, 0.1f, corridor.start.y * tileSize);
            Vector3 end = new Vector3(corridor.end.x * tileSize, 0.1f, corridor.end.y * tileSize);
            Gizmos.DrawLine(start, end);
        }
    }
}
