using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerControllerChR : MonoBehaviour
{
    [SerializeField] private Vector2 speedMove = new Vector2(0, 15f);
    [SerializeField] private Vector2 clampMaxPosition = new Vector2(0, 0);
    [SerializeField] private Vector2 clampMinPosition = new Vector2(0, 0);

    private bool isMove;
    private IInput input;

    public Action OnMove;
    public Action OnStop;

    public void Init(IInput input)
    {
        this.input = input;

        this.input.OnMoved += OnPointerMoved;
    }

    private void OnPointerMoved(IInput.InputMoveScreenInfo info)
    {
        if (GamePause.IsPause)
            return;

        Vector2 deltaMove = info.Delta;
        deltaMove /= Time.timeScale;
        transform.position += new Vector3(deltaMove.x * speedMove.x, deltaMove.y * speedMove.y, 0) * Time.deltaTime;
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, clampMinPosition.x, clampMaxPosition.x),
            Mathf.Clamp(transform.position.y, clampMinPosition.y, clampMaxPosition.y), 0);
        isMove = true;
    }

    private float stopInvokeTimer = 0.1f;
    private void Update()
    {
        if (isMove)
        {
            OnMove?.Invoke();
            isMove = false;
            stopInvokeTimer = 0.1f;
        }
        else
        {
            stopInvokeTimer -= Time.deltaTime;
            if (stopInvokeTimer < 0f)
            {
                OnStop?.Invoke();
                stopInvokeTimer = 0.1f;
            }
        }
    }
}
