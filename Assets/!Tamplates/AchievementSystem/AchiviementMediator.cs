using System;
using System.Collections.Generic;
using UnityEngine;

/// Медиатор
public class AchiviementMediator
{
    private readonly Dictionary<Type, IAchiviementObserver> achiviementObservers = new();

    private AchiviementObserver<T> GetOrCreateObserver<T>()
    {
        var type = typeof(T);
        if (!achiviementObservers.TryGetValue(type, out var obs))
        {
            var newObs = new AchiviementObserver<T>();
            achiviementObservers[type] = newObs;
            return newObs;
        }
        return (AchiviementObserver<T>)obs;
    }

    /// Добавление одиночного значения
    public void AddAchiviementForEndLevel<T>(string title, T state, Action<T> action = null)
    {
        GetOrCreateObserver<T>().AddAchiviementForEndLevel(title, state, action);
    }

    /// Добавление списка
    public void AddAchiviementForEndLevel<T>(string title, Action<List<T>> action)
    {
        GetOrCreateObserver<T>().AddAchiviementForEndLevel(title, action);
    }

    public void ChangeStateAchiviementForEndLevel<T>(string title, T state)
    {
        GetOrCreateObserver<T>().ChangeStateAchiviementForEndLevel(title, state);
    }

    public void AddActionForEndLevel<T>(string title, Action<T> action)
    {
        GetOrCreateObserver<T>().AddActionForEndLevel(title, action);
    }

    public void AddInList<T>(string title, T add, bool useCopy = true)
    {
        GetOrCreateObserver<T>().AddInList(title, add, useCopy);
    }

    public void RemoveOutList<T>(string title, T add, bool removeAll = true)
    {
        GetOrCreateObserver<T>().RemoveOutList(title, add, removeAll);
    }

    public IReadOnlyList<T> GetList<T>(string title)
    {
        return GetOrCreateObserver<T>().GetList(title);
    }

    public void InvokeEndLevel()
    {
        foreach (var observer in achiviementObservers.Values)
            observer.InvokeEndLevel();
    }
}

/// Интерфейс для универсального вызова InvokeEndLevel()
public interface IAchiviementObserver
{
    void InvokeEndLevel();
}

/// Универсальный наблюдатель для достижений
public class AchiviementObserver<T> : IAchiviementObserver
{
    private readonly Dictionary<string, T> achivForEndLvl = new();
    private readonly Dictionary<string, List<T>> achivLists = new();
    private readonly Dictionary<string, Action<T>> actionForEndLvl = new();
    private readonly Dictionary<string, Action<List<T>>> actionListForEndLvl = new();

    /// Для одиночного значения
    public void AddAchiviementForEndLevel(string title, T state, Action<T> action = null)
    {
        achivForEndLvl[title] = state;
        if (action != null)
            actionForEndLvl[title] = action;
    }

    /// Для списка
    public void AddAchiviementForEndLevel(string title, Action<List<T>> action)
    {
        if (action != null)
            actionListForEndLvl[title] = action;
    }

    public void ChangeStateAchiviementForEndLevel(string title, T state)
    {
        Debug.Log($"ChangeStateAchiviementForEndLevel {title} an {state}");
        if (achivForEndLvl.ContainsKey(title))
            achivForEndLvl[title] = state;
    }

    public void AddActionForEndLevel(string title, Action<T> action)
    {
        if (action != null)
            actionForEndLvl[title] = action;
    }

    public void AddActionListForEndLevel(string title, Action<List<T>> action)
    {
        if (action != null)
            actionListForEndLvl[title] = action;
    }

    /// Добавление элемента в список
    public void AddInList(string title, T add, bool useCopy)
    {
        if (!achivLists.TryGetValue(title, out var list))
        {
            list = new List<T>();
            achivLists[title] = list;
        }
        if (useCopy || list.Contains(add) == false)
            list.Add(add);
    }

    public void RemoveOutList(string title, T remove, bool removeAll)
    {
        if (!achivLists.TryGetValue(title, out var list))
        {
            list = new List<T>();
            achivLists[title] = list;
        }
        if (list.Contains(remove))
        {
            if (removeAll)            
                list.RemoveAll((x) => x.Equals(remove));            
            else            
                list.Remove(remove);            
        }
    }

    public IReadOnlyList<T> GetList(string title)
    {
        if (achivLists.TryGetValue(title, out var list))
            return list;
        return Array.Empty<T>();
    }

    public void InvokeEndLevel()
    {
        foreach (var kvp in actionForEndLvl)
        {
            if (achivForEndLvl.TryGetValue(kvp.Key, out var state))
            {
                Debug.Log($"Achiviement {kvp.Key} {state}");
                kvp.Value?.Invoke(state);
            }
            else
                Debug.Log($"Achiviement {kvp.Key} test is empty");
        }

        EndLevelForList();
    }

    private void EndLevelForList()
    {
        foreach (var kvp in actionListForEndLvl)
        {
            if (achivLists.TryGetValue(kvp.Key, out var state))
            {
                Debug.Log($"Achiviement {kvp.Key} {state.Count}");
                kvp.Value?.Invoke(state);
            }
            else
            {
                Debug.Log($"Achiviement {kvp.Key} {0}");
                kvp.Value?.Invoke(new List<T>());
            }
        }
    }
}