using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CustomTerrainGeneratorWindow : EditorWindow
{

    [MenuItem("TerrainGenerator/TerrainGenerator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CustomTerrainGeneratorWindow));
    }

    TerrainGenerator terrain;

    void OnEnable()
    {
        //if (GameObject.FindGameObjectsWithTag("TerrainManager").Length > 1)
        //    return;

        //if (GameObject.FindGameObjectWithTag("TerrainManager"))
        //{
        //    terrain = GameObject.FindGameObjectWithTag("TerrainManager").GetComponent<TerrainGenerator>();
        //}
        //else
        //{
        //    GameObject newTerrain = Resources.Load("CustomWindowTerrain/Terrain") as GameObject;
        //    GameObject instance = GameObject.Instantiate(newTerrain, Vector3.zero, Quaternion.identity) as GameObject;

        //    instance.name = "Terrain";

        //    terrain = instance.GetComponent<TerrainGenerator>();
        //}
    }

    void OnGUI()
    {
        if (Application.isPlaying)
        {
            GUILayout.Label("Map Edition not available on Runtime");
            return;
        }

        if (GameObject.FindGameObjectsWithTag("TerrainManager").Length > 1)
        {
            GUILayout.Label("This does not support multi maps");
            return;
        }

        if (terrain == null)
        {
            //OnEnable();
            if (GameObject.FindGameObjectsWithTag("TerrainManager").Length >= 1)
            {
                terrain = GameObject.FindGameObjectWithTag("TerrainManager").GetComponent<TerrainGenerator>();
                return;
            }

            GUILayout.Space(50);
            GUILayout.BeginHorizontal("Box");

            if (GUILayout.Button("Create Map"))
            {
                CreateMap();
                //RefreshTiles();
                RefreshObjects("Terrain", ref terrain.Tiles, ref tiles3DPreviewEditors, ref tiles3DPreviewGenerated);
                RefreshObjects("SpawnFlags", ref terrain.SpawnFlags, ref spawnFlags3DPreviewEditors, ref spawnFlags3DPreviewGenerated);

                return;
            }

            if (GUILayout.Button("Load Map"))
            {
                LoadMap();
                if (terrain == null)
                    return;
                //terrain.LoadTiles();
                terrain.LoadObjects(ref terrain.Tiles, "Terrain");
                terrain.LoadObjects(ref terrain.SpawnFlags, "SpawnFlags");

                //RefreshTiles();
                RefreshObjects("Terrain", ref terrain.Tiles, ref tiles3DPreviewEditors, ref tiles3DPreviewGenerated);
                RefreshObjects("SpawnFlags", ref terrain.SpawnFlags, ref spawnFlags3DPreviewEditors, ref spawnFlags3DPreviewGenerated);

                //LoadUtils();
            }

            GUILayout.EndHorizontal();
            return;
        }

        topScrollPosition = GUILayout.BeginScrollView(topScrollPosition);
        GUILayout.Space(20);

        GUILayout.BeginHorizontal("Box");
        terrain.name = GUILayout.TextField(terrain.name);

        if (GUILayout.Button("Save Map"))
        {
            SaveMap();
        }

        if (GUILayout.Button("Unload Map"))
        {
            UnloadMap();
            return;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("Box");

        if (terrain.transform.childCount > 0)
        {
            GUI.enabled = false;
        }

        GUILayout.Label("Size X:");
        terrain.TerrainSize.x = (int)EditorGUILayout.IntField((int)terrain.TerrainSize.x);
        GUILayout.Label(" Y:");
        terrain.TerrainSize.y = (int)EditorGUILayout.IntField((int)terrain.TerrainSize.y);

        GUI.enabled = true;

        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        GUILayout.BeginVertical("Box");

        GUILayout.Label("Map Generation : ");

        GUILayout.BeginHorizontal();

        if (terrain.transform.childCount > 0)
        {
            GUI.enabled = false;
        }

        if (GUILayout.Button("Generate"))
        {
            GenerateTerrain();
        }

        GUI.enabled = true;

        Color back = GUI.color;
        GUI.color = Color.red;


        if (terrain.transform.childCount <= 0)
        {
            GUI.enabled = false;
        }

        if (GUILayout.Button("Destroy"))
        {
            DestroyTerrain();
        }
        GUI.color = back;
        GUI.enabled = true;

        GUILayout.EndHorizontal();

        // Regenerate all the tiles
        if (GUILayout.Button("Regenerate Tiles"))
        {
            RegenerateTiles();
        }

        GUILayout.EndVertical();

        GUILayout.Space(50);

        #region TILES MANAGEMENT

        ///////////////////////////
        GUILayout.BeginVertical("Box");

        GUILayout.BeginHorizontal();

        showTiles = GUILayout.Toggle(showTiles, "Tiles :");


        if (GUILayout.Button("Manually Refresh"))
        {
            //RefreshTiles();
            RefreshObjects("Terrain", ref terrain.Tiles, ref tiles3DPreviewEditors, ref tiles3DPreviewGenerated);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
        if (showTiles)
        {

            int jump = 0;

            tilesScrollPosition = GUILayout.BeginScrollView(tilesScrollPosition, GUILayout.MinHeight(150));

            if (Event.current.type != EventType.MouseDown)
            {
                mousePosition = new Vector2(
                    Event.current.mousePosition.x,
                    Event.current.mousePosition.y/* + scrollPosition.y*/ - 234.0f
                    );
            }

            EditorGUILayout.BeginHorizontal();

            for (int i = 0; i < terrain.Tiles.Count; ++i, ++jump)
            {
                if (i == currentTileSelection && TerrainGenerator.TypeOfSelection == TerrainGenerator.selectionType.tileSelection)
                {
                    GUI.backgroundColor = Color.blue;
                }
                else
                {
                    GUI.backgroundColor = Color.white;
                }

                Rect box = EditorGUILayout.BeginVertical("Box", GUILayout.Width(100));

                SetSelectedObject(box, i, terrain.Tiles, ref currentTileSelection, TerrainGenerator.selectionType.tileSelection);

                GUILayout.Label(terrain.Tiles[i].name);
                if (tiles3DPreviewGenerated == false)
                {
                    Editor gameObjectEditor = Editor.CreateEditor(terrain.Tiles[i]);
                    gameObjectEditor.OnPreviewGUI(GUILayoutUtility.GetRect(64, 64), EditorStyles.helpBox);
                    tiles3DPreviewEditors.Add(gameObjectEditor);
                }
                else
                {
                    tiles3DPreviewEditors[i].OnPreviewGUI(GUILayoutUtility.GetRect(64, 64), EditorStyles.helpBox);
                }

                GUILayout.EndVertical();

                if (jump == 2)
                {
                    jump = -1;
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }

            }

            tiles3DPreviewGenerated = true;


            EditorGUILayout.EndHorizontal();

            GUI.backgroundColor = Color.white;
            GUILayout.EndScrollView();
            GUI.backgroundColor = Color.white;
        }
        else
        {
            //currentTileSelection = -1;
            //TerrainGenerator.SelectedObject = null;
            //TerrainGenerator.TypeOfSelection = TerrainGenerator.selectionType.none;
        }

        GUILayout.EndVertical();
        //////////////////////////

        #endregion

        #region UTILS MANAGEMENT

        GUILayout.Space(20);

        // Vertical Box For Utils
        GUILayout.BeginVertical("Box");
        GUI.backgroundColor = Color.white;

        GUILayout.BeginHorizontal();
        showUtils = GUILayout.Toggle(showUtils, "SpawnFlags :");

        if (GUILayout.Button("Manually Refresh"))
        {
            RefreshObjects("SpawnFlags", ref terrain.SpawnFlags, ref spawnFlags3DPreviewEditors, ref spawnFlags3DPreviewGenerated);
        }

        GUILayout.EndHorizontal();

        if (showUtils)
        {

            int jump = 0;
            spawnFlagsScrollPosition = GUILayout.BeginScrollView(spawnFlagsScrollPosition, GUILayout.MinHeight(150));

            GUILayout.BeginHorizontal();
            for (int i = 0; i < terrain.SpawnFlags.Count; ++i)
            {
                if (i == currentSpawnFlagSelection && TerrainGenerator.TypeOfSelection == TerrainGenerator.selectionType.spawnFlagSelection)
                {
                    GUI.backgroundColor = Color.blue;
                }
                else
                {
                    GUI.backgroundColor = Color.white;
                }

                Rect box = EditorGUILayout.BeginVertical("Box", GUILayout.Width(100));


                SetSelectedObject(box, i, terrain.SpawnFlags, ref currentSpawnFlagSelection, TerrainGenerator.selectionType.spawnFlagSelection);

                GUILayout.Label(terrain.SpawnFlags[i].name);

                if (spawnFlags3DPreviewGenerated == false)
                {
                    Editor gameObjectEditor = Editor.CreateEditor(terrain.SpawnFlags[i]);
                    gameObjectEditor.OnPreviewGUI(GUILayoutUtility.GetRect(64, 64), EditorStyles.helpBox);
                    spawnFlags3DPreviewEditors.Add(gameObjectEditor);
                }
                else
                {
                    spawnFlags3DPreviewEditors[i].OnPreviewGUI(GUILayoutUtility.GetRect(64, 64), EditorStyles.helpBox);
                }

                GUILayout.EndVertical();

                if (jump == 2)
                {
                    jump = -1;
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }
            }

            spawnFlags3DPreviewGenerated = true;
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();

        }
        else
        {
            //currentSpawnFlagSelection = -1;
            //TerrainGenerator.SelectedObject = null;
            //TerrainGenerator.TypeOfSelection = TerrainGenerator.selectionType.none;
        }

        GUILayout.EndVertical();

        #endregion

        GUILayout.EndScrollView();


    }

    void SetSelectedObject(Rect box, int i, List<GameObject> objects, ref int currentSelection, TerrainGenerator.selectionType selectionType)
    {
        if (Event.current.type == EventType.MouseDown)
        {
            if (box.Contains(mousePosition))
            {
                if (i == currentSelection)
                {
                    currentSelection = -1;
                    TerrainGenerator.SelectedObject = null;
                    TerrainGenerator.TypeOfSelection = TerrainGenerator.selectionType.none;
                }
                else
                {
                    currentSelection = i;
                    TerrainGenerator.SelectedObject = objects[i];
                    TerrainGenerator.TypeOfSelection = selectionType;
                }
            }
        }
    }

    void SaveMap()
    {
        //AssetDatabase.CreateAsset(terrain, "Assets/Resources/Maps/" + terrain.name + ".asset");
        PrefabUtility.CreatePrefab("Assets/Resources/Maps/" + terrain.name + ".prefab", terrain.gameObject);
    }

    void UnloadMap()
    {
        SaveMap();
        GameObject.DestroyImmediate(terrain.gameObject);
        terrain = null;
    }

    void CreateMap()
    {
        GameObject newTerrain = Resources.Load("CustomWindowTerrain/Terrain") as GameObject;
        GameObject instance = GameObject.Instantiate(newTerrain, Vector3.zero, Quaternion.identity) as GameObject;

        instance.name = "Terrain";

        terrain = instance.GetComponent<TerrainGenerator>();
    }

    void LoadMap()
    {
        string file = EditorUtility.OpenFilePanel("Select a Map", "Assets/Resources/Maps", "prefab");
        if (file.Length <= 0)
        {
            return;
        }

        string[] path = file.Split('/');
        string[] fileName = path[path.Length - 1].Split('.');

        GameObject map = Resources.Load("Maps/" + fileName[0]) as GameObject;
        GameObject instance = GameObject.Instantiate(map, Vector3.zero, Quaternion.identity);
        instance.name = fileName[0];
        terrain = instance.GetComponent<TerrainGenerator>();
    }

    //void RefreshTiles()
    //{
    //    //terrain.LoadTiles();
    //    terrain.LoadObjects(ref terrain.Tiles, "Terrain");
    //    for (int i = 0; i < tiles3DPreviewEditors.Count; ++i)
    //    {
    //        Editor e = tiles3DPreviewEditors[i];
    //        Editor.DestroyImmediate(e);
    //    }

    //    tiles3DPreviewEditors.Clear();
    //    tiles3DPreviewGenerated = false;
    //}

    void RefreshObjects(string objectToRefresh, ref List<GameObject> obj, ref List<Editor> editorsList, ref bool listGenerated)
    {
        terrain.LoadObjects(ref obj, objectToRefresh);
        for (int i = 0; i < editorsList.Count; ++i)
        {
            Editor e = editorsList[i];
            Editor.DestroyImmediate(e);
        }

        editorsList.Clear();
        listGenerated = false;
    }

    //void RefreshSpawnFlags()
    //{
    //    for (int i = 0; i < spawnFlags3DPreviewEditors.Count; ++i)
    //    {
    //        Editor e = spawnFlags3DPreviewEditors[i];
    //        Editor.DestroyImmediate(e);
    //    }

    //    spawnFlags3DPreviewEditors.Clear();
    //    spawnFlags3DPreviewGenerated = false;
    //}

    void RegenerateTiles()
    {
        Debug.Log("Needs Dev");
    }

    void RegenerateSpawnFlags()
    {
        Debug.Log("Needs Dev");
    }

    void GenerateTerrain()
    {
        terrain.GenerateMap();
    }

    void DestroyTerrain()
    {
        if (EditorUtility.DisplayDialog("WAIT", "Are you sure you want to destroy the map ?", "Yes", "Cancel"))
            terrain.DestroyMap();
    }

    Vector2 topScrollPosition = new Vector2(0, 0);

    Vector2 mousePosition;

    #region TILES 3D PREVIEW
    Vector2 tilesScrollPosition = new Vector2(0, 0);
    int currentTileSelection = -1;
    List<Editor> tiles3DPreviewEditors = new List<Editor>();
    bool tiles3DPreviewGenerated = false;
    bool showTiles = true;
    #endregion

    #region SPAWN FLAGS 3D PREVIEW
    Vector2 spawnFlagsScrollPosition = new Vector2(0, 0);
    int currentSpawnFlagSelection = -1;
    List<Editor> spawnFlags3DPreviewEditors = new List<Editor>();
    bool spawnFlags3DPreviewGenerated = false;
    bool showUtils = true;
    #endregion

}

