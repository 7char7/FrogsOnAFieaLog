using UnityEngine;
using UnityEditor;

public class ChunkCreatorWizard : EditorWindow
{
    private Vector2Int chunkSize = new Vector2Int(6, 6);
    private ChunkType chunkType = ChunkType.Standard;
    private string chunkName = "RoomChunk";
    private bool createConnectionPoints = true;
    private bool createSpawnPoints = false;
    private int connectionsPerSide = 1;
    
    [MenuItem("Tools/Dungeon/Chunk Creator Wizard")]
    public static void ShowWindow()
    {
        GetWindow<ChunkCreatorWizard>("Chunk Creator");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Chunk Creator Wizard", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Chunk Properties", EditorStyles.boldLabel);
        chunkName = EditorGUILayout.TextField("Chunk Name", chunkName);
        chunkSize = EditorGUILayout.Vector2IntField("Chunk Size", chunkSize);
        chunkType = (ChunkType)EditorGUILayout.EnumPopup("Chunk Type", chunkType);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Connection Setup", EditorStyles.boldLabel);
        createConnectionPoints = EditorGUILayout.Toggle("Create Connection Points", createConnectionPoints);
        
        if (createConnectionPoints)
        {
            connectionsPerSide = EditorGUILayout.IntSlider("Connections Per Side", connectionsPerSide, 1, 3);
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Spawn Points", EditorStyles.boldLabel);
        createSpawnPoints = EditorGUILayout.Toggle("Create Spawn Points", createSpawnPoints);
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "This will create a GameObject with DungeonChunk component and connection points set up.\n\n" +
            "You can then add your room geometry as children and save it as a prefab.",
            MessageType.Info
        );
        
