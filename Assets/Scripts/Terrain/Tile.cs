using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TileSpecs
{
    public bool Walkable;
    public int WalkMalus;
}

public class Tile : MonoBehaviour
{
    public Material NormalMat;
    public Material HoverMat;

    [HideInInspector]
    public TileSpecs Specs;

    [HideInInspector]
    public Vector2 Coordinates;
    [HideInInspector]
    public Vector2 ArrayCoordinates;
    [HideInInspector]
    public List<Tile> Neighbours = new List<Tile>();

    void Start()
    {

    }

    void Update()
    {

    }

    public void HoverOn()
    {
        GetComponentInChildren<MeshRenderer>().material = HoverMat;
    }

    public void HoverOff()
    {
        GetComponentInChildren<MeshRenderer>().material = NormalMat;
    }

    GameObject mesh;
}
