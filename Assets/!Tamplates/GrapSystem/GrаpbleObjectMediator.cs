using System;
using System.Collections.Generic;

/// <summary>
/// Посредник для рассылки событий о конкретных GrappableActionContainer,
/// хранит подписчиков в Dictionary<Type, List<Delegate>>.
/// </summary>
public class GrappableObjectMediator
{
    // Словарь: ключ — конкретный Type (наследник GrappableActionContainer),
    // значение — список Delegate (ActionAtTaste<T>) для этого типа.
    private readonly Dictionary<Type, List<Delegate>> _subscribers
        = new Dictionary<Type, List<Delegate>>();

    /// <summary>
    /// Подписаться на события конкретного типа T.
    /// </summary>
    public void Subscribe<T>(Action<T, GrapObject> handler) where T : GrappableObjectAction
    {
        var type = typeof(T);
        if (!_subscribers.TryGetValue(type, out var list))
        {
            list = new List<Delegate>();
            _subscribers[type] = list;
        }

        if (!list.Contains(handler))
            list.Add(handler);
    }

    /// <summary>
    /// Отписаться от событий конкретного типа T.
    /// </summary>
    public void Unsubscribe<T>(Action<T, GrapObject> handler) where T : GrappableObjectAction
    {
        var type = typeof(T);
        if (!_subscribers.TryGetValue(type, out var list)) return;

        list.Remove(handler);
        if (list.Count == 0)
            _subscribers.Remove(type);
    }

    public void Invoke(GrapObject grapObject)
    {
        Invoke(grapObject.Behaviour, grapObject);
    }

    public void Invoke(GrappableActionContainer obj, GrapObject grapObject)
    {
        foreach (var behaviourAction in obj.commands.actions)
        {
            Invoke(behaviourAction, grapObject);
        }
    }

    /// <summary>
    /// Вызывает событие для всех подписчиков точного типа переданного объекта.
    /// </summary>
    public void Invoke(GrappableObjectAction obj, GrapObject grapObject)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        var type = obj.GetType();
        if (!_subscribers.TryGetValue(type, out var list)) return;

        // Копируем, чтобы безопасно вызывать даже при изменении списка
        var toInvoke = list.ToArray();
        foreach (var del in toInvoke)
        {
            // делегат точно был добавлен как ActionAtTaste<T>, где T == type
            // можно вызвать через DynamicInvoke
            del.DynamicInvoke(obj, grapObject);
        }
    }
}