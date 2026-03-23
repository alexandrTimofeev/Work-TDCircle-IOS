using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameGlobalData", menuName = "SGames/GameGlobalData")]
public class GameGlobalData : ScriptableObject
{
    private const string RESOURCE_PATH = "GameGlobalData";

    [SerializeField] private GameData data;
    private ReadOnlyGameData readOnlyData;
    public ReadOnlyGameData Data
    {
        get
        {
            if (readOnlyData == null)
                readOnlyData = data.GetReadOnly();
            return readOnlyData;
        }
    }

    private static GameGlobalData instance;
    public static GameGlobalData Instance
    {
        get
        {
            if (instance != null)
                return instance;

            instance = Resources.Load<GameGlobalData>(RESOURCE_PATH);
            if(instance)
                instance.ClearData();

            if (instance == null)
            {
                Debug.LogError(
                    $"GameGlobalDataPreload not found at MyResources/{RESOURCE_PATH}");
            }

            return instance;
        }
    }

    private void ClearData()
    {
        readOnlyData = null;
    }

    public static void ClearInstance()
    {
        instance = null;
    }
}
