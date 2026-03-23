using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public static class Utils
{
    private static readonly System.Random _random = new();

    public static T GetRandomValue<T>() where T : Enum
    {
        var values = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
        return values[_random.Next(values.Length)];
    }

    public static T GetRandomEnumValue<T>(this T _) where T : Enum
    {
        return GetRandomEnumValue<T>();
    }

    public static T GetRandomEnumValue<T>() where T : Enum
    {
        var values = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
        return values[_random.Next(values.Length)];
    }

    /// <summary>
    /// Глубокое клонирование объекта через бинарную сериализацию.
    /// Требует, чтобы T и все вложенные классы были [Serializable].
    /// </summary>
    public static T DeepCloneBinary<T>(T source)
    {
        if (source == null) return default;

        using (var ms = new MemoryStream())
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(ms, source);
            ms.Position = 0;
            return (T)formatter.Deserialize(ms);
        }
    }

    /// <summary>
    /// Глубокое клонирование объекта через JSON.
    /// Работает с [Serializable] классами Unity.
    /// Не клонирует UnityEngine.Object (GameObject, ScriptableObject и т.д.).
    /// </summary>
    public static T DeepCloneJson<T>(T source)
    {
        if (source == null) return default;

        string json = JsonUtility.ToJson(source);
        return JsonUtility.FromJson<T>(json);
    }

    public static IReadOnlyDictionary<TKey, TValue> ToReadOnlyDictionary<TKey, TValue>(
        TValue[] values,
        Func<TValue, TKey> keySelector)
    {
        if (values == null)
            throw new ArgumentNullException(nameof(values));
        if (keySelector == null)
            throw new ArgumentNullException(nameof(keySelector));

        var dict = new Dictionary<TKey, TValue>(values.Length);

        foreach (var value in values)
        {
            var key = keySelector(value);
            dict.Add(key, value);
        }

        return dict;
    }
}

/// <summary>
/// Простая очередь с приоритетом на основе List + сортировка.
/// Работает для A* алгоритма.
/// </summary>
public class SimplePriorityQueue<T>
{
    private readonly List<(T item, int priority)> _data = new();

    public int Count => _data.Count;

    public void Enqueue(T item, int priority)
    {
        _data.Add((item, priority));
        _data.Sort((a, b) => a.priority.CompareTo(b.priority));
    }

    public T Dequeue()
    {
        if (_data.Count == 0)
            throw new InvalidOperationException("Queue is empty");

        var item = _data[0].item;
        _data.RemoveAt(0);
        return item;
    }
}

public static class CoroutineHelper
{
    /// <summary>
    /// Универсальный запуск корутин для коллекции элементов.
    /// Каждая корутина запускается параллельно.
    /// Метод ждёт завершения всех.
    /// </summary>
    public static IEnumerator RunAll<T>(MonoBehaviour runner, IEnumerable<T> items, Func<T, IEnumerator> action)
    {
        List<IEnumerator> coroutines = new List<IEnumerator>();
        foreach (var item in items)
        {
            coroutines.Add(action(item));
        }

        yield return RunAll(runner, coroutines);
    }

    /// <summary>
    /// Запуск нескольких IEnumerator параллельно и ожидание завершения всех
    /// </summary>
    public static IEnumerator RunAll(MonoBehaviour runner, IEnumerable<IEnumerator> coroutines)
    {
        int remaining = 0;
        foreach (var co in coroutines)
        {
            remaining++;
            runner.StartCoroutine(RunSingle(co, () => remaining--));
        }

        while (remaining > 0)
            yield return null;
    }

    private static IEnumerator RunSingle(IEnumerator coroutine, Action onComplete)
    {
        yield return coroutine;
        onComplete?.Invoke();
    }
}
