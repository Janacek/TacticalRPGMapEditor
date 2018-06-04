using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameInfos))]
public class CustomGameInfosInspector : Editor
{
    GameInfos i;

    void OnEnable()
    {
        i = (GameInfos)target;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Space(20);

        GUILayout.BeginVertical("Box");

        GUILayout.Label("Game Informations :");

        if (i.MapToLoadS.Length == 0)
        {
            i.MapToLoad = (GameObject)EditorGUILayout.ObjectField("Map To Load", i.MapToLoad, typeof(GameObject), true);
        }

        if (i.MapToLoad == null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Map To Load(from name)");
            i.MapToLoadS = GUILayout.TextField(i.MapToLoadS);
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10);

        i.EntitiesPlacement = (GameInfos.TypeOfEntitiesPlacement)EditorGUILayout.EnumPopup("Entities Placement", i.EntitiesPlacement);

        GUILayout.Space(10);

        i.ControllerStarter = (GameInfos.ControllerStart)EditorGUILayout.EnumPopup("Controller Start", i.ControllerStarter);

        GUILayout.EndVertical();

        EditorUtility.SetDirty(i);
        Undo.RecordObject(i, "d");

        //GUILayout.Space(30);
        //base.OnInspectorGUI();
    }
}
