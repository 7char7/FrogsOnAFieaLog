using System.Collections.Generic;
using UnityEngine;

public class ChunkBasedDungeonGenerator : MonoBehaviour
{
    [Header("Chunk Library")]
    [SerializeField] private ChunkLibrary chunkLibrary;
    
    [Header("Generation Settings")]
    [SerializeField] private int numberOfRooms = 10;
    [SerializeField] private int maxPlacementAttempts = 100;
    [SerializeField] private int roomPadding = 2;
    [SerializeField] private bool useRandomSeed = true;
    [SerializeField] private int seed = 0;
    
    [Header("Generation Area")]
    [SerializeField] private int dungeonWidth = 100;
    [SerializeField] private int dungeonHeight = 100;
    
    [Header("Player Spawning")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private bool spawnPlayer = true;
    [SerializeField] private float playerSpawnHeight = 1.5f;
    
    [Header("Generation")]
    [SerializeField] private bool generateOnStart = true;
    
    private List<ChunkPlacement> placedChunks = new List<ChunkPlacement>();
    private List<Room> rooms = new List<Room>();
    private Dictionary<Room, List<Room>> roomConnections = new Dictionary<Room, List<Room>>();
    private bool[,] occupiedGrid;
    private Transform dungeonParent;
    private GameObject spawnedPlayer;
    
    public List<ChunkPlacement> PlacedChunks => placedChunks;
    public List<Room> Rooms => rooms;
    public Dictionary<Room, List<Room>> RoomConnections => roomConnections;
    
    private void Start()
    {
        if (generateOnStart)
        {
            GenerateChunkBasedDungeon();
        }
    }
    
    public void GenerateChunkBasedDungeon()
    {
        if (!ValidateChunkLibrary()) return;
        
        if (!useRandomSeed)
        {
            Random.InitState(seed);
        }
        
        ClearDungeon();
        GenerateRooms();
        ConnectRooms();
        CalculateRoomDepths();
        InstantiateChunks();
        
        if (spawnPlayer && playerPrefab != null)
        {
            SpawnPlayer();
        }
        
        Debug.Log($"Chunk-based dungeon generated with {placedChunks.Count} chunks!");
    }
    
    private bool ValidateChunkLibrary()
    {
        if (chunkLibrary == null)
        {
            Debug.LogError("ChunkLibrary is not assigned!");
            return false;
        }
        
        if (!chunkLibrary.HasStandardRooms)
        {
            Debug.LogError("ChunkLibrary has no standard room chunks!");
            return false;
        }
        
        return true;
    }
    
    private void ClearDungeon()
    {
        placedChunks.Clear();
        rooms.Clear();
        roomConnections.Clear();
        occupiedGrid = new bool[dungeonWidth, dungeonHeight];
        
        if (dungeonParent != null)
        {
            DestroyImmediate(dungeonParent.gameObject);
        }
        
        if (spawnedPlayer != null)
        {
            DestroyImmediate(spawnedPlayer);
        }
        
        dungeonParent = new GameObject("ChunkDungeon").transform;
        dungeonParent.SetParent(transform);
    }
    
    private void GenerateRooms()
    {
        int roomsPlaced = 0;
        int attempts = 0;
        
        while (roomsPlaced < numberOfRooms && attempts < maxPlacementAttempts * numberOfRooms)
        {
            attempts++;
            
            GameObject chunkPrefab = roomsPlaced == 0 && chunkLibrary.HasStartRooms 
                ? chunkLibrary.GetRandomStartRoom() 
                : chunkLibrary.GetRandomStandardRoom();
            
            if (chunkPrefab == null) continue;
            
            DungeonChunk chunkComponent = chunkPrefab.GetComponent<DungeonChunk>();
            if (chunkComponent == null)
            {
                Debug.LogWarning($"Chunk prefab {chunkPrefab.name} is missing DungeonChunk component!");
                continue;
            }
            
            Vector2Int chunkSize = chunkComponent.ChunkSize;
            Vector2Int roomPosition;
            
            if (roomsPlaced == 0)
            {
                roomPosition = new Vector2Int(0, 0);
            }
            else
            {
                roomPosition = FindAdjacentPosition(chunkSize, attempts);
            }
            
            Room newRoom = new Room(roomPosition, chunkSize);
            
            if (!RoomOverlapsExisting(newRoom))
            {
                rooms.Add(newRoom);
                newRoom.chunkPrefab = chunkPrefab;
                roomsPlaced++;
            }
        }
        
        Debug.Log($"Generated {roomsPlaced} rooms in {attempts} attempts");
    }
    
    private Vector2Int FindAdjacentPosition(Vector2Int chunkSize, int attemptNumber)
    {
        Room baseRoom = rooms[Random.Range(0, rooms.Count)];
        
        Vector2Int[] offsets = new Vector2Int[]
        {
            new Vector2Int(baseRoom.size.x, 0),
            new Vector2Int(-chunkSize.x, 0),
            new Vector2Int(0, baseRoom.size.y),
            new Vector2Int(0, -chunkSize.y)
        };
        
        int offsetIndex = (attemptNumber / 4) % offsets.Length;
        return baseRoom.position + offsets[offsetIndex];
    }
    
    private bool RoomOverlapsExisting(Room newRoom)
    {
        foreach (Room existingRoom in rooms)
        {
            if (newRoom.Overlaps(existingRoom, roomPadding))
            {
                return true;
            }
        }
        return false;
    }
    
    private void ConnectRooms()
    {
        if (rooms.Count == 0) return;
        
        List<Room> unconnected = new List<Room>(rooms);
        List<Room> connected = new List<Room>();
        
        Room startRoom = unconnected[0];
        connected.Add(startRoom);
        unconnected.RemoveAt(0);
        roomConnections[startRoom] = new List<Room>();
        
        while (unconnected.Count > 0)
        {
            Room closestUnconnected = null;
            Room closestConnected = null;
            float shortestDistance = float.MaxValue;
            
            foreach (Room connectedRoom in connected)
            {
                foreach (Room unconnectedRoom in unconnected)
                {
                    float distance = Vector2Int.Distance(connectedRoom.Center, unconnectedRoom.Center);
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        closestConnected = connectedRoom;
                        closestUnconnected = unconnectedRoom;
                    }
                }
            }
            
            if (closestUnconnected != null)
            {
                if (!roomConnections.ContainsKey(closestConnected))
                {
                    roomConnections[closestConnected] = new List<Room>();
                }
                if (!roomConnections.ContainsKey(closestUnconnected))
                {
                    roomConnections[closestUnconnected] = new List<Room>();
                }
                
                roomConnections[closestConnected].Add(closestUnconnected);
                roomConnections[closestUnconnected].Add(closestConnected);
                
                connected.Add(closestUnconnected);
                unconnected.Remove(closestUnconnected);
            }
        }
    }
    