        EditorGUILayout.Space();
        if (GUILayout.Button("Create Chunk", GUILayout.Height(30)))
        {
            CreateChunk();
        }
    }
    
    private void CreateChunk()
    {
        GameObject chunkRoot = new GameObject($"{chunkName}_{chunkSize.x}x{chunkSize.y}");
        chunkRoot.transform.position = Vector3.zero;
        
        DungeonChunk dungeonChunk = chunkRoot.AddComponent<DungeonChunk>();
        
        SerializedObject serializedChunk = new SerializedObject(dungeonChunk);
        serializedChunk.FindProperty("chunkSize").vector2IntValue = chunkSize;
        serializedChunk.FindProperty("chunkType").enumValueIndex = (int)chunkType;
        
        if (createConnectionPoints)
        {
            CreateConnectionPointsForChunk(chunkRoot, serializedChunk);
        }
        
        if (createSpawnPoints)
        {
            CreateSpawnPointsForChunk(chunkRoot, serializedChunk);
        }
        
        CreateVisualGuideForChunk(chunkRoot);
        
        serializedChunk.ApplyModifiedProperties();
        
        Selection.activeGameObject = chunkRoot;
        EditorGUIUtility.PingObject(chunkRoot);
        
        Debug.Log($"Created chunk: {chunkRoot.name}. Add your room geometry as children, then save as prefab!");
    }
    
    private void CreateConnectionPointsForChunk(GameObject chunkRoot, SerializedObject serializedChunk)
    {
        GameObject connectionsParent = new GameObject("_ConnectionPoints");
        connectionsParent.transform.SetParent(chunkRoot.transform);
        connectionsParent.transform.localPosition = Vector3.zero;
        
        Transform[] northConnections = CreateConnectionsForDirection(connectionsParent, "North", Vector3.forward, chunkSize.y / 2f);
        Transform[] southConnections = CreateConnectionsForDirection(connectionsParent, "South", Vector3.back, chunkSize.y / 2f);
        Transform[] eastConnections = CreateConnectionsForDirection(connectionsParent, "East", Vector3.right, chunkSize.x / 2f);
        Transform[] westConnections = CreateConnectionsForDirection(connectionsParent, "West", Vector3.left, chunkSize.x / 2f);
        
        SetConnectionArray(serializedChunk, "northConnections", northConnections);
        SetConnectionArray(serializedChunk, "southConnections", southConnections);
        SetConnectionArray(serializedChunk, "eastConnections", eastConnections);
        SetConnectionArray(serializedChunk, "westConnections", westConnections);
    }
    
    private Transform[] CreateConnectionsForDirection(GameObject parent, string directionName, Vector3 direction, float distance)
    {
        Transform[] connections = new Transform[connectionsPerSide];
        
        for (int i = 0; i < connectionsPerSide; i++)
        {
            GameObject connectionPoint = new GameObject($"Connection_{directionName}_{i + 1:00}");
            connectionPoint.transform.SetParent(parent.transform);
            
            Vector3 position = direction * distance;
            
            if (connectionsPerSide > 1)
            {
                float spacing = Mathf.Min(chunkSize.x, chunkSize.y) / (connectionsPerSide + 1);
                Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;
                float offset = (i - (connectionsPerSide - 1) / 2f) * spacing;
                position += perpendicular * offset;
            }
            
            connectionPoint.transform.localPosition = position;
            connections[i] = connectionPoint.transform;
        }
        
        return connections;
    }
    
    private void CreateSpawnPointsForChunk(GameObject chunkRoot, SerializedObject serializedChunk)
    {
        GameObject spawnParent = new GameObject("_SpawnPoints");
        spawnParent.transform.SetParent(chunkRoot.transform);
        spawnParent.transform.localPosition = Vector3.zero;
        
        GameObject playerSpawn = new GameObject("PlayerSpawnPoint");
        playerSpawn.transform.SetParent(spawnParent.transform);
        playerSpawn.transform.localPosition = Vector3.up * 0.5f;
        
        serializedChunk.FindProperty("playerSpawnPoint").objectReferenceValue = playerSpawn.transform;
        
        int enemySpawnCount = 3;
        Transform[] enemySpawns = new Transform[enemySpawnCount];
        for (int i = 0; i < enemySpawnCount; i++)
        {
            GameObject enemySpawn = new GameObject($"EnemySpawnPoint_{i + 1:00}");
            enemySpawn.transform.SetParent(spawnParent.transform);
            
            float angle = (360f / enemySpawnCount) * i * Mathf.Deg2Rad;
            float radius = Mathf.Min(chunkSize.x, chunkSize.y) * 0.3f;
            Vector3 position = new Vector3(Mathf.Cos(angle) * radius, 0.5f, Mathf.Sin(angle) * radius);
            enemySpawn.transform.localPosition = position;
            
            enemySpawns[i] = enemySpawn.transform;
        }
        
        SetConnectionArray(serializedChunk, "enemySpawnPoints", enemySpawns);
    }
    
    private void CreateVisualGuideForChunk(GameObject chunkRoot)
    {
        GameObject visualGuide = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visualGuide.name = "_VisualGuide_DELETE_BEFORE_PREFAB";
        visualGuide.transform.SetParent(chunkRoot.transform);
        visualGuide.transform.localPosition = Vector3.zero;
        visualGuide.transform.localScale = new Vector3(chunkSize.x, 0.1f, chunkSize.y);
        
        Material guideMaterial = new Material(Shader.Find("Standard"));
        guideMaterial.color = new Color(0f, 1f, 0f, 0.3f);
        guideMaterial.SetFloat("_Mode", 3);
        guideMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        guideMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        guideMaterial.SetInt("_ZWrite", 0);
        guideMaterial.DisableKeyword("_ALPHATEST_ON");
        guideMaterial.EnableKeyword("_ALPHABLEND_ON");
        guideMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        guideMaterial.renderQueue = 3000;
        
        visualGuide.GetComponent<MeshRenderer>().material = guideMaterial;
        DestroyImmediate(visualGuide.GetComponent<Collider>());
    }
    
    private void SetConnectionArray(SerializedObject serializedObject, string propertyName, Transform[] transforms)
    {
        SerializedProperty arrayProp = serializedObject.FindProperty(propertyName);
        arrayProp.arraySize = transforms.Length;
        
        for (int i = 0; i < transforms.Length; i++)
        {
            arrayProp.GetArrayElementAtIndex(i).objectReferenceValue = transforms[i];
        }
    }
}
