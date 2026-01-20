using UnityEngine;

public class DungeonChunk : MonoBehaviour
{
    [Header("Chunk Properties")]
    [SerializeField] private Vector2Int chunkSize = new Vector2Int(6, 6);
    [SerializeField] private ChunkType chunkType = ChunkType.Standard;
    [SerializeField] private int depthZone = 0;
    
    [Header("Connection Points")]
    [SerializeField] private Transform[] northConnections;
    [SerializeField] private Transform[] southConnections;
    [SerializeField] private Transform[] eastConnections;
    [SerializeField] private Transform[] westConnections;
    
    [Header("Spawn Points")]
    [SerializeField] private Transform[] enemySpawnPoints;
    [SerializeField] private Transform[] crystalSpawnPoints;
    [SerializeField] private Transform playerSpawnPoint;
    
    [Header("Visual Bounds (For Editor)")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private Color gizmoColor = new Color(0f, 1f, 0f, 0.3f);
    
    public Vector2Int ChunkSize => chunkSize;
    public ChunkType Type => chunkType;
    public int DepthZone { get => depthZone; set => depthZone = value; }
    
    public Transform[] GetConnections(Direction direction)
    {
        switch (direction)
        {
            case Direction.North: return northConnections;
            case Direction.South: return southConnections;
            case Direction.East: return eastConnections;
            case Direction.West: return westConnections;
            default: return null;
        }
    }
    
    public Transform[] EnemySpawnPoints => enemySpawnPoints;
    public Transform[] CrystalSpawnPoints => crystalSpawnPoints;
    public Transform PlayerSpawnPoint => playerSpawnPoint;
    
    public bool HasConnectionInDirection(Direction direction)
    {
        Transform[] connections = GetConnections(direction);
        return connections != null && connections.Length > 0;
    }
    
    public Vector3 GetConnectionPosition(Direction direction, int index = 0)
    {
        Transform[] connections = GetConnections(direction);
        if (connections != null && index < connections.Length)
        {
            return connections[index].position;
        }
        
        return GetDefaultConnectionPosition(direction);
    }
    
    private Vector3 GetDefaultConnectionPosition(Direction direction)
    {
        Vector3 center = transform.position;
        float halfWidth = chunkSize.x / 2f;
        float halfHeight = chunkSize.y / 2f;
        
        switch (direction)
        {
            case Direction.North: return center + new Vector3(0, 0, halfHeight);
            case Direction.South: return center + new Vector3(0, 0, -halfHeight);
            case Direction.East: return center + new Vector3(halfWidth, 0, 0);
            case Direction.West: return center + new Vector3(-halfWidth, 0, 0);
            default: return center;
        }
    }
    
    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
        
        Gizmos.color = gizmoColor;
        Vector3 center = transform.position;
        Vector3 size = new Vector3(chunkSize.x, 0.5f, chunkSize.y);
        Gizmos.DrawCube(center + Vector3.up * 0.25f, size);
        
        Gizmos.color = Color.yellow;
        DrawConnectionGizmos(northConnections, "N");
        DrawConnectionGizmos(southConnections, "S");
        DrawConnectionGizmos(eastConnections, "E");
        DrawConnectionGizmos(westConnections, "W");
        
        if (playerSpawnPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(playerSpawnPoint.position, 0.5f);
        }
    }
    
    private void DrawConnectionGizmos(Transform[] connections, string label)
    {
        if (connections == null) return;
        
        foreach (Transform connection in connections)
        {
            if (connection != null)
            {
                Gizmos.DrawWireSphere(connection.position, 0.3f);
#if UNITY_EDITOR
                UnityEditor.Handles.Label(connection.position, label);
#endif
            }
        }
    }
}

public enum ChunkType
{
    Standard,
    StartRoom,
    BossRoom,
    TreasureRoom,
    Hallway
}

public enum Direction
{
    North,
    South,
    East,
    West
}
