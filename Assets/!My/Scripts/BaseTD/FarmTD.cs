using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;

public class FarmTD : MonoBehaviour
{
    [SerializeField] private FarmData[] farmDatas;
    private Dictionary<FarmData, float> keyValues = new Dictionary<FarmData, float>();

    private void Awake()
    {
        StartFarm();
    }

    private void StartFarm()
    {
        foreach (var farmData in farmDatas)
        {
            keyValues.Add(farmData, farmData.Dealy);
        }
    }

    private void Update()
    {
        if (keyValues.Count == 0)
            return;

        List<FarmData> values = new List<FarmData>();

        foreach (var key in keyValues.Keys)
        {
            values.Add(key);
        }

        for (int i = 0; i < values.Count; i++)
        {
            FarmData key = values[i];

            keyValues[key] -= Time.deltaTime;
            if (keyValues[key] <= 0f)
            {
                FarmWork(key);
            }
        }
    }

    private void FarmWork(FarmData key)
    {
        GameG.ResourceManager.AddResource(key.ResourceTitle, key.Add, transform.position + (Vector3.back * 10f));
        keyValues[key] = key.Dealy;
    }
}

[Serializable]
public struct FarmData
{
    public string ResourceTitle;
    public float Dealy;
    public int Add;

    [Space]
    public float Power;
}
