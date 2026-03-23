using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class Player2DAction<C> : Player2DActionBase where C : Controller2DAction
{
    [SerializeField] protected C playerController;
    [SerializeField] protected PlayerVisual playerVisual;
    [SerializeField] protected DamageHurtBox2D damageCollider;
    [SerializeField] protected GrapCollider grapCollider;

    [Space]
    [SerializeField] protected float InvictibleDelay = 1f;
    [SerializeField] protected GameObject invictibleVFX;
    [SerializeField] protected GameObject slowMoVFX;
    [SerializeField] protected GameObject atGroundVFXPref;

    private Tween invictibleTween;
    private Tween addSizeTween;
    private bool isInvictible;
    private float speedStart;
    private Vector2 distantion;

    public override event Action<DamageContext2D> OnDamage;
    public override event Action OnGroundTick;
    public override event Action<Vector2> OnDistChange;
    public override event Action OnTriggerFinal;

    public override void Init(IInput input)
    {
        playerController.Init(input);

        if(damageCollider)
            damageCollider.OnDamage += DamageOn;
    }

    public override void InfinityFly(float duration)
    {
        playerController.InfinityFly(duration);
    }

    public override void SetSpeedKof(float speed)
    {
        throw new NotImplementedException();
    }

    public override void SlowMotion(float duration)
    {
        StartCoroutine(SlowMotionRountine(duration));
    }

    private IEnumerator SlowMotionRountine(float duration)
    {
        Time.timeScale = 0.3f;
        float timer = duration;
        if(slowMoVFX) slowMoVFX.SetActive(true);
        while (timer > 0)
        {
            if (GamePause.IsPause == false)
                timer -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        Time.timeScale = 1f;
        if (slowMoVFX) slowMoVFX.SetActive(false);
    }

    private void DamageOn(DamageContext2D context)
    {
        if (isInvictible)
            return;

        Invictible();
        if (GameSettings.IsVibrationPlay)
            Handheld.Vibrate();
        OnDamage?.Invoke(context);
    }

    public void Invictible()
    {
        Invictible(InvictibleDelay);
    }

    public override void Invictible(float delay)
    {
        if (invictibleTween != null)
            invictibleTween.Kill(true);

        damageCollider.gameObject.SetActive(false);
        if (invictibleVFX) invictibleVFX?.SetActive(true);
        playerVisual.StartFlicker();
        isInvictible = true;
        invictibleTween = DOVirtual.DelayedCall(delay, () =>
        {
            damageCollider.gameObject.SetActive(true);
            if(invictibleVFX) invictibleVFX?.SetActive(false);
            playerVisual.StopFlicker();
            isInvictible = false;
        });
    }

    public override void AddSize(float addSize, float duration)
    {
        if (addSizeTween != null)
            addSizeTween.Kill(true);

        AddSize(addSize);
        //playerVisual.StartFlicker();
        invictibleTween = DOVirtual.DelayedCall(duration, () =>
        {
            RemoveSize(addSize);
        });
    }

    protected virtual void AddSize(float addSize)
    {
        throw new NotImplementedException();
    }

    protected virtual void RemoveSize(float removeSize)
    {
        throw new NotImplementedException();
    }

    public override void Reload(Transform checkpoint)
    {
        playerController.StopImmidiatly();
        playerController.transform.position = checkpoint.position;
        //playerController.transform.rotation = checkpoint.rotation;
    }

    public void DistUpdate(Vector3 posStart, Vector3? playerPos = null)
    {
        if (playerPos == null)
            playerPos = transform.position;
        distantion = new Vector2(playerPos.Value.x - posStart.x, playerPos.Value.y - posStart.y);
        OnDistChange?.Invoke(new Vector2(playerPos.Value.x - posStart.x, playerPos.Value.y - posStart.y));
    }

    public override Vector2 GetDist() => distantion;

    public override void Dead()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Final")
        {
            OnTriggerFinal?.Invoke();
        }
    }
}

public abstract class Player2DActionBase : MonoBehaviour
{
    public abstract event Action<DamageContext2D> OnDamage;
    public abstract event Action OnGroundTick;
    public abstract event Action<Vector2> OnDistChange;
    public abstract event Action OnTriggerFinal;

    public abstract void Init(IInput input);

    public abstract void SetSpeedKof(float speed);

    public abstract void SlowMotion(float duration);

    public abstract void InfinityFly(float duration);

    public abstract Vector2 GetDist();

    public abstract void Invictible(float delay);

    public abstract void Reload(Transform checkpoint);

    public abstract void AddSize(float addSize, float duration);

    public virtual Transform GetTarget() => transform;

    public abstract void Dead();
}