using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.SceneManagement;

// Глобальные системы
public static class G
{
    public static IInput _Input;
    public static ReadOnlyGameData GlobalData;

    public static void Init()
    {
        Debug.Log("G initialized (global systems)");

        _Input = InputFabric.GetOrCreateInpit(true);
        GameGlobalData.ClearInstance();
        GlobalData = GameGlobalData.Instance.Data;
    }
}

// Локальные G-системы сцены Game
public static class GameG
{
    //Stundart

    public static GameSessionData SessionData;
    //public static GameMain Main;
    public static ScoreSystem ScoreSys;
    public static ResourceManager ResourceManager;
    public static ButtonGameActionMediator ButtonGameMediator;
    public static GrappableObjectMediator GrappableObjectMediator;

    public static CellPlacer CellPlacer;
    public static GameSessionManagerMono GameSessionManagerMono;
    public static LevelData CurrentLevelData;

    //ForThisGame
    public static PlanetPositor Planet;
    public static BuildOnPlanetManager BuildManager;
    public static UIBuildMenu UIBuildMenu;
    public static SpawnManagerPlanet SpawnManager;

    public static void Init()
    {
        Debug.Log("GameG initialized (scene systems)");
        InitBase();

        //ForThisGame
        GameSessionManagerMono = Object.FindAnyObjectByType<GameSessionManagerMono>(FindObjectsInactive.Include);
        int lvlDbg = TestBooleans.GetValueInt("CurrentLvl");
        Debug.Log($"LevelSelectWindow.CurrentLvl {LevelSelectWindow.CurrentLvl}");
        CurrentLevelData = LevelData.LoadFromResources(lvlDbg == -1 ? LevelSelectWindow.CurrentLvl : lvlDbg);

        Planet = Object.FindAnyObjectByType<PlanetPositor>(FindObjectsInactive.Include);
        UIBuildMenu = Object.FindAnyObjectByType<UIBuildMenu>(FindObjectsInactive.Include);
        SpawnManager = Object.FindAnyObjectByType<SpawnManagerPlanet>(FindObjectsInactive.Include);

        BuildManager = new BuildOnPlanetManager();
    }

    private static void InitBase()
    {
        CellPlacer = Object.FindAnyObjectByType<CellPlacer>();

        ScoreSys = new ScoreSystem();
        SessionData = new GameSessionData();
        ButtonGameMediator = new ButtonGameActionMediator();
        GrappableObjectMediator = new GrappableObjectMediator();
        ResourceManager = new ResourceManager();
    }
}

// Локальные G-системы сцены Menu
public static class MenuG
{
    public static void Init()
    {
        Debug.Log("MenuG initialized (scene systems)");
    }
}
