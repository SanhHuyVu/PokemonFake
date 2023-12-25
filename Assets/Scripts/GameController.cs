using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog, Menu, Cutscene, Paused}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;

    // temp
    public PlayerController PlayerController { get { return playerController; } }

    MenuController menuController;

    GameState state;
    GameState stateBeforePause;

    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PrevScene { get; private set; }

    public static GameController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        menuController = GetComponent<MenuController>();

        ConditionsDB.Init();
        UnitDB.Init();
        AAbilitysDB.Init();
    }

    private void Start()
    {
        battleSystem.OnBattleOver += EndBattle;

        DialogManager.Instance.OnShowDialog += () =>{ state = GameState.Dialog; };
        DialogManager.Instance.OnCloseDialog += () =>
        {
            if (state == GameState.Dialog)
                state = GameState.FreeRoam; 
        };

        menuController.onBack += () =>
        {
            state = GameState.FreeRoam;
        };
        menuController.onMenuSelected += OnMenuSelected;
    }

    public void PauseGame(bool pause)
    {
        if (pause == true)
        {
            stateBeforePause = state;
            state = GameState.Paused;
        }
        else
        {
            state = stateBeforePause;
        }
    }

    public void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerUnits = playerController.GetComponent<UnitParty>().UnitsParty();
        var enemyUnits = CurrentScene.GetComponent<MapArea>().GetRandomEnemyUnitList().enemyUnits;

        battleSystem.StartBattle(playerUnits, enemyUnits);
    }

    EliteController elite;
    public void StartEliteBattle(EliteController elite)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        this.elite = elite;
        var playerUnits = playerController.GetComponent<UnitParty>().UnitsParty();
        var eliteUnits = elite.GetComponent<UnitParty>().UnitsParty();

        battleSystem.StartBattle(playerUnits, eliteUnits, true);
    }

    public void OnEnterEliteView(EliteController elite)
    {
        state = GameState.Cutscene;
        StartCoroutine(elite.TriggerEliteBattle(playerController));
    }

    void EndBattle(bool won)
    {
        if (elite != null && won == true)
        {
            elite.BattleLost();
            elite = null;
        }
        StartCoroutine(End());
    }
    IEnumerator End()
    {
        yield return new WaitForSeconds(2f);
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
            {
                menuController.OpenMenu();
                state = GameState.Menu;
            }
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
        else if (state == GameState.Menu)
        {
            if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                menuController.CloseMenu();
            }
        }
    }

    public void SetCurrentScene(SceneDetails currScene)
    {
        PrevScene = CurrentScene;
        CurrentScene = currScene;
    }

    public void OnMenuSelected(int selectedItem)
    {
        switch (selectedItem)
        {
            case 0: // view party
                var playerUnits = playerController.GetComponent<UnitParty>().UnitsParty();
                break;
            case 1:// save
                SavingSystem.i.Save("saveSlot1");
                menuController.CloseMenu();
                break;
            case 2:// load
                SavingSystem.i.Load("saveSlot1");
                menuController.CloseMenu();
                break;
        }
    }
}
