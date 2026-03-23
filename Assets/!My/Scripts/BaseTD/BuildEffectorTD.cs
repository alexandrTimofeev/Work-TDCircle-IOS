using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;
using System;
public class BuildEffectorTD : MonoBehaviour
{
    public int AddHealth;
    public float PowerDamage;
    public float PowerSpeed;

    [Space]
    public float Delay = 1f;

    private List<GameObject> targets = new List<GameObject>();
    private Dictionary<GameObject, GetEffectTD> effectBodies = new Dictionary<GameObject, GetEffectTD>();

    private void Start()
    {
        StartCoroutine(MainRoutine());
    }

    public void AddTarget(GameObject target)
    {
        if (target == null || targets.Contains(target))
            return;

        targets.Add(target);
        if (target.TryGetComponent(out GetEffectTD effectBody))
        {
            effectBodies[target] = effectBody; // Используем индексатор вместо Add (безопаснее)
            effectBodies[target].ApplyFirst(this);
        }
    }

    public void RemoveTarget(GameObject target)
    {
        if (target == null) return;

        targets.Remove(target);
        if(effectBodies.ContainsKey(target))
            effectBodies[target].DisableEffect(this);
        effectBodies.Remove(target); // Просто удаляем, проверка не нужна
    }

    private IEnumerator MainRoutine()
    {
        while (true)
        {
            // Очистка уничтоженных объектов
            CleanupDestroyedTargets();

            // Применяем эффекты к целям
            foreach (var target in targets)
            {
                ApplyToTarget(target);
            }

            yield return new WaitForSeconds(Delay);
        }
    }

    private void CleanupDestroyedTargets()
    {
        // Удаляем уничтоженные объекты из списка
        targets.RemoveAll(t => t == null);

        // Собираем ключи уничтоженных объектов для удаления из словаря
        var destroyedKeys = effectBodies.Keys.Where(key => key == null).ToList();
        foreach (var key in destroyedKeys)
        {
            effectBodies.Remove(key);
        }
    }

    private void ApplyToTarget(GameObject target)
    {
        if (target == null) return;

        // Пытаемся получить LifeBody из словаря
        if (effectBodies.TryGetValue(target, out GetEffectTD effectBody))
        {
            ApplyToTarget(effectBody);
        }
        else if (target.TryGetComponent(out effectBody))
        {
            // Если не нашли в словаре, но компонент есть - добавляем
            effectBodies[target] = effectBody;
            ApplyToTarget(effectBody);
        }
    }

    private void ApplyToTarget(GetEffectTD effectBody)
    {
        if (effectBody == null) return;

        effectBody.Apply(this);
    }

    private void OnDestroy()
    {
        DisableAllEffects();
    }

    private void DisableAllEffects()
    {
        foreach (var target in effectBodies.Values)
            target.DisableEffect(this);
    }
}