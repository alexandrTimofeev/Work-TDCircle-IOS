using System;
using UnityEngine;

public class DeadlyEnemyTD : MonoBehaviour
{
    [SerializeField] private FarmData[] farmDatas;

    [Space]
    [SerializeField] private FarmData[] farmDataPower;

    private void Awake()
    {
        GetComponent<LifeBody>().OnDead += DeadWork;

        foreach (var farmData in farmDataPower)
        {
            GameG.ResourceManager.AddPower(farmData.ResourceTitle, farmData.Power);
        }
    }

    private void DeadWork()
    {
        foreach (var farm in farmDatas)
        {
            GameG.ResourceManager.AddResource(farm.ResourceTitle, farm.Add, transform.position);
        }

        foreach (var farmData in farmDataPower)
        {
            GameG.ResourceManager.RemovePower(farmData.ResourceTitle, farmData.Power);
        }
    }
}
