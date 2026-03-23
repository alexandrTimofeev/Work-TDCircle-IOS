using System;
using UnityEngine;
// Простейшая реализация ISpawnble
public class SimpleSpawnable : MonoBehaviour, ISpawnble
{
    [SerializeField] private int weight = 1;
    [SerializeField] private float delayCoef = 1f;
    public int Weight => weight;

    public GameObject GameObject => gameObject;

    public float DelayCoef => delayCoef;


    public event Action<ISpawnble> OnDead;

    public virtual void SpawnInit()
    {
        IContainsISpawner[] containsISpawners = GetComponents<IContainsISpawner>();
        foreach (var item in containsISpawners)        
            item.SpawnInit();        
    }

    private void OnDestroy()
    {
        OnDead?.Invoke(this);
    }
}

public interface IContainsISpawner
{
    void SpawnInit();
}