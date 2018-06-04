using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TeamController : MonoBehaviour
{
    [HideInInspector]
    public GameManager gm = null;

    #region TURN TYPES

    public enum TurnTypes
    {
        Movement = 0,
        Attack = 1,
    }

    #endregion

    #region INIT

    public abstract void Awake();
    public abstract void Start();

    public abstract void Init(GameObject gameManager);

    public string Name = "";

    public void AddUnits(List<GameObject> units)
    {
        units.ForEach(u =>
        {
            if (Units.Contains(u) == false)
                Units.Add(u);
        });
    }

    public void AddUnit(GameObject unit)
    {
        if (Units.Contains(unit) == false)
            Units.Add(unit);
    }

    #endregion

    #region UPDATE

    public abstract void Update();
    public abstract void ControllerUpdate();

    #endregion

    #region UTILS

    public abstract void EndTurn();
    public abstract void NextUnit();
    public abstract void FinishUnitAction();

    public int NumberOfUnits
    {
        get
        {
            return Units.Count + PlayedUnits.Count;
        }
    }

    #region UNITS

    protected List<GameObject> Units = new List<GameObject>();
    protected List<GameObject> PlayedUnits = new List<GameObject>();

    protected int CurrentUnit = 0;

    #endregion

    #region Turn Type

    protected TurnTypes TurnType = TurnTypes.Movement;
    public TurnTypes GetTurnType
    {
        get
        {
            return TurnType;
        }
    }

    #endregion

    #endregion
}
