using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Tile))]
public class CustomTileInspector : Editor
{
    Tile t;

    void OnEnable()
    {
        t = (Tile)target;
    }

    public override void OnInspectorGUI()
    {
        //GUILayout.Space(20);

        //GUILayout.Label(t.name);
        //GUILayout.Space(20);

        //GUILayout.BeginHorizontal();

        //t.NormalMat = (Material)EditorGUILayout.ObjectField("Normal Mat", t.NormalMat, typeof(Material), true);
        //t.HoverMat = (Material)EditorGUILayout.ObjectField("Hover Mat", t.HoverMat, typeof(Material), true);

        //GUILayout.EndHorizontal();

        //GUILayout.Space(100);
        base.OnInspectorGUI();

        GUILayout.Space(30);

        GUILayout.BeginVertical("Box");

        GUILayout.Label("Tile Specs :");
        GUILayout.Space(20);

        GUILayout.BeginVertical("Box");
        t.Specs.Walkable = GUILayout.Toggle(t.Specs.Walkable, "    Walkable", GUILayout.MinWidth(150));

        if (t.Specs.Walkable)
        {
            GUILayout.BeginHorizontal();
            t.Specs.WalkMalus = EditorGUILayout.IntSlider(t.Specs.WalkMalus, 0, 50);
            GUILayout.Label("Walk Malus");
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();

        GUILayout.EndVertical();
    }
}
