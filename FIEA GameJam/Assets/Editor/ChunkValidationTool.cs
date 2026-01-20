using UnityEngine;
using UnityEditor;

public class ChunkValidationTool : EditorWindow
{
    private ChunkLibrary libraryToValidate;
    private Vector2 scrollPosition;
    
    [MenuItem("Tools/Dungeon/Validate Chunk Library")]
    public static void ShowWindow()
    {
        GetWindow<ChunkValidationTool>("Chunk Validator");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Chunk Library Validator", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        libraryToValidate = (ChunkLibrary)EditorGUILayout.ObjectField(
            "Chunk Library", 
            libraryToValidate, 
            typeof(ChunkLibrary), 
            false
        );
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Validate Library", GUILayout.Height(30)))
        {
            if (libraryToValidate != null)
            {
                ValidateChunkLibrary(libraryToValidate);
            }
            else
            {
                Debug.LogWarning("Please assign a ChunkLibrary to validate!");
            }
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "This tool checks your chunk library for common issues:\n" +
            "• Missing DungeonChunk components\n" +
            "• Incorrect chunk sizes\n" +
            "• Missing connection points\n" +
            "• Empty prefab slots",
            MessageType.Info
        );
    }
    
    private void ValidateChunkLibrary(ChunkLibrary library)
    {
        Debug.Log("=== CHUNK LIBRARY VALIDATION STARTED ===");
        
        int totalIssues = 0;
        int totalPrefabs = 0;
        
        totalIssues += ValidatePrefabList(library, "standardRooms", "Standard Rooms", ref totalPrefabs);
        totalIssues += ValidatePrefabList(library, "startRooms", "Start Rooms", ref totalPrefabs);
        totalIssues += ValidatePrefabList(library, "bossRooms", "Boss Rooms", ref totalPrefabs);
        totalIssues += ValidatePrefabList(library, "treasureRooms", "Treasure Rooms", ref totalPrefabs);
        totalIssues += ValidatePrefabList(library, "straightCorridors", "Straight Corridors", ref totalPrefabs);
        totalIssues += ValidatePrefabList(library, "cornerCorridors", "Corner Corridors", ref totalPrefabs);
        totalIssues += ValidatePrefabList(library, "tJunctionCorridors", "T-Junction Corridors", ref totalPrefabs);
        totalIssues += ValidatePrefabList(library, "crossJunctionCorridors", "Cross Junction Corridors", ref totalPrefabs);
        
        if (totalIssues == 0)
        {
            Debug.Log($"<color=green>✓ VALIDATION PASSED!</color> All {totalPrefabs} prefabs are properly configured.");
        }
        else
        {
            Debug.LogWarning($"<color=yellow>⚠ VALIDATION COMPLETED</color> - Found {totalIssues} issues across {totalPrefabs} prefabs. Check console for details.");
        }
        
        Debug.Log("=== CHUNK LIBRARY VALIDATION FINISHED ===");
    }
    
    private int ValidatePrefabList(ChunkLibrary library, string fieldName, string displayName, ref int totalPrefabs)
    {
        SerializedObject serializedLibrary = new SerializedObject(library);
        SerializedProperty listProp = serializedLibrary.FindProperty(fieldName);
        
        if (listProp == null || !listProp.isArray)
        {
            return 0;
        }
        
        int issueCount = 0;
        int prefabCount = listProp.arraySize;
        
        if (prefabCount == 0)
        {
            Debug.Log($"<color=gray>ℹ {displayName}: Empty (0 prefabs)</color>");
            return 0;
        }
        
        Debug.Log($"--- Validating {displayName} ({prefabCount} prefabs) ---");
        
        for (int i = 0; i < prefabCount; i++)
        {
            SerializedProperty element = listProp.GetArrayElementAtIndex(i);
            GameObject prefab = element.objectReferenceValue as GameObject;
            
            if (prefab == null)
            {
                Debug.LogWarning($"  ✗ [{i}] NULL prefab reference");
                issueCount++;
                continue;
            }
            
            totalPrefabs++;
            
            DungeonChunk chunkComponent = prefab.GetComponent<DungeonChunk>();
            
            if (chunkComponent == null)
            {
                Debug.LogError($"  ✗ [{i}] '{prefab.name}' - Missing DungeonChunk component!");
                issueCount++;
                continue;
            }
            
            int componentIssues = 0;
            
            if (chunkComponent.ChunkSize.x <= 0 || chunkComponent.ChunkSize.y <= 0)
            {
                Debug.LogWarning($"  ✗ [{i}] '{prefab.name}' - Invalid chunk size: {chunkComponent.ChunkSize}");
                componentIssues++;
            }
            
            bool hasAnyConnections = false;
            hasAnyConnections |= CheckConnectionArray(chunkComponent, Direction.North, prefab.name, ref componentIssues);
            hasAnyConnections |= CheckConnectionArray(chunkComponent, Direction.South, prefab.name, ref componentIssues);
            hasAnyConnections |= CheckConnectionArray(chunkComponent, Direction.East, prefab.name, ref componentIssues);
            hasAnyConnections |= CheckConnectionArray(chunkComponent, Direction.West, prefab.name, ref componentIssues);
            
            if (!hasAnyConnections && !fieldName.Contains("Room"))
            {
                Debug.LogWarning($"  ⚠ [{i}] '{prefab.name}' - No connection points defined (corridors should have connections)");
                componentIssues++;
            }
            
            if (componentIssues == 0)
            {
                Debug.Log($"  ✓ [{i}] '{prefab.name}' - OK (Size: {chunkComponent.ChunkSize})");
            }
            
            issueCount += componentIssues;
        }
        
        return issueCount;
    }
    
    private bool CheckConnectionArray(DungeonChunk chunk, Direction direction, string prefabName, ref int issueCount)
    {
        Transform[] connections = chunk.GetConnections(direction);
        
        if (connections == null || connections.Length == 0)
        {
            return false;
        }
        
        int nullCount = 0;
        foreach (Transform t in connections)
        {
            if (t == null) nullCount++;
        }
        
        if (nullCount > 0)
        {
            Debug.LogWarning($"    ⚠ '{prefabName}' - {direction} connections has {nullCount} null references");
            issueCount++;
        }
        
        return connections.Length > 0;
    }
}
