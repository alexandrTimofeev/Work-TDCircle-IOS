using System;
using UnityEngine;

[Serializable]
public class ReadOnlyWrapper<T> where T : class
{
    [SerializeField] private T target;

    public T Value => target;

    // Можно добавить здесь делегаты только для чтения, например, свойства Steps
}
