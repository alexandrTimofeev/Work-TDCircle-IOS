using System;
using UnityEngine;

public class ControllerPlayerEscape : Controller2DAction
{
    [Space]
    [SerializeField] private Vector2 speedMove = Vector2.right;
    private float coefSpeed = 1f;
    [SerializeField] private float upForce = 1f;
    [SerializeField] private float maxY = 10f;

    [Space]
    [SerializeField] private bool isUseFlyFuel = true;
    [SerializeField] private float fuelСonsumption = 35f;
    [SerializeField] private float fuelRegeneration = 35f;
    [SerializeField] private float fuelRegenerationGroundCoef = 1f;
    [SerializeField] private string barUIFuelName;
    private float fuel = 100f;
    private bool isBlockFly;
    public bool IsCanFly => isUseFlyFuel ? (isBlockFly ? false : (fuel > 0f)) : true;

    [Space]
    [SerializeField] private GameObject flyVFX;

    public event Action<float> OnChangeFuel;

    public override void Init(IInput input)
    {
        base.Init(input);

        if (isUseFlyFuel)
        {
            InterfaceManager.BarMediator.SetMaxForID(barUIFuelName, 100f);
            OnChangeFuel += (v) => InterfaceManager.BarMediator.ShowForID(barUIFuelName, v);
        }
    }

    protected override void BeganOn(Vector2 point)
    {
        base.BeganOn(point);
        if (IsCanFly)
        {
            StartFly();
        }
    }

    protected override void EndedOn(Vector2 point)
    {
        base.EndedOn(point);
        StopFly();
    }

    public override void Update()
    {
        base.Update();
        UpdateMove();
    }

    private void UpdateMove()
    {
        if (GamePause.IsPause)
            return;

        transform.position += (Vector3)speedMove * coefSpeed * Time.deltaTime;
        if (isHoldButton)
        {
            if (IsCanFly)
            {
                FlyUp();
                fuel -= fuelСonsumption * Time.deltaTime;

                if (fuel <= 0f)
                {
                    StopFly();
                }

                OnChangeFuel?.Invoke(fuel);
            }

            if (isUseFlyFuel && fuel <= 0f && isGrounded == false && isBlockFly == false)
            {
                isBlockFly = true;
            }
        }
        else
        {
            if (isBlockFly == false || isGrounded)
            {
                fuel += fuelRegeneration * (isGrounded ? fuelRegenerationGroundCoef : 1f) * Time.deltaTime;
                fuel = Mathf.Clamp(fuel, 0f, 100f);
                OnChangeFuel?.Invoke(fuel);
            }
        }

        if (isGrounded && isBlockFly && fuel > 10f)
        {
            isBlockFly = false;
        }
    }

    private void FlyUp()
    {
        if (GamePause.IsPause)
            return;

        transform.position = new Vector3(transform.position.x,
            Mathf.Clamp(transform.position.y + (upForce * Time.deltaTime), -1000, maxY),
            transform.position.z);
    }

    private void StartFly()
    {
        Rigidbody2D.gravityScale = 0f;
        Rigidbody2D.linearVelocity = Vector2.zero;
        flyVFX.SetActive(true);
    }

    private void StopFly()
    {
        Rigidbody2D.gravityScale = 1f;
        flyVFX.SetActive(false);
    }

    public void SetCoefSpeed (float coef)
    {
        coefSpeed = coef * 1.5f;
    }
}