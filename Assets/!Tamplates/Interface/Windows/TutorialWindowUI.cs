using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialWindowUI : WindowUI
{
    private IInput input;

    public void Init(IInput input)
    {
        this.input = input;
        input.OnEnded += Click;
        OnClose += UnSub;
    }

    private void Click(Vector2 obj)
    {
        Close();
    }

    private void UnSub(WindowUI window)
    {
        input.OnEnded -= Click;
    }
}
