using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameSessionManagerMono : MonoBehaviour
{
    [SerializeField] private DamageHurtBox2D planetHurtBox;

    [Space]
    [SerializeField] private BuildbleObject testBuildObject;
    [SerializeField] private BuildRule testRule;

    private void Start()
    {
        planetHurtBox.OnDamage += OnPlanetDamageWork;

        StartCoroutine(MainRoutine());
        StartCoroutine(CreateEnergyRoutine());
    }

    private void OnPlanetDamageWork(DamageContext2D context)
    {
        GameG.ResourceManager.MyResources["Life"].RemoveValue(context.Dmg);
    }

    public void ClickBuild()
    {
        //GameG.BuildManager.StartBuildState(testBuildObject, GameG.Planet, testRule);
        if (GameG.BuildManager.State == BuildPlanetState.SelectToPlace)
            GameG.BuildManager.ClickBuild();
    }

    public void ClickCancelBuild()
    {
        GameG.BuildManager.SetStateOff();
    }

    private IEnumerator MainRoutine()
    {
        while (true)
        {
            yield return new WaitWhile(() => GamePause.IsPause);
            yield return null;
        }
    }

    private IEnumerator CreateEnergyRoutine()
    {
        while (true)
        {
            yield return new WaitWhile(() => GamePause.IsPause);
            yield return new WaitForSeconds(G.GlobalData.WaitToEnergy);
            GameG.ResourceManager.MyResources["Energy"].AddValue(G.GlobalData.AddEnergy);
        }
    }

    public void Lose()
    {
        GamePause.SetPause(true);
        GameG.SpawnManager.StopSpawning(true);

        InterfaceManager.ShowLoseWindow(0, 0);
    }

    public void Win()
    {
        GamePause.SetPause(true);

        InterfaceManager.ShowWinWindow(0, 0);
        AchieviementSystem.ForceUnlock($"Level{LevelSelectWindow.CurrentLvl + 1}");

        if (GameG.ResourceManager.MyResources["Life"].Value >= 100) {
            AchieviementSystem.ForceUnlock("Survivor");

            if (LevelSelectWindow.CurrentLvl == 6)
                AchieviementSystem.ForceUnlock("Invictible");
        }

        LevelSelectWindow.CompliteLvl();
    }

    public void WinWait()
    {
        StartCoroutine(WinWaitRoutine());
    }

    private IEnumerator WinWaitRoutine()
    {
        yield return new WaitWhile(() => GameG.SpawnManager.ActiveEnemiesCount > 0);
        Win();
    }
}
