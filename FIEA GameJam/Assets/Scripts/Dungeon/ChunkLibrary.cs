using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChunkLibrary", menuName = "Dungeon/Chunk Library")]
public class ChunkLibrary : ScriptableObject
{
    [Header("Room Chunks")]
    [SerializeField] private List<GameObject> standardRooms = new List<GameObject>();
    [SerializeField] private List<GameObject> startRooms = new List<GameObject>();
    [SerializeField] private List<GameObject> bossRooms = new List<GameObject>();
    [SerializeField] private List<GameObject> treasureRooms = new List<GameObject>();
    
    [Header("Corridor Chunks")]
    [SerializeField] private List<GameObject> straightCorridors = new List<GameObject>();
    [SerializeField] private List<GameObject> cornerCorridors = new List<GameObject>();
    [SerializeField] private List<GameObject> tJunctionCorridors = new List<GameObject>();
    [SerializeField] private List<GameObject> crossJunctionCorridors = new List<GameObject>();
    
    [Header("Depth Zone Materials (Optional)")]
    [SerializeField] private Material[] baseZoneMaterials;
    [SerializeField] private Material[] blueZoneMaterials;
    [SerializeField] private Material[] redZoneMaterials;
    
    public GameObject GetRandomStandardRoom()
    {
        return GetRandomFromList(standardRooms);
    }
    
    public GameObject GetRandomStartRoom()
    {
        return GetRandomFromList(startRooms);
    }
    
    public GameObject GetRandomBossRoom()
    {
        return GetRandomFromList(bossRooms);
    }
    
    public GameObject GetRandomTreasureRoom()
    {
        return GetRandomFromList(treasureRooms);
    }
    
    public GameObject GetRandomStraightCorridor()
    {
        return GetRandomFromList(straightCorridors);
    }
    
    public GameObject GetRandomCornerCorridor()
    {
        return GetRandomFromList(cornerCorridors);
    }
    
    public GameObject GetRandomTJunction()
    {
        return GetRandomFromList(tJunctionCorridors);
    }
    
    public GameObject GetRandomCrossJunction()
    {
        return GetRandomFromList(crossJunctionCorridors);
    }
    
    public GameObject GetCorridorForConnectionType(int connectionCount, bool[] directions)
    {
        switch (connectionCount)
        {
            case 2:
                bool isStraight = (directions[0] && directions[2]) || (directions[1] && directions[3]);
                return isStraight ? GetRandomStraightCorridor() : GetRandomCornerCorridor();
            
            case 3:
                return GetRandomTJunction();
            
            case 4:
                return GetRandomCrossJunction();
            
            default:
                return GetRandomStraightCorridor();
        }
    }
    
    public Material[] GetMaterialsForDepth(int depth, int maxDepth)
    {
        if (depth <= maxDepth / 3 && baseZoneMaterials != null && baseZoneMaterials.Length > 0)
        {
            return baseZoneMaterials;
        }
        else if (depth <= (maxDepth * 2) / 3 && blueZoneMaterials != null && blueZoneMaterials.Length > 0)
        {
            return blueZoneMaterials;
        }
        else if (redZoneMaterials != null && redZoneMaterials.Length > 0)
        {
            return redZoneMaterials;
        }
        
        return null;
    }
    
    private GameObject GetRandomFromList(List<GameObject> list)
    {
        if (list == null || list.Count == 0)
        {
            Debug.LogWarning($"Chunk library list is empty or null!");
            return null;
        }
        
        return list[Random.Range(0, list.Count)];
    }
    
    public bool HasStandardRooms => standardRooms != null && standardRooms.Count > 0;
    public bool HasStartRooms => startRooms != null && startRooms.Count > 0;
    public bool HasCorridors => straightCorridors != null && straightCorridors.Count > 0;
}
