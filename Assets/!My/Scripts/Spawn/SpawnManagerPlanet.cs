using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

// ==================== Интерфейс спавна ====================
public interface ISpawnble
{
    void SpawnInit();
    GameObject GameObject { get; }
    int Weight { get; }
    float DelayCoef { get; }

    event Action<ISpawnble> OnDead;
}

// ==================== Перечисление типов ожидания ====================
public enum NextSpawnWaitType
{
    OnlyWait,                       // Просто ждем Wait секунд
    AllDead,                        // Все враги из всех волн мертвы
    AllLastWaveDead,                // Все враги только из последней волны мертвы
    ValueProccentAllDead,            // Определенный процент всех врагов мертв (по количеству)
    ValueProccentLastWaveDead,       // Определенный процент врагов последней волны мертв (по количеству)
    ValueProccentAllWeight,          // Определенный процент всех врагов мертв (по весу)
    ValueProccentLastWeight          // Определенный процент врагов последней волны мертв (по весу)
}

// ==================== Вспомогательные классы для волны ====================
[Serializable]
public class SpawnableVariant
{
    [Tooltip("Вариант префаба")]
    public SimpleSpawnable Prefab;
    [Tooltip("Вес вероятности (чем больше, тем чаще спавнится)")]
    public int Weight = 1;
}

[Serializable]
public class SpawnableGroup
{
    [Header("Основной префаб")]
    [Tooltip("Базовый объект для спавна")]
    public SimpleSpawnable Prefab;

    [Header("Количество")]
    [Tooltip("Фиксированное количество")]
    public int FixedCount = 1;
    [Tooltip("Минимальное количество (для случайного диапазона)")]
    public int MinCount = 1;
    [Tooltip("Максимальное количество (для случайного диапазона)")]
    public int MaxCount = 1;
    [Tooltip("Использовать случайное количество в диапазоне")]
    public bool UseRandomCount = false;

    [Header("Вариации")]
    [Tooltip("Использовать случайные варианты с весами")]
    public bool UseRandomWeight = false;
    [Tooltip("Варианты объектов (если нужно разнообразие)")]
    public SpawnableVariant[] Variants;

    /// <summary>Получить количество объектов для спавна</summary>
    public int GetSpawnCount()
    {
        if (UseRandomCount)
            return Random.Range(MinCount, MaxCount + 1);
        return FixedCount;
    }

    /// <summary>Создать копию группы</summary>
    public SpawnableGroup Clone()
    {
        var clone = new SpawnableGroup
        {
            Prefab = this.Prefab,
            FixedCount = this.FixedCount,
            MinCount = this.MinCount,
            MaxCount = this.MaxCount,
            UseRandomCount = this.UseRandomCount,
            UseRandomWeight = this.UseRandomWeight
        };
        if (this.Variants != null)
        {
            clone.Variants = new SpawnableVariant[this.Variants.Length];
            for (int i = 0; i < this.Variants.Length; i++)
            {
                clone.Variants[i] = new SpawnableVariant
                {
                    Prefab = this.Variants[i].Prefab,
                    Weight = this.Variants[i].Weight
                };
            }
        }
        return clone;
    }
}

[Serializable]
public class SpawnWaveInfo
{
    [Header("Состав волны")]
    [Tooltip("Список групп объектов для спавна")]
    public List<SpawnableGroup> SpawnGroups = new List<SpawnableGroup>();

    [Header("Настройки волны")]
    [Tooltip("Задержка между спавнами внутри волны (сек)")]
    public float SpawnDelay = 0f;
    [Tooltip("Перемешивать порядок спавна групп")]
    public bool ShuffleGroups = false;
    [Tooltip("Максимальное количество одновременно живых врагов (0 = без ограничений)")]
    public int MaxConcurrentEnemies = 0;

    [Header("Ожидание после волны")]
    [Tooltip("Время ожидания после волны")]
    public float Wait = 1f;
    [Tooltip("Тип ожидания следующей волны")]
    public NextSpawnWaitType WaitType = NextSpawnWaitType.OnlyWait;
    [Tooltip("Значение для процентных типов (0-1)")]
    [Range(0f, 1f)]
    public float Value = 0.5f;

