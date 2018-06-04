using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public GameObject Map = null;


    // TO DELETE
    public GameObject Player;
    public GameObject Enemy;
    // ^^^^^^^^^^^^^^^^^^^^^^

    public GameObject HumanController;
    public GameObject ComputerController;

    #region INIT

    void Start()
    {
        GameObject gameInfos = GameObject.Find("GameInfos");
        if (gameInfos == null)
        {
            Debug.LogError("Could not find GameInfos game object.");
            throw new System.Exception("Aborting");
        }

        gi = gameInfos.GetComponent<GameInfos>();

        if (LoadMap() == false)
        {
            throw new System.Exception("Could not load the map. Aborting");
        }

        SpawnsManagement();

        TeamControllersManagement();

        EntitiesManagement();
        InitUI();

        LoadUtils();
    }

    void LoadUtils()
    {
        CurrentUnitIndicator = Resources.Load("Utils/CurrentUnit") as GameObject;
        CurrentUnitIndicator = GameObject.Instantiate(CurrentUnitIndicator);

        CurrentUnitIndicator.transform.parent = Map.transform;
        CurrentUnitIndicator.SetActive(false);
    }

    bool LoadMap()
    {
        if (gi.MapToLoad == null && gi.MapToLoadS.Length <= 0)
            return false;

        if (gi.MapToLoad)
        {
            Map = GameObject.Instantiate(gi.MapToLoad, Vector3.zero, Quaternion.identity) as GameObject;
            if (Map == null)
            {
                return false;
            }
        }
        else if (gi.MapToLoadS.Length > 0)
        {
            GameObject res = Resources.Load("Maps/" + gi.MapToLoadS) as GameObject;
            if (res == null)
            {
                return false;
            }

            Map = GameObject.Instantiate(res, Vector3.zero, Quaternion.identity) as GameObject;
            if (Map == null)
            {
                return false;
            }
        }

        return true;
    }

    void SpawnsManagement()
    {
        if (gi.EntitiesPlacement == GameInfos.TypeOfEntitiesPlacement.random)
        {
            Map.GetComponent<TerrainGenerator>().SpawnFlagsOnMap.ForEach(s =>
            {
                Destroy(s.transform.GetChild(2).gameObject);
                Destroy(s.transform.GetChild(1).gameObject);
                Destroy(s.transform.GetChild(0).gameObject);
            });
        }
        else if (gi.EntitiesPlacement == GameInfos.TypeOfEntitiesPlacement.choice)
        {
            Debug.LogError("Entities placement with choice not available yet");
        }

        AvailableSpawns.AddRange(Map.GetComponent<TerrainGenerator>().SpawnFlagsOnMap);
    }

    void TeamControllersManagement()
    {
        HumanController = GameObject.Instantiate(HumanController);
        HumanController.GetComponent<TeamController>().Init(gameObject);

        /*
        ComputerController = GameObject.Instantiate(ComputerController);
        */

        if (gi.ControllerStarter == GameInfos.ControllerStart.human)
        {
            Controllers.Add(HumanController);
            //Controllers.Add(ComputerController);
        }
        else if (gi.ControllerStarter == GameInfos.ControllerStart.computer)
        {
            //Controller.Add(ComputerController");
            Controllers.Add(HumanController);
        }
        else if (gi.ControllerStarter == GameInfos.ControllerStart.random)
        {
            if (Random.Range(0, 1) == 0)
            {
                Controllers.Add(HumanController);
                //Controllers.Add(ComputerController);
            }
            else
            {
                //Controller.Add(ComputerController");
                Controllers.Add(HumanController);
            }
        }
    }


    // Should load the current Entities from a file/gameobject 
    // Acutally a placeholder
    void EntitiesManagement()
    {
        GameObject player = GameObject.Instantiate(Player);
        GameObject enemy = GameObject.Instantiate(Enemy);

        if (gi.EntitiesPlacement == GameInfos.TypeOfEntitiesPlacement.random)
        {
            //// WOULD NEED REFACTORING
            List<GameObject> goodSpawns = AvailableSpawns.FindAll(x => x.GetComponent<SpawnFlagsInfos>().FlagType == SpawnFlagsInfos.TypeOfFlag.good);
            GameObject randomSpawnFlag = goodSpawns[Random.Range(0, goodSpawns.Count)];

            AvailableSpawns.Remove(randomSpawnFlag);
            goodSpawns.Remove(randomSpawnFlag);
            UsedSpawns.Add(randomSpawnFlag);

            player.transform.position = randomSpawnFlag.transform.position;
            player.GetComponent<EntityInfos>().Coordinates = randomSpawnFlag.GetComponent<SpawnFlagsInfos>().Coordinates;
            player.GetComponent<EntityInfos>().ArrayCoordinates = randomSpawnFlag.GetComponent<SpawnFlagsInfos>().ArrayCoordinates;
            player.transform.parent = randomSpawnFlag.transform.parent;
            HumanController.GetComponent<TeamController>().AddUnit(player);


            List<GameObject> badSpawns = AvailableSpawns.FindAll(x => x.GetComponent<SpawnFlagsInfos>().FlagType == SpawnFlagsInfos.TypeOfFlag.bad);
            GameObject randomBadSpawnFlag = badSpawns[Random.Range(0, badSpawns.Count)];

            AvailableSpawns.Remove(randomBadSpawnFlag);
            badSpawns.Remove(randomBadSpawnFlag);
            UsedSpawns.Add(randomBadSpawnFlag);

            enemy.transform.position = randomBadSpawnFlag.transform.position;
            enemy.GetComponent<EntityInfos>().Coordinates = randomBadSpawnFlag.GetComponent<SpawnFlagsInfos>().Coordinates;
            enemy.GetComponent<EntityInfos>().ArrayCoordinates = randomBadSpawnFlag.GetComponent<SpawnFlagsInfos>().ArrayCoordinates;
            enemy.transform.parent = randomBadSpawnFlag.transform.parent;
            //ComputerController.GetComponent<TeamController>().AddUnit(enemy);
        }
    }

    #region UI METHODS

    void InitUI()
    {
        UI = GameObject.Find("UI");
        if (UI == null)
            throw new System.Exception("Could not find UI GameObject. Make sure it's not missing in the scene");

        Transform bottomPanel = UI.transform.Find("BottomPanel");

        EndTurnButton = bottomPanel.Find("EndTurnButton").gameObject;
        EndTurnButton.GetComponent<Button>().onClick.AddListener(delegate ()
        {
            Controllers[CurrentControllerPlaying].GetComponent<TeamController>().EndTurn();
        });

        NextUnitButton = bottomPanel.Find("NextUnitButton").gameObject;
        NextUnitButton.GetComponent<Button>().onClick.AddListener(delegate ()
        {
            Controllers[CurrentControllerPlaying].GetComponent<TeamController>().NextUnit();
        });

        FinishActionButton = bottomPanel.Find("FinishActionButton").gameObject;
        FinishActionButton.GetComponent<Button>().onClick.AddListener(delegate ()
        {
            Controllers[CurrentControllerPlaying].GetComponent<TeamController>().FinishUnitAction();
        });

        CurrentControllerName = bottomPanel.Find("CurrentControllerName").gameObject;
    }

    #endregion

    #endregion

    #region UPDATE

    void Update()
    {
        if (Map == null)
            return;
        Controllers[CurrentControllerPlaying].GetComponent<TeamController>().ControllerUpdate();
        UpdateUI();
    }

    void UpdateUI()
    {
        bool human = Controllers[CurrentControllerPlaying].GetComponent<HumanTeamController>() != null ? true : false;

        EndTurnButton.GetComponent<Button>().interactable = human;

        //CurrentControllerName.GetComponent<TextMeshProUGUI>().text = Controllers[CurrentControllerPlaying].GetComponent<TeamController>().Name + " turn";
    }

    #endregion

    #region TURN MANAGEMENT

    public void EndTurn()
    {
        ++CurrentControllerPlaying;
        if (CurrentControllerPlaying >= Controllers.Count)
            CurrentControllerPlaying = 0;
    }

    #endregion

    private GameInfos gi = null;

    private List<GameObject> AvailableSpawns = new List<GameObject>();
    private List<GameObject> UsedSpawns = new List<GameObject>();

    private List<GameObject> Controllers = new List<GameObject>();
    private int CurrentControllerPlaying = 0;

    #region UTILS

    [HideInInspector]
    public GameObject CurrentUnitIndicator = null;

    #endregion

    #region UI Components

    private GameObject UI = null;
    private GameObject EndTurnButton = null;
    private GameObject NextUnitButton = null;
    private GameObject FinishActionButton = null;
    private GameObject CurrentControllerName = null;

    #endregion
}
