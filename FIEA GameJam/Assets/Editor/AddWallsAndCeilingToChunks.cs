using UnityEngine;
using UnityEditor;

public class AddWallsAndCeilingToChunks : EditorWindow
{
    private GameObject chunkPrefab;
    private float wallHeight = 3f;
    private bool addCeiling = true;
    private Material wallMaterial;
    private Material ceilingMaterial;
    
    [MenuItem("Tools/Dungeon/Add Walls & Ceiling to Chunk")]
    public static void ShowWindow()
    {
        GetWindow<AddWallsAndCeilingToChunks>("Add Walls & Ceiling");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Add Walls & Ceiling to Chunk", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        chunkPrefab = (GameObject)EditorGUILayout.ObjectField(
            "Chunk Prefab", 
            chunkPrefab, 
            typeof(GameObject), 
            false
        );
        
        wallHeight = EditorGUILayout.FloatField("Wall Height", wallHeight);
        addCeiling = EditorGUILayout.Toggle("Add Ceiling", addCeiling);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Materials (Optional)", EditorStyles.boldLabel);
        wallMaterial = (Material)EditorGUILayout.ObjectField(
            "Wall Material", 
            wallMaterial, 
            typeof(Material), 
            false
        );
        
        if (addCeiling)
        {
            ceilingMaterial = (Material)EditorGUILayout.ObjectField(
                "Ceiling Material", 
                ceilingMaterial, 
                typeof(Material), 
                false
            );
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "This will add wall and ceiling geometry to the chunk prefab based on its size.\n\n" +
            "Make sure the chunk has a DungeonChunk component with correct size set!",
            MessageType.Info
        );
        
        EditorGUILayout.Space();
        
        GUI.enabled = chunkPrefab != null;
        if (GUILayout.Button("Add Walls & Ceiling", GUILayout.Height(30)))
        {
            AddGeometry();
        }
        GUI.enabled = true;
    }
    
    private void AddGeometry()
    {
        string prefabPath = AssetDatabase.GetAssetPath(chunkPrefab);
        
        if (string.IsNullOrEmpty(prefabPath))
        {
            Debug.LogError("Selected object is not a prefab!");
            return;
        }
        
        GameObject prefabInstance = PrefabUtility.LoadPrefabContents(prefabPath);
        
        DungeonChunk chunkComponent = prefabInstance.GetComponent<DungeonChunk>();
        if (chunkComponent == null)
        {
            Debug.LogError("Chunk prefab doesn't have DungeonChunk component!");
            PrefabUtility.UnloadPrefabContents(prefabInstance);
            return;
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
        
        Debug.Log($"Added walls and ceiling to {chunkPrefab.name}!");
    }
    
    private void CreateWalls(Transform parent, Vector2Int chunkSize)
    {
        float halfWidth = chunkSize.x / 2f;
        float halfDepth = chunkSize.y / 2f;
        
        CreateWall(parent, "Wall_North", new Vector3(0, wallHeight / 2f, halfDepth), new Vector3(chunkSize.x, wallHeight, 0.1f), Quaternion.identity);
        CreateWall(parent, "Wall_South", new Vector3(0, wallHeight / 2f, -halfDepth), new Vector3(chunkSize.x, wallHeight, 0.1f), Quaternion.identity);
        CreateWall(parent, "Wall_East", new Vector3(halfWidth, wallHeight / 2f, 0), new Vector3(0.1f, wallHeight, chunkSize.y), Quaternion.identity);
        CreateWall(parent, "Wall_West", new Vector3(-halfWidth, wallHeight / 2f, 0), new Vector3(0.1f, wallHeight, chunkSize.y), Quaternion.identity);
    }
    
    private void CreateWall(Transform parent, string name, Vector3 position, Vector3 scale, Quaternion rotation)
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
        wall.transform.localRotation = rotation;
        wall.transform.localScale = scale;
        
        if (wallMaterial != null)
        {
            wall.GetComponent<MeshRenderer>().material = wallMaterial;
        }
        
        wall.layer = LayerMask.NameToLayer("Wall");
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
        ceiling.transform.localScale = new Vector3(chunkSize.x, 0.1f, chunkSize.y);
        
        if (ceilingMaterial != null)
        {
            ceiling.GetComponent<MeshRenderer>().material = ceilingMaterial;
        }
        
        DestroyImmediate(ceiling.GetComponent<Collider>());
    }
}
