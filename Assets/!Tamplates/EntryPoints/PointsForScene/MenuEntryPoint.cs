using UnityEngine;

// EntryPoint сцены Menu
public class MenuEntryPoint : ISceneEntryPoint
{
    public string SceneName => GameSceneManager.MenuSceneName;
    public void InitGSystems() => MenuG.Init();
    public void OnSceneLoaded()
    {
        Debug.Log("Menu scene loaded");

        InterfaceManager.Init();
        InterfaceManager.BarMediator.ShowForID("Best", LeaderBoard.GetBestScore());

        InterfaceManager.BarMediator.ShowForID("BestRock", LeaderBoard.GetScore("Rock", true).score);
        InterfaceManager.BarMediator.ShowForID("BestMoney", LeaderBoard.GetScore("Money", true).score);
    }
    public void OnSceneUnloaded() { }
}