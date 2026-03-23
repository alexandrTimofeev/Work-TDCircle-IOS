using System;
using UnityEngine;

public class InterObSelect : InterObClick, ISelectable3D
{
    public event Action<SelectInfo> OnSelect;

    private bool isSelected;

    public bool IsSelect() => isSelected;

    public void SetSelect(bool select)
    {
        if (isSelected == select)
            return;

        isSelected = select;
        rend.material.color = select ? Color.green : baseColor;

        OnSelect?.Invoke(new SelectInfo
        {
            Selectable = this,
            IsSelect = select
        });
    }

    public void Delite()
    {
        Destroy(gameObject);
    }
}