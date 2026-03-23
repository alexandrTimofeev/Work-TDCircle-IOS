using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

[Serializable]
public class GameData
{
    [Space, Header("Effects")]
    public float HitStopPlayerHit = 0.5f;
    public Vector3 CameraShakePunch = Vector3.one;

    [Space, Header("Game Parametrs")]
    public float WaitToEnergy;
    public int AddEnergy;
    public BuildDataPreset[] BuildPresets;

    [Space, Header("MyResources")]
    public IntContainer[] Resources;
    /*public IntContainer LifeContainer;
    public IntContainer EnergyContainer;
    public IntContainer MaterialContainer;*/

    //[Space, Header("Presets")]
    // public BuyData[] BuyDatas;

    /// <summary>
    /// Возвращает полностью защищённый фасад
    /// </summary>
    public ReadOnlyGameData GetReadOnly()
    {
        return ReadOnlyGameData.Create(this);
    }
}

public class ReadOnlyGameData
{
    //Game Parametrs
    public float WaitToEnergy;
    public int AddEnergy;

    //MyResources
    /*public ReadOnlyIntContainer LifeContainer { get; private set; }
    public ReadOnlyIntContainer EnergyContainer { get; private set; }
    public ReadOnlyIntContainer MaterialContainer { get; private set; }*/
    public ReadOnlyIntContainer[] Resources;

    public float HitStopPlayerHit { get; private set; }
    public Vector3 CameraShakePunch { get; private set; }

    //Presets
    private WindowUI[] WindowsInGame;
    public GrappableActionContainer[] FoodEatbleObjects;
    public ISpawnble[] SpawnblePrefs;
    public BuildDataPreset[] BuildPresets;


    // Приватный конструктор, чтобы не создавать объект напрямую
    private ReadOnlyGameData() { }

    /// <summary>
    /// Фабричный метод для создания ReadOnlyGameData
    /// из GameData.
    /// </summary>
    public static ReadOnlyGameData Create(GameData data)
    {
        ReadOnlyIntContainer[] originalResources = new ReadOnlyIntContainer[data.Resources.Length];
        for (int i = 0; i < originalResources.Length; i++)
        {
            originalResources[i] = new ReadOnlyIntContainer(data.Resources[i]);
        }

        var readOnly = new ReadOnlyGameData
        {
            HitStopPlayerHit = data.HitStopPlayerHit,
            CameraShakePunch = data.CameraShakePunch,
            Resources = originalResources,
            WaitToEnergy = data.WaitToEnergy,
            AddEnergy = data.AddEnergy,
            BuildPresets = data.BuildPresets,
        };
        return readOnly;
    }

    public WindowUI[] WindowsForGame()
    {
        return WindowsInGame;
    }
}