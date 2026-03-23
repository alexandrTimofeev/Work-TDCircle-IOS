using System;
using UnityEngine;

public class InterObClick : InterObOver, IClickable3D
{
    public event Action<IClickable3D> OnClick;

    public virtual void Click()
    {
        OnClick?.Invoke(this);
    }
}