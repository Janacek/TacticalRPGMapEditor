using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanTeamController : TeamController
{
    #region INIT

    public override void Awake()
    {
        Debug.Log("Human Controller Awake");
    }

    public override void Start()
    {
        Debug.Log("Human Controller Start");
    }

    public override void Init(GameObject gameManager)
    {
        Debug.Log("Human Controller Init");

        gm = gameManager.GetComponent<GameManager>();
    }

    #endregion

    #region UPDATE

    public override void Update()
    {
        //Debug.Log("Human Update");
        gm.CurrentUnitIndicator.transform.parent = Units[CurrentUnit].transform;
        gm.CurrentUnitIndicator.transform.localPosition = Vector3.zero + new Vector3(0, 3, 0);
        gm.CurrentUnitIndicator.SetActive(true);
    }

    public override void ControllerUpdate()
    {
        //Debug.Log("Human Controller Update");
    }

    #endregion

    #region UTILS

    public override void EndTurn()
    {
        if (Units.Count > 0)
        {
            Debug.Log("Still units to use");
        }
        else
        {
            Debug.Log("Human End Turn");
            ResetTurnParameters();
        }
    }

    public override void NextUnit()
    {
        if (Units.Count <= 0)
        {
            Debug.Log("No more units to use");
        }
        else if (Units.Count == 1)
        {
            Debug.Log("Only one unit to use");
        }
        else
        {
            ++CurrentUnit;
            if (CurrentUnit >= Units.Count)
            {
                CurrentUnit = 0;
            }
        }
    }

    public override void FinishUnitAction()
    {
        if (Units.Count <= 0)
        {
            Debug.Log("This should never happen");
        }
        else if (Units.Count == 1)
        {
            PlayedUnits.Add(Units[CurrentUnit]);
            Units.Remove(Units[CurrentUnit]);
            EndTurn();
        }
        else
        {
            PlayedUnits.Add(Units[CurrentUnit]);
            Units.Remove(Units[CurrentUnit]);
            NextUnit();
        }
    }

    void ResetTurnParameters()
    {
        Units.AddRange(PlayedUnits);
        PlayedUnits.Clear();
        gm.EndTurn();

        CurrentUnit = 0;
    }

    #endregion
}