    /// <summary>Получить все ISpawnble объекты с учётом весов и количества</summary>
    public ISpawnble[] GetAllISpawnble()
    {
        List<ISpawnble> result = new List<ISpawnble>();

        foreach (var group in SpawnGroups)
        {
            if (group.Prefab == null && (group.Variants == null || group.Variants.Length == 0))
                continue;

            int count = group.GetSpawnCount();
            for (int i = 0; i < count; i++)
            {
                if (group.UseRandomWeight && group.Variants != null && group.Variants.Length > 0)
                {
                    ISpawnble variant = GetRandomVariantByWeight(group.Variants);
                    if (variant != null)
                        result.Add(variant);
                }
                else if (group.Prefab != null)
                {
                    result.Add(group.Prefab);
                }
            }
        }

        if (ShuffleGroups)
            result = result.OrderBy(x => Random.value).ToList();

        return result.ToArray();
    }

    private ISpawnble GetRandomVariantByWeight(SpawnableVariant[] variants)
    {
        int totalWeight = variants.Sum(v => v.Weight);
        int random = Random.Range(0, totalWeight);
        int current = 0;
        foreach (var v in variants)
        {
            current += v.Weight;
            if (random < current)
                return v.Prefab;
        }
        return variants[0].Prefab;
    }

    /// <summary>Получить общий вес волны (сумма весов всех спавнов)</summary>
    public int GetTotalWeight()
    {
        int total = 0;
        foreach (var group in SpawnGroups)
        {
            int count = group.GetSpawnCount();
            if (group.Prefab != null)
                total += group.Prefab.Weight * count;

            if (group.UseRandomWeight && group.Variants != null)
            {
                // Приблизительный средний вес вариантов
                float avg = (float)group.Variants.Average(v => v.Weight);
                total += Mathf.RoundToInt(avg * count);
            }
        }
        return total;
    }

    /// <summary>Проверить, все ли спавны валидны</summary>
    public bool IsValid()
    {
        if (SpawnGroups == null || SpawnGroups.Count == 0)
            return false;
        foreach (var g in SpawnGroups)
        {
            if (g.Prefab == null && (g.Variants == null || g.Variants.Length == 0))
                return false;
            if (g.UseRandomWeight && g.Variants != null && g.Variants.Any(v => v.Prefab == null))
                return false;
        }
        return true;
    }

    /// <summary>Создать глубокую копию волны</summary>
    public SpawnWaveInfo Clone()
    {
        var clone = new SpawnWaveInfo
        {
            SpawnGroups = new List<SpawnableGroup>(),
            SpawnDelay = this.SpawnDelay,
            ShuffleGroups = this.ShuffleGroups,
            MaxConcurrentEnemies = this.MaxConcurrentEnemies,
            Wait = this.Wait,
            WaitType = this.WaitType,
            Value = this.Value
        };
        foreach (var g in this.SpawnGroups)
            clone.SpawnGroups.Add(g.Clone());
        return clone;
    }
}

// ==================== Пресет спавна ====================
[Serializable]
public class SpawnPreset
{
    [Header("Основные настройки")]
    [Tooltip("Название пресета (для удобства)")]
    public string PresetName = "New Spawn Preset";
    [Tooltip("Волны спавна")]
    public SpawnWaveInfo[] waveInfos;

    [Header("Глобальные настройки")]
    [Tooltip("Случайный поворот при спавне")]
    public bool randomRotation = false;
    [Tooltip("Зациклить волны после завершения")]
    public bool loopWaves = false;
    [Tooltip("Автоматически начать спавн при старте")]
    public bool autoStart = true;

    [Header("Сложность")]
    [Tooltip("Множитель количества врагов (для баланса)")]
    [Range(0.1f, 3f)]
    public float difficultyScale = 1f;
    [Tooltip("Множитель веса врагов (для сложности)")]
    [Range(0.1f, 3f)]
    public float weightScale = 1f;

    [Header("Дополнительно")]
    [Tooltip("Максимальное общее количество врагов (0 = без лимита)")]
    public int maxTotalEnemies = 0;
    [Tooltip("Время между волнами (переопределяет индивидуальные настройки)")]
    public float globalWaveDelay = -1f; // -1 = использовать индивидуальные

