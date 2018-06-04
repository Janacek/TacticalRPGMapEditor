using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class MouseManager : MonoBehaviour
{
    public LayerMask TileLayer;

    void Start()
    {

    }

#if UNITY_EDITOR

    void OnEnable()
    {
        if (Application.isPlaying == false)
        {
            EditorApplication.update += Update;
        }
    }

    void OnDisable()
    {
        if (Application.isPlaying == false)
        {
            EditorApplication.update -= Update;
        }
    }

#endif

    Vector3 mPos;

    void OnGUI()
    {
        if (Event.current.type != EventType.MouseDrag && Event.current.type != EventType.MouseUp)
        {
            mPos = Event.current.mousePosition;
            mPos.y = Camera.main.pixelHeight - mPos.y;
            mPos.z = 4000;
        }
        else if (Event.current.type == EventType.ScrollWheel)
        {
        }
        else
        {
            if (Event.current.button == 0 && m_hovered && TerrainGenerator.SelectedObject != null)
            {
                if (TerrainGenerator.TypeOfSelection == TerrainGenerator.selectionType.tileSelection)
                {
                    ChangeTile();
                }
                else if (TerrainGenerator.TypeOfSelection == TerrainGenerator.selectionType.spawnFlagSelection)
                {
                    HandleSpawnFlag();
                }
            }
        }
    }

    void HandleSpawnFlag()
    {
        GameObject g = TerrainGenerator.SelectedObject;
        SpawnFlagsInfos.TypeOfFlag type = g.GetComponent<SpawnFlagsInfos>().FlagType;

        if (type == SpawnFlagsInfos.TypeOfFlag.none)
        {
            Debug.Log("Deleting SpawnFlag");
            Vector2 pos = m_lastTouched.transform.parent.GetComponent<Tile>().Coordinates;
            GameObject found = null;
            m_lastTouched.transform.parent.parent.GetComponent<TerrainGenerator>().SpawnFlagsOnMap.ForEach(s =>
            {
                if (s.GetComponent<SpawnFlagsInfos>().Coordinates == pos)
                {
                    found = s;
                }
            });

            if (found)
            {
                m_lastTouched.transform.parent.parent.GetComponent<TerrainGenerator>().SpawnFlagsOnMap.Remove(found);
                DestroyImmediate(found);
            }
        }
        else
        {
            if (m_lastTouched.transform.parent.GetComponent<Tile>().Specs.Walkable == false)
            {
                Debug.LogError("Can't add a Spawn Flag on a non walkable tile.");
                return;
            }
            Debug.Log("Adding SpawnFlag");
            Vector2 pos = m_lastTouched.transform.parent.GetComponent<Tile>().Coordinates;  
            bool alreadyPlacedSpawn = false;
            m_lastTouched.transform.parent.parent.GetComponent<TerrainGenerator>().SpawnFlagsOnMap.ForEach(s =>
            {
                if (s.GetComponent<SpawnFlagsInfos>().Coordinates == pos)
                {
                    alreadyPlacedSpawn = true;
                }
            });

            if (alreadyPlacedSpawn)
                return;

            GameObject newSpawnFlag = GameObject.Instantiate(g, m_lastTouched.transform.position, g.transform.rotation) as GameObject;
            newSpawnFlag.transform.parent = m_lastTouched.transform.parent.parent;
            newSpawnFlag.GetComponent<SpawnFlagsInfos>().Coordinates = m_lastTouched.transform.parent.GetComponent<Tile>().Coordinates;
            newSpawnFlag.GetComponent<SpawnFlagsInfos>().ArrayCoordinates = m_lastTouched.transform.parent.GetComponent<Tile>().ArrayCoordinates;

            TerrainGenerator map = m_lastTouched.transform.parent.parent.GetComponent<TerrainGenerator>();
            map.SpawnFlagsOnMap.Add(newSpawnFlag);
        }
    }

    void ChangeTile()
    {
        GameObject instance = GameObject.Instantiate(TerrainGenerator.SelectedObject);
        instance.name = m_hovered.transform.parent.name;
        instance.transform.position = m_hovered.transform.parent.transform.position;

        m_hovered.transform.parent.GetComponent<Tile>().NormalMat = instance.GetComponent<Tile>().NormalMat;
        m_hovered.transform.parent.GetComponent<Tile>().HoverMat = instance.GetComponent<Tile>().HoverMat;

        m_hovered.transform.parent.GetComponent<Tile>().Specs = instance.GetComponent<Tile>().Specs;

        GameObject par = m_hovered.transform.parent.gameObject;

        DestroyImmediate(m_hovered.gameObject);
        instance.transform.GetChild(0).transform.parent = par.transform;

        DestroyImmediate(instance.gameObject);
    }

    public void Update()
    {
        Ray inputRay;

        if (Application.isPlaying)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        }
        else
        {
            inputRay = Camera.main.ScreenPointToRay(mPos);
        }

        RaycastHit hit;
        m_hovered = null;

        if (Physics.Raycast(inputRay, out hit, 100, TileLayer))
        {
            GameObject cell = hit.collider.gameObject;

            //if (cell.transform.parent.GetComponent<Tile>().Specs.Walkable == false)
            //    return;

            m_hovered = cell;

            if (m_lastTouched == cell)
            {
                EditorUtility.SetDirty(this);
                return;
            }

            if (m_lastTouched)
            {
                m_lastTouched.GetComponent<MeshRenderer>().material = m_lastTouched.transform.parent.GetComponent<Tile>().NormalMat;

                //m_lastTouched.GetComponent<MeshRenderer>().material.SetFloat("_Outline", 0);
            }

            cell.GetComponent<MeshRenderer>().material = cell.transform.parent.GetComponent<Tile>().HoverMat;
            //cell.GetComponent<MeshRenderer>().material.SetFloat("_Outline", 0.1f);


            //Debug.Log(cell.transform.parent.GetComponent<Tile>().Neighbours.Count);

            m_lastTouched = cell;
        }
        else
        {
            if (m_lastTouched)
            {
                m_lastTouched.GetComponent<MeshRenderer>().material = m_lastTouched.transform.parent.GetComponent<Tile>().NormalMat;
                //m_lastTouched.GetComponent<MeshRenderer>().material.SetFloat("_Outline", 0);

            }
        }

        EditorUtility.SetDirty(this);
    }

    private GameObject m_lastTouched = null;
    private GameObject m_hovered = null;
}
