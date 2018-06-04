using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfos : MonoBehaviour
{
    public enum TypeOfEntitiesPlacement
    {
        none = 0,
        random,
        choice,
    }

    public enum ControllerStart
    {
        human = 0,
        computer,
        random,
    }

    public GameObject MapToLoad = null;
    public string MapToLoadS = "";

    public TypeOfEntitiesPlacement EntitiesPlacement = TypeOfEntitiesPlacement.none;

    public ControllerStart ControllerStarter = ControllerStart.human;
}