    /// <summary>Получить все волны с применёнными масштабами сложности</summary>
    public SpawnWaveInfo[] GetScaledWaves()
    {
        if (waveInfos == null) return Array.Empty<SpawnWaveInfo>();
        var scaled = new SpawnWaveInfo[waveInfos.Length];
        for (int i = 0; i < waveInfos.Length; i++)
        {
            scaled[i] = waveInfos[i].Clone();
            if (globalWaveDelay >= 0)
                scaled[i].Wait = globalWaveDelay;

            if (Mathf.Abs(difficultyScale - 1f) > 0.01f)
            {
                foreach (var g in scaled[i].SpawnGroups)
                {
                    if (g.UseRandomCount)
                    {
                        g.MinCount = Mathf.RoundToInt(g.MinCount * difficultyScale);
                        g.MaxCount = Mathf.RoundToInt(g.MaxCount * difficultyScale);
                    }
                    else
                    {
                        g.FixedCount = Mathf.RoundToInt(g.FixedCount * difficultyScale);
                    }
                }
            }
        }
        return scaled;
    }

    /// <summary>Получить общее количество врагов во всех волнах</summary>
    public int GetTotalEnemyCount()
    {
        if (waveInfos == null) return 0;
        return waveInfos.Sum(w => w.SpawnGroups.Sum(g => g.GetSpawnCount()));
    }

    /// <summary>Получить общий вес всех врагов</summary>
    public int GetTotalWeight()
    {
        if (waveInfos == null) return 0;
        return Mathf.RoundToInt(waveInfos.Sum(w => w.GetTotalWeight()) * weightScale);
    }

    /// <summary>Проверить валидность пресета</summary>
    public bool IsValid()
    {
        return waveInfos != null && waveInfos.Length > 0 && waveInfos.All(w => w.IsValid());
    }

    /// <summary>Создать копию пресета</summary>
    public SpawnPreset Clone()
    {
        return new SpawnPreset
        {
            PresetName = this.PresetName + " (Copy)",
            waveInfos = this.waveInfos?.Select(w => w.Clone()).ToArray(),
            randomRotation = this.randomRotation,
            loopWaves = this.loopWaves,
            autoStart = this.autoStart,
            difficultyScale = this.difficultyScale,
            weightScale = this.weightScale,
            maxTotalEnemies = this.maxTotalEnemies,
            globalWaveDelay = this.globalWaveDelay
        };
    }
}

// ==================== Менеджер спавна ====================
public class SpawnManagerPlanet : MonoBehaviour
{
    [Header("Настройки спавна")]
    [SerializeField] private SpawnPreset spawnPreset;
    [SerializeField] private Transform[] spawnPoints; // Опционально: точки спавна вместо рандома

    [Header("Позиционирование")]
    [SerializeField] private float distAtCenter = 5f;
    [SerializeField] private float distYMax = 5f;
    [SerializeField] private bool useLocalPosition = true;

    [Header("Сложность (динамическая)")]
    [SerializeField] private float globalDifficultyScale = 1f;

    [Header("Отладка")]
    [SerializeField] private bool autoStart = true;
    [SerializeField] private bool debugMode = false;

    public event Action<SpawnWaveInfo> OnSpawnWave;
    public event Action<ISpawnble> OnSpawn;
    public event Action OnAllWavesCompleted;
    public event Action<int> OnWaveCompleted; // номер волны

    private Dictionary<int, List<ISpawnble>> activeSpawnedObjects = new Dictionary<int, List<ISpawnble>>();
    private Dictionary<int, SpawnWaveInfo> waveInfoCache = new Dictionary<int, SpawnWaveInfo>();
    private int currentWaveIndex = -1;
    private bool isSpawning = false;
    private Coroutine spawnRoutine;

    private SpawnWaveInfo[] currentScaledWaves;

    public bool IsActive => isSpawning;
    public int CurrentWave => currentWaveIndex;
    public int TotalWaves => spawnPreset?.waveInfos?.Length ?? 0;
    public int ActiveEnemiesCount => activeSpawnedObjects.Values.Sum(list => list.Count);

    // Для доступа из инспектора (если нужно)
    //public ISpawnble[] SpawnblePrefs => G.GlobalData?.SpawnblePrefs ?? Array.Empty<ISpawnble>();

    private void Start()
    {
        if (autoStart)
            StartSpawning();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        ClearAllSpawned();
    }

