using System;
using UnityEngine;

[Serializable]
public class ReadOnly<T> where T : class
{
    [SerializeField] private T target; // редактируем в Inspector

    public ReadOnly(T target)
    {
        this.target = target;
    }

    // Только getter, снаружи нельзя присвоить новый объект
    public T Value => target;
}
