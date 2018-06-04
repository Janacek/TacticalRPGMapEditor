using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFlagsInfos : MonoBehaviour
{
    public enum TypeOfFlag
    {
        none = 0,
        good,
        bad,
    }

    public bool DeleteSpawnFlag = false;
    public TypeOfFlag FlagType = TypeOfFlag.none;

    [HideInInspector]
    public Vector2 Coordinates;
    [HideInInspector]
    public Vector2 ArrayCoordinates;
}