    public void StartSpawning(SpawnPreset spawnPreset)
    {
        this.spawnPreset = spawnPreset;
        StartSpawning();
    }

    public void StartSpawning()
    {
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);
        spawnRoutine = StartCoroutine(MainSpawnRoutine());
    }

    public void StopSpawning(bool insactiveObjects = false)
    {
        isSpawning = false;
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }

        foreach (var spawnbles in activeSpawnedObjects.Values)
        {
            spawnbles.ForEach((sp) => sp.GameObject.SetActive(false) );
        }
    }

    public void PauseSpawning() => isSpawning = false;

    public void ResumeSpawning()
    {
        if (!isSpawning && spawnRoutine != null)
            isSpawning = true;
    }

    private IEnumerator MainSpawnRoutine()
    {
        isSpawning = true;

        if (spawnPreset == null || !spawnPreset.IsValid())
        {
            Debug.LogError($"[{name}] SpawnPreset is invalid!");
            yield break;
        }

        // Получаем волны с учётом сложности из пресета
        currentScaledWaves = spawnPreset.GetScaledWaves();

        // Применяем глобальный множитель сложности (динамический)
        foreach (var wave in currentScaledWaves)
        {
            foreach (var group in wave.SpawnGroups)
            {
                if (group.UseRandomCount)
                {
                    group.MinCount = Mathf.RoundToInt(group.MinCount * globalDifficultyScale);
                    group.MaxCount = Mathf.RoundToInt(group.MaxCount * globalDifficultyScale);
                }
                else
                {
                    group.FixedCount = Mathf.RoundToInt(group.FixedCount * globalDifficultyScale);
                }
            }
        }

        int waveCount = currentScaledWaves.Length;
        currentWaveIndex = 0;

        while (currentWaveIndex < waveCount)
        {
            if (IsPause())
            {
                yield return null;
                continue;
            }

            SpawnWaveInfo currentWave = currentScaledWaves[currentWaveIndex];

            // Проверка лимита одновременных врагов
            if (currentWave.MaxConcurrentEnemies > 0)
            {
                while (ActiveEnemiesCount >= currentWave.MaxConcurrentEnemies && isSpawning)
                {
                    yield return new WaitForSeconds(0.2f);
                }
            }

            if (debugMode)
                Debug.Log($"[{name}] Starting wave {currentWaveIndex + 1}/{waveCount}");

            OnSpawnWave?.Invoke(currentWave);

            // Спавним всех врагов волны
            yield return StartCoroutine(SpawnWaveRoutine(currentWave));

            // Ждем завершения волны
            yield return StartCoroutine(WaitForWaveCompletion(currentWave));

            OnWaveCompleted?.Invoke(currentWaveIndex);
            currentWaveIndex++;

            if (spawnPreset.loopWaves && currentWaveIndex >= waveCount)
            {
                currentWaveIndex = 0;
                if (debugMode)
                    Debug.Log($"[{name}] Looping waves");
            }
        }

        if (!spawnPreset.loopWaves)
        {
            isSpawning = false;
            OnAllWavesCompleted?.Invoke();
            if (debugMode)
                Debug.Log($"[{name}] All waves completed");
        }
    }

    // Добавляем словарь для кэша массивов спавнов по индексу волны
    private Dictionary<int, ISpawnble[]> waveSpawnablesCache = new Dictionary<int, ISpawnble[]>();

    private IEnumerator SpawnWaveRoutine(SpawnWaveInfo waveInfo)
    {
        var allSpawnables = waveInfo.GetAllISpawnble();
        if (allSpawnables.Length == 0)
            yield break;

        // Кэшируем массив для этой волны
        waveSpawnablesCache[currentWaveIndex] = allSpawnables;

        if (!activeSpawnedObjects.ContainsKey(currentWaveIndex))
            activeSpawnedObjects[currentWaveIndex] = new List<ISpawnble>();

        waveInfoCache[currentWaveIndex] = waveInfo;

        for (int i = 0; i < allSpawnables.Length; i++)
        {
            if (IsPause())
            {
                i--;
                yield return null;
                continue;
            }

            ISpawnble spawnable = allSpawnables[i];
            if (spawnable?.GameObject == null)
            {
                Debug.LogWarning($"[{name}] Null spawnable at wave {currentWaveIndex}, index {i}");
                continue;
            }

            ISpawnble spawnble = Spawn(spawnable);

            if (waveInfo.SpawnDelay > 0 && i < allSpawnables.Length - 1)
                yield return new WaitForSeconds(waveInfo.SpawnDelay * spawnble.DelayCoef);
        }
    }

    private IEnumerator WaitForWaveCompletion(SpawnWaveInfo waveInfo)
    {
        if (waveInfo.WaitType == NextSpawnWaitType.OnlyWait)
        {
            yield return new WaitForSeconds(waveInfo.Wait);
            yield break;
        }

        float targetValue = waveInfo.Value;
        bool useLastWave = waveInfo.WaitType == NextSpawnWaitType.AllLastWaveDead ||
                          waveInfo.WaitType == NextSpawnWaitType.ValueProccentLastWaveDead ||
                          waveInfo.WaitType == NextSpawnWaitType.ValueProccentLastWeight;

        ProgressType progressType = (waveInfo.WaitType == NextSpawnWaitType.ValueProccentAllWeight ||
                                     waveInfo.WaitType == NextSpawnWaitType.ValueProccentLastWeight)
                                     ? ProgressType.Weight
                                     : ProgressType.Count;

        while (isSpawning && !IsPause())
        {
            float progress = GetWaveProgress(useLastWave, progressType);
            bool conditionMet = false;

            switch (waveInfo.WaitType)
            {
                case NextSpawnWaitType.AllDead:
                case NextSpawnWaitType.AllLastWaveDead:
                    conditionMet = progress >= 1f;
                    break;
                case NextSpawnWaitType.ValueProccentAllDead:
                case NextSpawnWaitType.ValueProccentLastWaveDead:
                case NextSpawnWaitType.ValueProccentAllWeight:
                case NextSpawnWaitType.ValueProccentLastWeight:
                    conditionMet = progress >= targetValue;
                    break;
            }

            if (conditionMet)
                break;

            yield return new WaitForSeconds(0.1f);
        }
    }

    private enum ProgressType { Count, Weight }
    private float GetWaveProgress(bool useLastWaveOnly, ProgressType progressType)
    {
        int totalCount = 0;
        int deadCount = 0;
        int totalWeight = 0;
        int deadWeight = 0;

        if (useLastWaveOnly)
        {
            if (activeSpawnedObjects.TryGetValue(currentWaveIndex, out var list) &&
                waveSpawnablesCache.TryGetValue(currentWaveIndex, out var spawns))
            {
                totalCount = spawns.Length;
                totalWeight = spawns.Sum(s => s?.Weight ?? 0);
                deadCount = totalCount - list.Count;
                deadWeight = totalWeight - list.Sum(s => s?.Weight ?? 0);
            }
        }
        else
        {
            // Суммируем по всем волнам из кэша
            foreach (var kvp in waveSpawnablesCache)
            {
                totalCount += kvp.Value.Length;
                totalWeight += kvp.Value.Sum(s => s?.Weight ?? 0);
            }
            int aliveCount = activeSpawnedObjects.Values.Sum(list => list.Count);
            int aliveWeight = activeSpawnedObjects.Values.Sum(list => list.Sum(s => s?.Weight ?? 0));
            deadCount = totalCount - aliveCount;
            deadWeight = totalWeight - aliveWeight;
        }

        if (progressType == ProgressType.Count)
            return totalCount == 0 ? 1f : (float)deadCount / totalCount;
        else
            return totalWeight == 0 ? 1f : (float)deadWeight / totalWeight;
    }

    public void Spawn(ISpawnble[] spawnbles)
    {
        if (spawnbles == null) return;
        foreach (var item in spawnbles)
            Spawn(item);
    }

    public ISpawnble Spawn(ISpawnble spawnble)
    {
        if (spawnble?.GameObject == null) return null;

        Vector3 spawnPos = GetSpawnPoint();
        GameObject ngo = Instantiate(spawnble.GameObject, spawnPos, Quaternion.identity);

        if (spawnPreset != null && spawnPreset.randomRotation)
            ngo.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        ISpawnble spawn = ngo.GetComponent<ISpawnble>();
        if (spawn == null)
        {
            Debug.LogError($"[{name}] Spawned object {ngo.name} does not implement ISpawnble!");
            Destroy(ngo);
            return null;
        }

        spawn.OnDead += OnSpawnDead;
        spawn.SpawnInit();

        if (!activeSpawnedObjects.ContainsKey(currentWaveIndex))
            activeSpawnedObjects[currentWaveIndex] = new List<ISpawnble>();
        activeSpawnedObjects[currentWaveIndex].Add(spawn);

        OnSpawn?.Invoke(spawn);

        if (debugMode)
            Debug.Log($"[{name}] Spawned {ngo.name} at wave {currentWaveIndex}");

        return spawn;
    }

    private void OnSpawnDead(ISpawnble spawnble)
    {
        if (spawnble == null) return;
        spawnble.OnDead -= OnSpawnDead;

        foreach (var kvp in activeSpawnedObjects)
        {
            if (kvp.Value.Remove(spawnble))
            {
                if (debugMode)
                    Debug.Log($"[{name}] Enemy died, remaining in wave {kvp.Key}: {kvp.Value.Count}");
                break;
            }
        }
    }

    public Vector3 GetSpawnPoint()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            return point.position;
        }
        return GetRandomPoint();
    }

    public Vector3 GetRandomPoint()
    {
        Vector3 basePos = useLocalPosition ? transform.position : Vector3.zero;
        float horiz = Random.Range(0, 2) == 0 ? -distAtCenter : distAtCenter;
        float vert = Random.Range(-distYMax, distYMax);
        return basePos + new Vector3(horiz, vert, 0);
    }

    public void ClearAllSpawned()
    {
        foreach (var list in activeSpawnedObjects.Values)
        {
            foreach (var spawn in list)
            {
                if (spawn?.GameObject != null)
                {
                    spawn.OnDead -= OnSpawnDead;
                    Destroy(spawn.GameObject);
                }
            }
        }
        activeSpawnedObjects.Clear();
        waveInfoCache.Clear();
        waveSpawnablesCache.Clear();
    }

    public void ClearCurrentWave()
    {
        if (activeSpawnedObjects.TryGetValue(currentWaveIndex, out var list))
        {
            foreach (var spawn in list)
            {
                if (spawn?.GameObject != null)
                {
                    spawn.OnDead -= OnSpawnDead;
                    Destroy(spawn.GameObject);
                }
            }
            list.Clear();
        }
    }

    public void SkipToNextWave()
    {
        ClearCurrentWave();
        // Корутина продолжит сама
    }

    public void SetDifficultyScale(float scale)
    {
        globalDifficultyScale = Mathf.Clamp(scale, 0.1f, 3f);
    }

    public string GetPresetInfo()
    {
        if (spawnPreset == null) return "No currentPreset";
        return $"{spawnPreset.PresetName}: {spawnPreset.GetTotalEnemyCount()} enemies, {spawnPreset.GetTotalWeight()} weight, {TotalWaves} waves";
    }

    private bool IsPause()
    {
        // Предполагается, что где-то есть статический класс GamePause с полем IsPause
        return GamePause.IsPause;
    }

    private void OnDrawGizmosSelected()
    {
        if (!useLocalPosition)
        {
            Gizmos.color = Color.green;
            Vector3 center = transform.position;
            Vector3 left = center + Vector3.left * distAtCenter;
            Vector3 right = center + Vector3.right * distAtCenter;
            Gizmos.DrawLine(left + Vector3.up * distYMax, left + Vector3.down * distYMax);
            Gizmos.DrawLine(right + Vector3.up * distYMax, right + Vector3.down * distYMax);
            Gizmos.DrawLine(left + Vector3.up * distYMax, right + Vector3.up * distYMax);
            Gizmos.DrawLine(left + Vector3.down * distYMax, right + Vector3.down * distYMax);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(center, 0.2f);
        }

        if (spawnPoints != null)
        {
            Gizmos.color = Color.cyan;
            foreach (var p in spawnPoints)
                if (p != null) Gizmos.DrawSphere(p.position, 0.3f);
        }
    }
}

// ==================== Заглушка для GamePause (если нет) ====================
// Если у вас нет такого класса, раскомментируйте:
/*
public static class GamePause
{
    public static bool IsPause = false;
}
*/