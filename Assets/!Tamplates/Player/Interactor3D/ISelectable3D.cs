using System;
using System.Collections;
using UnityEngine;

public interface ISelectable3D : IClickable3D
{
    public event Action<SelectInfo> OnSelect;

    void Delite();
    public bool IsSelect();
    public void SetSelect(bool select);
}

public class SelectInfo
{
    public ISelectable3D Selectable;
    public bool IsSelect;
}