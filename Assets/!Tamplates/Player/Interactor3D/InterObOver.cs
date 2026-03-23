using System;
using UnityEngine;

public class InterObOver : MonoBehaviour, IOverable3D
{
    public event Action<IClickable3D> OnOver;
    public event Action<IClickable3D> OnUnOver;

    protected Renderer rend;
    protected Color baseColor;

    protected virtual void Awake()
    {
        rend = GetComponent<Renderer>();
        baseColor = rend.material.color;
    }

    public virtual void Over()
    {
        rend.material.color = Color.yellow;
        OnOver?.Invoke(this as IClickable3D);
    }

    public virtual void UnOver()
    {
        rend.material.color = baseColor;
        OnUnOver?.Invoke(this as IClickable3D);
    }

    public bool IsEnable()
    {
        return true;
    }
}