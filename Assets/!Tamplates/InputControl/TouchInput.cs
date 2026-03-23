using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static IInput;

/// <summary>
/// Мобильный ввод через сенсорный экран.
/// </summary>
public class TouchInput : MonoBehaviour, IInput
{
    public event Action<Vector2> OnBegan;
    public event Action<InputMoveScreenInfo> OnMoved;
    public event Action<Vector2> OnEnded;
    public event Action<Vector2> OnClickAnyPosition;
    public event Action OnClickJump;
    public event Action OnClickEscape;

    void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            // Игнорировать, если палец над UI
            if (GetOverGameObjectUI() != null)
                continue;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    OnBegan?.Invoke(touch.position);
                    break;

                case TouchPhase.Moved:
                    // touch.deltaPosition — это смещение с прошлого кадра
                    if (touch.deltaPosition != Vector2.zero)
                        OnMoved?.Invoke(new InputMoveScreenInfo(touch.position, touch.deltaPosition));
                    break;

                case TouchPhase.Ended:
                    OnEnded?.Invoke(touch.position);
                    break;

                // Canceled зафиксируем тоже как окончание
                case TouchPhase.Canceled:
                    OnEnded?.Invoke(touch.position);
                    break;
            }

            OnClickAnyPosition?.Invoke(touch.position);
        }

    }

    public Vector2 GetOverPosition() => Input.touchCount > 0 ? Input.GetTouch(0).position : Vector2.zero;

    public void SetActive(bool active)
    {
        throw new NotImplementedException();
    }

    public GameObject GetOverGameObjectUI()
    {
        return EventSystem.current.currentSelectedGameObject;
    }
}
