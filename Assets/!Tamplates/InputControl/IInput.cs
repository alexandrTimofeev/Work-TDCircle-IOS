using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IInput
{
    public event Action<Vector2> OnBegan;
    public event Action<Vector2> OnEnded;
    public event Action<InputMoveScreenInfo> OnMoved;
    public event Action<Vector2> OnClickAnyPosition;
    public event Action OnClickJump;
    public event Action OnClickEscape;

    public Vector2 GetOverPosition();

    public void SetActive(bool active);
    
    public struct InputMoveScreenInfo
    {
        public Vector2 PositionOnScreen;
        public Vector2 Delta;

        public InputMoveScreenInfo(Vector2 positionOnScreen, Vector2 delta)
        {
            PositionOnScreen = positionOnScreen;
            Delta = delta;
        }

        public Vector2 StartPositionOnScreen => PositionOnScreen - Delta;
        public Vector2 DirectionMove => Delta.normalized;
    }

    public GameObject GetOverGameObjectUI();
}