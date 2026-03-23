using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public static class GameSceneManager
{
    public const string MenuSceneName = "Menu";
    public const string GameSceneName = "Game";
    public static event Action OnGameLoad;
    public static event Action OnMenuLoad;

    private static string lastScene;

    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    public static void Init()
    {
        OnGameLoad = null;
        OnMenuLoad = null;
        SceneManager.sceneUnloaded -= ForgiveLastScene;
        SceneManager.sceneUnloaded += ForgiveLastScene;
        SceneManager.sceneLoaded -= InvokeSceneLoad;
        SceneManager.sceneLoaded += InvokeSceneLoad;
    }

    public static void LoadGame()
    {
        //lastScene = "";
        SceneManager.LoadScene(GameSceneName);
    }

    public static void LoadMenu()
    {
        //lastScene = "";
        SceneManager.LoadScene(MenuSceneName);
    }

    private static void ForgiveLastScene(Scene scene)
    {
        lastScene = "";
    }

    public static void LoadGameNextLvl()
    {
        LevelSelectWindow.LoadLevel(LevelSelectWindow.CurrentLvl + 1);
    }

    private static void InvokeSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (lastScene == scene.name)        
            return;        

        if (scene.name == MenuSceneName)        
            OnMenuLoad?.Invoke();        
        else if (scene.name == GameSceneName)
            OnGameLoad?.Invoke();

        lastScene = scene.name;
    }
}