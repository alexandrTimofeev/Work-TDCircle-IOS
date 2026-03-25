using DG.Tweening;
using System;
using UnityEngine;

// EntryPoint сцены Game
public class GameEntryPoint : ISceneEntryPoint
{
    public string SceneName => GameSceneManager.GameSceneName;
    public void InitGSystems() => GameG.Init();
    public void OnSceneLoaded()
    {
        Debug.Log("Game scene loaded");

        GameG.SessionData.Init(G.GlobalData);
        PocketRandomazer.Clear();
        MyTagsObserver.Initialize(true);

        InitButtonMediator();
        InitInterface();
        InitInterfaceCommands();
        InitPlayer();
        InitSpawnManagerAndFinal();
        InitBonusMediator();
        InitLevel();
        InitSpecialWindows();
        InitResourceManager();

        AudioManager.Init();

        //GameG.PatternFFly.targetTest = GameG.Player.transform;

        GamePause.SetPause(false);
    }

    public void OnSceneUnloaded()
    {
        GameG.BuildManager.Clear();
    }

    private static void InitInterface()
    {
        InterfaceManager.Init();
        //GameG.ScoreSys.OnAddScore += (score, point) => InterfaceManager.CreateScoreFlyingText(score, point);

        InterfaceManager.BarMediator.ShowForID("Score", 0);
        GameG.ScoreSys.OnScoreChange += (info) =>
        {
            InterfaceManager.BarMediator.ShowForID("Score", info.Value);
            if(info.Point.HasValue)
                InterfaceManager.CreateScoreFlyingText(info.Delta, info.Point.Value);
        };

        InitReloadingUI();

        GameG.BuildManager.OnStateChange += (state) => GameG.UIBuildMenu.SetBuildState(state);

        //InterfaceManager.BarMediator.ShowForID("BulletsCount", GameG.Player.Gun.CountBullets.Steps);
        //GameG.Player.OnCountBulletChange += (value) => InterfaceManager.BarMediator.ShowForID("BulletsCount", value);
        //GameG.Player.OnReloadingTimeChange += (wait) => InterfaceManager.BarMediator.ShowForID("Reloading", 
        //    (GameG.Player.Gun.ReloadingTime - wait) / GameG.Player.Gun.ReloadingTime);

        //InterfaceManager.BarMediator.ShowForID("Life", GameG.SessionData.LifeContainer.Steps);
        //GameG.SessionData.LifeContainer.OnChangeValue += (value) => InterfaceManager.BarMediator.ShowForID("Life", value);
    }

    private void InitButtonMediator()
    {
        GameG.ButtonGameMediator.OnClick += (actionInfo) =>
        {
            switch (actionInfo.ActionType)
            {
                case ButtonGameActionType.None:
                    break;
                case ButtonGameActionType.Delite:
                    break;
            }
        };
    }

    private void InitPlayer()
    {
        //GameG.Player.SetBehaviour(G._Input);
        //GameG.Player.OnGrap += (grapOb) => GameG.GrappableObjectMediator.InvokeEventEnter(grapOb);

        GameG.BuildManager.Init(G._Input);

        GameG.ResourceManager.OnChangeResource += (container) =>
        {
            if (container.Title == "Life" && container.Value <= 0)
                GameG.GameSessionManagerMono.Lose();
        };

        InitPlayerDamage();
    }

    private static void InitReloadingUI()
    {
        //GameG.Player.OnStartReload += OnStartReloadUIRection;
        //GameG.Player.OnEndReload += OnEndReloadUIRection;
    }

    private void InitPlayerDamage()
    {
        /*GameG.Player.OnDamage += (d) =>
        {
            EffectsManagerMono.HitStop(G.GlobalData.HitStopPlayerHit, 0.18f);
            EffectsManagerMono.CameraShake(G.GlobalData.CameraShakePunch, isIndependentUpdate: true);
            GameG.SessionData.LifeContainer.RemoveValue(1);
        };

        GameG.SessionData.LifeContainer.OnDownfullValue += (d) => Lose();*/
    }

    private void InitSpawnManagerAndFinal()
    {
        GameG.SpawnManager.OnAllWavesCompleted += GameG.GameSessionManagerMono.WinWait;

        GameG.SpawnManager.StartSpawning(GameG.CurrentLevelData.SpawnPreset);
    }

    private void InitBonusMediator()
    {
    }

    public void InitInterfaceCommands()
    {
        InterfaceManager.OnClickCommand += (command) =>
        {
            switch (command)
            {
                case InterfaceComand.OpenPause:
                    GamePause.SetPause(true);
                    break;
                case InterfaceComand.ClosePause:
                    GamePause.SetPause(false);
                    break;

                default:
                    break;
            }
        };
    }

    private void InitLevel()
    {
        GameG.UIBuildMenu.Init(GameG.CurrentLevelData.GetPresets());

        UnityEngine.GameObject.Find("PlanetSprite").GetComponent<Renderer>().material.mainTexture = 
            GameG.CurrentLevelData.Background.texture;        
    }

    private void InitSpecialWindows()
    {
    }

    private void InitResourceManager()
    {
        GameG.ResourceManager.Init();

        GameG.ResourceManager.OnChangeResource += (container) =>
        {
            if (container.Title == "Material" && container.Value >= 1000)
                AchieviementSystem.ForceUnlock("MaxMaterial");

            if (container.Title == "Energy" && container.Value >= 1000)
                AchieviementSystem.ForceUnlock("MaxEnergy");
        };
    }
}