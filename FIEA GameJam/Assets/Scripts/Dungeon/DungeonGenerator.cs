using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Dungeon Settings")]
    [SerializeField] private int numberOfRooms = 10;
    [SerializeField] private Vector2Int minRoomSize = new Vector2Int(6, 6);
    [SerializeField] private Vector2Int maxRoomSize = new Vector2Int(12, 12);
    [SerializeField] private int maxPlacementAttempts = 100;
    [SerializeField] private int roomPadding = 2;
    [SerializeField] private int corridorWidth = 2;

    [Header("Generation Area")]
    [SerializeField] private int dungeonWidth = 100;
    [SerializeField] private int dungeonHeight = 100;

    [Header("Random Seed")]
    [SerializeField] private bool useRandomSeed = true;
    [SerializeField] private int seed = 0;

    private List<Room> rooms = new List<Room>();
    private List<Corridor> corridors = new List<Corridor>();
    private bool[,] dungeonGrid;
    private Dictionary<Room, List<Room>> roomConnections = new Dictionary<Room, List<Room>>();

    public List<Room> Rooms => rooms;
    public List<Corridor> Corridors => corridors;
    public bool[,] DungeonGrid => dungeonGrid;

    public void GenerateDungeon()
    {
        if (!useRandomSeed)
        {
            Random.InitState(seed);
        }

        ClearDungeon();
        GenerateRooms();
        ConnectRooms();
        CalculateRoomDepths();
        CreateDungeonGrid();
    }

    private void ClearDungeon()
    {
        rooms.Clear();
        corridors.Clear();
        roomConnections.Clear();
        dungeonGrid = new bool[dungeonWidth, dungeonHeight];
    }

    private void GenerateRooms()
    {
        int roomsPlaced = 0;
        int attempts = 0;

        while (roomsPlaced < numberOfRooms && attempts < maxPlacementAttempts * numberOfRooms)
        {
            attempts++;

            Vector2Int roomSize = new Vector2Int(
                Random.Range(minRoomSize.x, maxRoomSize.x + 1),
                Random.Range(minRoomSize.y, maxRoomSize.y + 1)
            );

            Vector2Int roomPosition = new Vector2Int(
                Random.Range(0, dungeonWidth - roomSize.x),
                Random.Range(0, dungeonHeight - roomSize.y)
            );

            Room newRoom = new Room(roomPosition, roomSize);

            if (!RoomOverlapsExisting(newRoom))
            {
                rooms.Add(newRoom);
                roomsPlaced++;
            }
        }

        Debug.Log($"Generated {roomsPlaced} rooms in {attempts} attempts");
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
                Vector2Int startPoint = closestConnected.Center;
                Vector2Int endPoint = closestUnconnected.Center;

                if (Random.value > 0.5f)
                {
                    CreateLShapedCorridor(startPoint, endPoint);
                }
                else
                {
                    CreateLShapedCorridor(startPoint, endPoint, true);
                }

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

        rooms = connected;
    }

    private void CreateLShapedCorridor(Vector2Int start, Vector2Int end, bool horizontalFirst = true)
    {
        if (horizontalFirst)
        {
            Vector2Int corner = new Vector2Int(end.x, start.y);
            corridors.Add(new Corridor(start, corner, corridorWidth));
            corridors.Add(new Corridor(corner, end, corridorWidth));
        }
        else
        {
            Vector2Int corner = new Vector2Int(start.x, end.y);
            corridors.Add(new Corridor(start, corner, corridorWidth));
            corridors.Add(new Corridor(corner, end, corridorWidth));
        }
    }

    private void CreateDungeonGrid()
    {
        foreach (Room room in rooms)
        {
            for (int x = room.Left; x < room.Right; x++)
            {
                for (int y = room.Bottom; y < room.Top; y++)
                {
                    if (x >= 0 && x < dungeonWidth && y >= 0 && y < dungeonHeight)
                    {
                        dungeonGrid[x, y] = true;
                    }
                }
            }
        }

        foreach (Corridor corridor in corridors)
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
                    if (x >= 0 && x < dungeonWidth && y >= 0 && y < dungeonHeight)
                    {
                        dungeonGrid[x, y] = true;
                    }
                }
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

        int maxDepth = GetMaxDepth();
        int grayCount = 0;
        int blueCount = 0;
        int redCount = 0;

        foreach (Room room in rooms)
        {
            if (room.depth <= maxDepth / 3)
                grayCount++;
            else if (room.depth <= (maxDepth * 2) / 3)
                blueCount++;
            else
                redCount++;
        }

        Debug.Log($"Depth calculation complete. Max depth: {maxDepth}, Zone distribution - Gray: {grayCount}, Blue: {blueCount}, Red: {redCount}");
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
        minRoomSize.x = Mathf.Max(3, minRoomSize.x);
        minRoomSize.y = Mathf.Max(3, minRoomSize.y);
        maxRoomSize.x = Mathf.Max(minRoomSize.x, maxRoomSize.x);
        maxRoomSize.y = Mathf.Max(minRoomSize.y, maxRoomSize.y);
        corridorWidth = Mathf.Max(1, corridorWidth);
        dungeonWidth = Mathf.Max(10, dungeonWidth);
        dungeonHeight = Mathf.Max(10, dungeonHeight);
    }
}
