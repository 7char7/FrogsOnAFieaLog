using UnityEngine;

public class DungeonExample : MonoBehaviour
{
    [SerializeField] private DungeonGenerator dungeonGenerator;
    [SerializeField] private DungeonVisualizer dungeonVisualizer;

    private void Start()
    {
        if (dungeonGenerator == null)
        {
            dungeonGenerator = GetComponent<DungeonGenerator>();
        }

        if (dungeonVisualizer == null)
        {
            dungeonVisualizer = GetComponent<DungeonVisualizer>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateNewDungeon();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            PrintRoomInfo();
        }
    }

    public void GenerateNewDungeon()
    {
        if (dungeonVisualizer != null)
        {
            dungeonVisualizer.GenerateAndVisualize();
            Debug.Log("New dungeon generated! Press 'R' to see room info.");
        }
    }

    public void PrintRoomInfo()
    {
        if (dungeonGenerator == null || dungeonGenerator.Rooms == null)
        {
            Debug.LogWarning("No dungeon generated yet!");
            return;
        }

        Debug.Log($"--- Dungeon Info ---");
        Debug.Log($"Total Rooms: {dungeonGenerator.Rooms.Count}");
        Debug.Log($"Total Corridors: {dungeonGenerator.Corridors.Count}");

        for (int i = 0; i < dungeonGenerator.Rooms.Count; i++)
        {
            Room room = dungeonGenerator.Rooms[i];
            Debug.Log($"Room {i}: Position={room.position}, Size={room.size}, Center={room.Center}");
        }
    }

    public Room GetRoomAtPosition(Vector2Int worldPosition)
    {
        if (dungeonGenerator == null || dungeonGenerator.Rooms == null)
        {
            return null;
        }

        foreach (Room room in dungeonGenerator.Rooms)
        {
            if (room.Contains(worldPosition))
            {
                return room;
            }
        }

        return null;
    }

    public Vector3 GetRandomRoomCenter()
    {
        if (dungeonGenerator == null || dungeonGenerator.Rooms == null || dungeonGenerator.Rooms.Count == 0)
        {
            return Vector3.zero;
        }

        Room randomRoom = dungeonGenerator.Rooms[Random.Range(0, dungeonGenerator.Rooms.Count)];
        return new Vector3(randomRoom.Center.x, 0, randomRoom.Center.y);
    }

    public Vector3 GetFirstRoomCenter()
    {
        if (dungeonGenerator == null || dungeonGenerator.Rooms == null || dungeonGenerator.Rooms.Count == 0)
        {
            return Vector3.zero;
        }

        Room firstRoom = dungeonGenerator.Rooms[0];
        return new Vector3(firstRoom.Center.x, 0, firstRoom.Center.y);
    }
}
