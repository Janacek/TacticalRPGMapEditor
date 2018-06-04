using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

public class TerrainGenerator : MonoBehaviour
{
    public Vector2 TerrainSize;

    static void drawString(string text, Vector3 worldPos, Color? colour = null)
    {
        UnityEditor.Handles.BeginGUI();
        if (colour.HasValue) GUI.color = colour.Value;
        var view = UnityEditor.SceneView.currentDrawingSceneView;
        Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);
        Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
        GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text);
        UnityEditor.Handles.EndGUI();
    }

    int MaxROffset = 0;

    float t = 1.736f;
    float t2 = 1.525f;

    public void GenerateMap()
    {
        GameObject tileGameObject = Resources.Load("Terrain/GrassTile") as GameObject;

        Debug.Log("Storage size : r:" + TerrainSize.y + ", q : " + (TerrainSize.x + Mathf.Floor(TerrainSize.y / 2)));

        int maxROffset = ((int)TerrainSize.y >> 1) - 1;
        m_terrain = new Tile[(int)TerrainSize.y, (int)TerrainSize.x + maxROffset];
        MaxROffset = maxROffset;

        Vector2 basePos = new Vector2(0, 0);

        for (int r = 0; r < (int)TerrainSize.y; ++r)
        {
            int r_offset = r >> 1;

            for (int q = -maxROffset; q < (int)TerrainSize.x; ++q)
            {
                if (q < -r_offset || q >= (int)TerrainSize.x - r_offset)
                {
                    m_terrain[r, q + maxROffset] = null;
                }
                else
                {
                    Vector3 pos = new Vector3(basePos.x * t + (basePos.y % 2 == 0 ? 0 : 0.85f), 0, -basePos.y * t2);

                    GameObject tile = GameObject.Instantiate(tileGameObject, pos, Quaternion.identity) as GameObject;
                    m_terrain[r, q + maxROffset] = tile.GetComponent<Tile>();
                    tile.GetComponent<Tile>().Coordinates = new Vector2(r, q);
                    tile.GetComponent<Tile>().ArrayCoordinates = new Vector2(r, q + maxROffset);
                    tile.name = string.Format("{0} {1} Tile", q, r);
                    tile.transform.parent = transform;

                    ++basePos.x;
                }

            }

            basePos.x = 0;
            ++basePos.y;
        }






        for (int r = 0; r < (int)TerrainSize.y; ++r)
        {
            int r_offset = r >> 1;

            for (int q = 0; q < (int)TerrainSize.x + maxROffset; ++q)
            {
                if (m_terrain[r, q])
                    SetNeighbours(m_terrain[r, q].GetComponent<Tile>());
            }

        }


        //for (int x = 0; x < (int)TerrainSize.x; ++x)
        //{
        //    for (int y = 0; y < (int)TerrainSize.y; ++y)
        //    {
        //        SetNeighbours(m_terrain[x,y].GetComponent<Tile>());
        //    }
        //}

    }

    Vector2[] tilesHelper =
    {
        new Vector2(0, -1),
        new Vector2(1, -1),
        new Vector2(1, 0),
        new Vector2(0, 1),
        new Vector2(-1, +1),
        new Vector2(-1, 0)
    };

    void SetNeighbours(Tile tile)
    {
        for (int i = 0; i < tilesHelper.Length; ++i)
        {
            int x, y;

            x = (int)tile.ArrayCoordinates.x + (int)tilesHelper[i].x;
            y = (int)tile.ArrayCoordinates.y + (int)tilesHelper[i].y;


            if (x >= 0 && x < TerrainSize.x && y >= 0 && y < TerrainSize.y + MaxROffset)
            {
                if (m_terrain[x, y])
                    tile.Neighbours.Add(m_terrain[x, y].GetComponent<Tile>());
            }
        }
    }

    public void DestroyMap()
    {
        //for (int r = 0; r < (int)TerrainSize.y; ++r)
        //{
        //    int r_offset = r >> 1;

        //    for (int q = 0; q < (int)TerrainSize.x + MaxROffset; ++q)
        //    {
        //        if (m_terrain[r, q])
        //        {
        //            DestroyImmediate(m_terrain[r, q].gameObject);
        //        }
        //    }

        //}

        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        m_terrain = null;
    }

    public void LoadObjects(ref List<GameObject> obj, string objectToLoad)
    {
        obj.Clear();
        Object[] objInstances = Resources.LoadAll(objectToLoad);

        for (int i = 0; i < objInstances.Length; ++i)
        {
            obj.Add((GameObject)objInstances[i]);
        }

        obj.ForEach(o =>
        {
            Debug.Log(o.name);
        });
    }

    void Update()
    {

    }

    [HideInInspector]
    public Tile[,] m_terrain = null;

    [HideInInspector]
    public List<GameObject> Tiles;

    [HideInInspector]
    public List<GameObject> SpawnFlags;

    public static GameObject SelectedObject = null;
    public static selectionType TypeOfSelection = selectionType.none;

    [HideInInspector]
    public List<GameObject> SpawnFlagsOnMap = new List<GameObject>();

    public enum selectionType
    {
        none = 0,
        tileSelection,
        spawnFlagSelection,
    }
}
