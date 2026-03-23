using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Основной GameEntryPoints
public static class GameEntryPoints
{
    private static readonly Dictionary<string, ISceneEntryPoint> _entryPoints = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void Init()
    {
        // 1️⃣ Инициализация глобальных систем
        G.Init();

        // 2️⃣ Регистрируем EntryPoint сцены через рефлексию
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(ISceneEntryPoint).IsAssignableFrom(t)
                       && !t.IsInterface && !t.IsAbstract);

        //Debug.Log($"types {types.ToList().Count}");

        _entryPoints.Clear();
        List<Type> typesList = types.ToList();
        foreach (var type in typesList)
        {
            var instance = (ISceneEntryPoint)Activator.CreateInstance(type);
            _entryPoints.Add(instance.SceneName, instance);
        }

        // 3️⃣ Подписка на события сцены (один раз)
        SceneManager.sceneLoaded -= SceneLoadedWork;
        SceneManager.sceneLoaded += SceneLoadedWork;

        SceneManager.sceneUnloaded -= SceneUnloadedWork;
        SceneManager.sceneUnloaded += SceneUnloadedWork;

        //_sceneEventsSubscribed = true;

        EntryPointOnce();

#if UNITY_EDITOR
        if(SceneManager.GetActiveScene() != null)
            SceneLoadedWork(SceneManager.GetActiveScene(), LoadSceneMode.Single);
#endif
    }

    private static void SceneLoadedWork(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Load SceneLoadedWork {scene.name}");
        if (_entryPoints.TryGetValue(scene.name, out var entryPoint))
        {
            entryPoint.InitGSystems();  // локальные G сцены
            entryPoint.OnSceneLoaded();
        }
    }

    private static void SceneUnloadedWork(Scene scene)
    {
        if (_entryPoints.TryGetValue(scene.name, out var entryPoint))
            entryPoint.OnSceneUnloaded(); // сброс локальных G
    }

    private static void EntryPointOnce()
    {
        Debug.Log("EntryPointOnce");

        //InterfaceManager.SetBehaviour();
        InitInterfaceButtons();
    }

    private static void InitInterfaceButtons()
    {
        InterfaceManager.OnClickCommand += (command) =>
        {
            switch (command)
            {
                case InterfaceComand.None:
                    break;
                case InterfaceComand.SwitchPause:
                    break;
                case InterfaceComand.OpenPause:
                    break;
                case InterfaceComand.ClosePause:
                    break;
                case InterfaceComand.PlayGame:
                    GameSceneManager.LoadGame();
                    break;
                case InterfaceComand.Exit:
                    GameSceneManager.LoadMenu();
                    break;
                default:
                    break;
            }
        };

        InterfaceManager.OnOpen += (window) =>
        {
            if (window as PauseWindowUI)
            {
                AudioManager.PassFilterMusic(true);
            }
        };

        InterfaceManager.OnClose += (window) =>
        {
            if (window as PauseWindowUI)
            {
                AudioManager.PassFilterMusic(false);
            }
        };

        WindowUI.OnClickMenu = null;
        WindowUI.OnClickNextLvl = null;
        WindowUI.OnClickRestart = null;

        WindowUI.OnClickMenu += () => GameSceneManager.LoadMenu();
        WindowUI.OnClickNextLvl += () => GameSceneManager.LoadGameNextLvl();
        WindowUI.OnClickRestart += () => GameSceneManager.LoadGame();
    }
}