    private void CalculateRoomDepths()
    {
        if (rooms.Count == 0) return;
        
        Room startRoom = rooms[0];
        Queue<Room> queue = new Queue<Room>();
        HashSet<Room> visited = new HashSet<Room>();
        
        startRoom.depth = 0;
        queue.Enqueue(startRoom);
        visited.Add(startRoom);
        
        while (queue.Count > 0)
        {
            Room currentRoom = queue.Dequeue();
            
            if (roomConnections.ContainsKey(currentRoom))
            {
                foreach (Room neighbor in roomConnections[currentRoom])
                {
                    if (!visited.Contains(neighbor))
                    {
                        neighbor.depth = currentRoom.depth + 1;
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }
    }
    
    private void InstantiateChunks()
    {
        Transform roomsParent = new GameObject("Rooms").transform;
        roomsParent.SetParent(dungeonParent);
        
        foreach (Room room in rooms)
        {
            if (room.chunkPrefab == null) continue;
            
            Vector3 worldPosition = new Vector3(
                room.position.x + (room.size.x / 2f),
                0,
                room.position.y + (room.size.y / 2f)
            );
            
            GameObject chunkInstance = Instantiate(room.chunkPrefab, worldPosition, Quaternion.identity, roomsParent);
            chunkInstance.name = $"Room_{room.position.x}_{room.position.y}";
            
            DungeonChunk chunkComponent = chunkInstance.GetComponent<DungeonChunk>();
            if (chunkComponent != null)
            {
                chunkComponent.DepthZone = room.depth;
            }
            
            ChunkPlacement placement = new ChunkPlacement
            {
                chunk = chunkInstance,
                room = room,
                position = worldPosition
            };
            
            placedChunks.Add(placement);
        }
        
        // Corridors disabled - rooms connect directly when adjacent
        // if (chunkLibrary.HasCorridors)
        // {
        //     GenerateCorridorChunks();
        // }
    }
    
    private void GenerateCorridorChunks()
    {
        Transform corridorsParent = new GameObject("Corridors").transform;
        corridorsParent.SetParent(dungeonParent);
        
        foreach (var connection in roomConnections)
        {
            Room fromRoom = connection.Key;
            
            foreach (Room toRoom in connection.Value)
            {
                if (fromRoom.Center.x < toRoom.Center.x || 
                    (fromRoom.Center.x == toRoom.Center.x && fromRoom.Center.y < toRoom.Center.y))
                {
                    Vector3 midPoint = new Vector3(
                        (fromRoom.Center.x + toRoom.Center.x) / 2f,
                        0,
                        (fromRoom.Center.y + toRoom.Center.y) / 2f
                    );
                    
                    GameObject corridorPrefab = chunkLibrary.GetRandomStraightCorridor();
                    if (corridorPrefab != null)
                    {
                        Vector3 direction = new Vector3(
                            toRoom.Center.x - fromRoom.Center.x,
                            0,
                            toRoom.Center.y - fromRoom.Center.y
                        ).normalized;
                        
                        Quaternion rotation = Quaternion.LookRotation(direction);
                        GameObject corridor = Instantiate(corridorPrefab, midPoint, rotation, corridorsParent);
                        corridor.name = $"Corridor_{fromRoom.Center}_{toRoom.Center}";
                    }
                }
            }
        }
    }
    
    private void SpawnPlayer()
    {
        if (rooms.Count == 0 || placedChunks.Count == 0) return;
        
        ChunkPlacement startChunk = placedChunks[0];
        DungeonChunk chunkComponent = startChunk.chunk.GetComponent<DungeonChunk>();
        
        Vector3 spawnPosition;
        if (chunkComponent != null && chunkComponent.PlayerSpawnPoint != null)
        {
            spawnPosition = chunkComponent.PlayerSpawnPoint.position;
        }
        else
        {
            spawnPosition = startChunk.position + Vector3.up * playerSpawnHeight;
        }
        
        spawnedPlayer = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        spawnedPlayer.name = "Player";
        
        Debug.Log($"Player spawned at {spawnPosition}");
    }
    
    public int GetMaxDepth()
    {
        if (rooms.Count == 0) return 0;
        
        int maxDepth = 0;
        foreach (Room room in rooms)
        {
            if (room.depth > maxDepth)
            {
                maxDepth = room.depth;
            }
        }
        return maxDepth;
    }
    
    private void OnValidate()
    {
        numberOfRooms = Mathf.Max(1, numberOfRooms);
        dungeonWidth = Mathf.Max(10, dungeonWidth);
        dungeonHeight = Mathf.Max(10, dungeonHeight);
    }
}

[System.Serializable]
public class ChunkPlacement
{
    public GameObject chunk;
    public Room room;
    public Vector3 position;
}
