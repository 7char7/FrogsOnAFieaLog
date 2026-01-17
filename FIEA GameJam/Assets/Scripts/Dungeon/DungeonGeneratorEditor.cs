using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DungeonVisualizer))]
public class DungeonGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DungeonVisualizer visualizer = (DungeonVisualizer)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Generate New Dungeon", GUILayout.Height(40)))
        {
            visualizer.GenerateAndVisualize();
        }
    }
}
