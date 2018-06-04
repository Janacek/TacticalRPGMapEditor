using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGenerator))]
public class CustomTerrainManagerInspector : Editor
{
    TerrainGenerator terrain;

    void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Label("Use the TerrainGenerator window to edit the map");
    }
}
