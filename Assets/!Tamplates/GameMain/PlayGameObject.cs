using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayGameObject : MonoBehaviour
{
    private List<IPlayGameBase> _playGameComponents = new List<IPlayGameBase>();

    private void Awake()
    {
        // Собираем все IPlayGameBase компоненты на объекте
        _playGameComponents = GetComponents<MonoBehaviour>()
            .OfType<IPlayGameBase>()
            .ToList();

        Register();

        //foreach (var pg in _playGameComponents)
            //Debug.Log($"[PlayGameObject] Register IPlayGameBase {((MonoBehaviour)pg).name}");
    }

    private void OnDestroy()
    {
        Unregister();
    }

    public void Register() => GameMain.AddPlayGames(this);
    public void Unregister() => GameMain.RemovePlayGames(this);

    // Возвращаем все IPlayGameBase интерфейсы компонента
    public IPlayGame<T>[] GetPlayGameInterfaces<T>() where T : class, IPlayGameBase
    {
        List<IPlayGame<T>> result = new List<IPlayGame<T>>();
        foreach (var pg in _playGameComponents)
        {
            IPlayGame<T> update = (pg as IPlayGame<T>);
            if (update != null)
                result.Add(update);
        }
        return result.ToArray();
    }

    public IPlayGameUpdate<T>[] GetPlayGameUpdateInterfaces<T> () where T : IPlayGameBase
    {
        List<IPlayGameUpdate<T>> result = new List<IPlayGameUpdate<T>>(); 
        foreach (var pg in _playGameComponents)
        {
            IPlayGameUpdate<T> update = (pg as IPlayGameUpdate<T>);
            if(update != null)
                result.Add(update);
        }
        return result.ToArray();
    }
}