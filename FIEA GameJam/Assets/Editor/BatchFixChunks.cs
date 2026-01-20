using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BatchFixChunks : EditorWindow
{
    private ChunkLibrary chunkLibrary;
    private float wallHeight = 3f;
    private bool addCeiling = true;
    
    [MenuItem("Tools/Dungeon/Batch Fix All Chunks")]
    public static void ShowWindow()
    {
        GetWindow<BatchFixChunks>("Batch Fix Chunks");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Batch Fix All Chunks", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox(
            "This will automatically add walls and ceiling to ALL chunks in your library.",
            MessageType.Info
        );
        
        EditorGUILayout.Space();
        
        chunkLibrary = (ChunkLibrary)EditorGUILayout.ObjectField(
            "Chunk Library", 
            chunkLibrary, 
            typeof(ChunkLibrary), 
            false
        );
        
        wallHeight = EditorGUILayout.FloatField("Wall Height", wallHeight);
        addCeiling = EditorGUILayout.Toggle("Add Ceiling", addCeiling);
        
        EditorGUILayout.Space();
        
        GUI.enabled = chunkLibrary != null;
        if (GUILayout.Button("Fix All Chunks", GUILayout.Height(40)))
        {
            FixAllChunks();
        }
        GUI.enabled = true;
    }
    
    private void FixAllChunks()
    {
        int fixedCount = 0;
        List<GameObject> allChunks = new List<GameObject>();
        
        SerializedObject serializedLibrary = new SerializedObject(chunkLibrary);
        
        AddChunksFromList(serializedLibrary, "standardRooms", allChunks);
        AddChunksFromList(serializedLibrary, "startRooms", allChunks);
        AddChunksFromList(serializedLibrary, "bossRooms", allChunks);
        AddChunksFromList(serializedLibrary, "treasureRooms", allChunks);
        AddChunksFromList(serializedLibrary, "straightCorridors", allChunks);
        
        foreach (GameObject chunkPrefab in allChunks)
        {
            if (chunkPrefab == null) continue;
            
            if (FixChunk(chunkPrefab))
            {
                fixedCount++;
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"<color=green>✓ Fixed {fixedCount} chunks!</color>");
        EditorUtility.DisplayDialog("Success", $"Fixed {fixedCount} chunks with walls and ceiling!", "OK");
    }
    
    private void AddChunksFromList(SerializedObject serializedLibrary, string fieldName, List<GameObject> allChunks)
    {
        SerializedProperty listProp = serializedLibrary.FindProperty(fieldName);
        if (listProp == null || !listProp.isArray) return;
        
        for (int i = 0; i < listProp.arraySize; i++)
        {
            GameObject prefab = listProp.GetArrayElementAtIndex(i).objectReferenceValue as GameObject;
            if (prefab != null && !allChunks.Contains(prefab))
            {
                allChunks.Add(prefab);
            }
        }
    }
    
    private bool FixChunk(GameObject chunkPrefab)
    {
        string prefabPath = AssetDatabase.GetAssetPath(chunkPrefab);
        
        if (string.IsNullOrEmpty(prefabPath))
        {
            Debug.LogWarning($"Skipping {chunkPrefab.name} - not a prefab");
            return false;
        }
        
        GameObject prefabInstance = PrefabUtility.LoadPrefabContents(prefabPath);
        
        DungeonChunk chunkComponent = prefabInstance.GetComponent<DungeonChunk>();
        if (chunkComponent == null)
        {
            Debug.LogWarning($"Skipping {chunkPrefab.name} - no DungeonChunk component");
            PrefabUtility.UnloadPrefabContents(prefabInstance);
            return false;
        }
        
        Vector2Int chunkSize = chunkComponent.ChunkSize;
        
        Transform geometryParent = prefabInstance.transform.Find("Geometry");
        if (geometryParent == null)
        {
            GameObject geometryObj = new GameObject("Geometry");
            geometryObj.transform.SetParent(prefabInstance.transform);
            geometryObj.transform.localPosition = Vector3.zero;
            geometryParent = geometryObj.transform;
        }
        
        CreateWalls(geometryParent, chunkSize);
        
        if (addCeiling)
        {
            CreateCeiling(geometryParent, chunkSize);
        }
        
        PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
        PrefabUtility.UnloadPrefabContents(prefabInstance);
        
        Debug.Log($"Fixed: {chunkPrefab.name} (Size: {chunkSize})");
        return true;
    }
    
    private void CreateWalls(Transform parent, Vector2Int chunkSize)
    {
        float halfWidth = chunkSize.x / 2f;
        float halfDepth = chunkSize.y / 2f;
        
        CreateWall(parent, "Wall_North", new Vector3(0, wallHeight / 2f, halfDepth), new Vector3(chunkSize.x, wallHeight, 0.2f));
        CreateWall(parent, "Wall_South", new Vector3(0, wallHeight / 2f, -halfDepth), new Vector3(chunkSize.x, wallHeight, 0.2f));
        CreateWall(parent, "Wall_East", new Vector3(halfWidth, wallHeight / 2f, 0), new Vector3(0.2f, wallHeight, chunkSize.y));
        CreateWall(parent, "Wall_West", new Vector3(-halfWidth, wallHeight / 2f, 0), new Vector3(0.2f, wallHeight, chunkSize.y));
    }
    
    private void CreateWall(Transform parent, string name, Vector3 position, Vector3 scale)
    {
        Transform existingWall = parent.Find(name);
        if (existingWall != null)
        {
            DestroyImmediate(existingWall.gameObject);
        }
        
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.SetParent(parent);
        wall.transform.localPosition = position;
        wall.transform.localScale = scale;
        
        if (LayerMask.NameToLayer("Wall") >= 0)
        {
            wall.layer = LayerMask.NameToLayer("Wall");
        }
        
        wall.tag = "Wall";
    }
    
    private void CreateCeiling(Transform parent, Vector2Int chunkSize)
    {
        Transform existingCeiling = parent.Find("Ceiling");
        if (existingCeiling != null)
        {
            DestroyImmediate(existingCeiling.gameObject);
        }
        
        GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ceiling.name = "Ceiling";
        ceiling.transform.SetParent(parent);
        ceiling.transform.localPosition = new Vector3(0, wallHeight, 0);
        ceiling.transform.localScale = new Vector3(chunkSize.x, 0.2f, chunkSize.y);
        
        DestroyImmediate(ceiling.GetComponent<Collider>());
    }
}
