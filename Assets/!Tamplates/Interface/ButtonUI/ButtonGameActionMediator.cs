using System;
using UnityEngine;
using UnityEngine.Events;

public class ButtonGameActionMediator
{
    public Action<ButtonGameActionInfo> OnClick;

    public void Invoke(ButtonGameActionInfo actionInfo)
    {
        OnClick?.Invoke(actionInfo);    
    }
}
