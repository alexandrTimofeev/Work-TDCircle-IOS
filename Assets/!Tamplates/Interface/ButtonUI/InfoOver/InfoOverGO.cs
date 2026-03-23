using System;
using System.Collections;
using UnityEngine;

public class InfoOverGO : InfoOver, IOverable3D
{
    public event Action<IClickable3D> OnOver;
    public event Action<IClickable3D> OnUnOver;

    public bool IsEnable()
    {
        return true;
    }

    public void Over()
    {
        OverInfo(Camera.main.WorldToScreenPoint(transform.position));
    }

    public void UnOver()
    {
        UnOverInfo();
    }
}