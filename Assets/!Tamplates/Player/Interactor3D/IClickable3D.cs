using System;
using System.Collections;
using UnityEngine;

public interface IClickable3D : IOverable3D
{
    public event Action<IClickable3D> OnClick;

    public void Click();
}