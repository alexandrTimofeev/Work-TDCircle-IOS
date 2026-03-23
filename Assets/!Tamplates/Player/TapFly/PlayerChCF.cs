using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerChCF : MonoBehaviour
{
    [SerializeField] private PlayerControllerFly controllerFly;
    [SerializeField] private DamageHurtBox2D damageCollider;
    [SerializeField] private GrapCollider grapCollider;
    [SerializeField] private PlayerVisual playerVisual;

    [Space]
    [SerializeField] private float InvictibleDelay = 1f;
    [SerializeField] private float groundDealay = 0.5f;

    [Space]
    [SerializeField] private AudioSource sourcePlayer;
    [SerializeField] private AudioClip clipFly;
    [SerializeField] private GameObject vfxHit;
    [SerializeField] private GameObject vfxGrap;
    [SerializeField] private GameObject vfxGround;

    private IInput input;

    private Tween invictibleTween;
    private bool isInvictible;
    private Coroutine coroutineSlow;

    public Action<DamageHitBox2D> OnDamage;
    public Action<GrapObject> OnGrap;
    public Action<Vector2> OnMove;
    public Action<GameObject> OnGroundUpdate;
    private float groundTimer;

    //public DamageHurtBox2D PlayerDamageCollider => damageCollider;
    //public GrapCollider PlayerGrapCollider => grapCollider;

    public void Init (IInput input)
    {
        this.input = input;
        controllerFly.Init(input);
        controllerFly.OnMove += MoveOn;

        //damageCollider.OnDamage += Damage;
        grapCollider.OnGrap += GrapOn;

        StartCoroutine(GroundTimerCoroutine());
    }

    private void Damage(DamageHitBox2D damageContainer)
    {
        Invictible();
        playerVisual.PlayPunchScale();
        if(vfxHit)
            Destroy(Instantiate(vfxHit, transform.position, transform.rotation), 10f);
        if (GameSettings.IsVibrationPlay)
            Handheld.Vibrate();
        OnDamage?.Invoke(damageContainer);
    }
    private void GrapOn(GrapObject grapObject)
    {
        if(vfxGrap)
            Destroy(Instantiate(vfxGrap, grapObject.transform.position, grapObject.transform.rotation), 10f);
        OnGrap?.Invoke(grapObject);
    }

    public void Invictible()
    {
        Invictible(InvictibleDelay);
    }

    public void Invictible(float delay)
    {
        if (invictibleTween != null)        
            invictibleTween.Kill(true);        

        damageCollider.gameObject.SetActive(false);
        playerVisual.StartFlicker();
        isInvictible = true;
        invictibleTween = DOVirtual.DelayedCall(delay, () =>
        {
            damageCollider.gameObject.SetActive(true);
            playerVisual.StopFlicker();
            isInvictible = false;
        });
    }

    private void MoveOn(Vector2 move)
    {
        playerVisual.transform.localScale = new Vector3(Mathf.Sign(move.x),
            playerVisual.transform.localScale.y,
            playerVisual.transform.localScale.z);
        if (sourcePlayer)
        {
            sourcePlayer.pitch = UnityEngine.Random.Range(0.8f, 1.3f);
            sourcePlayer.PlayOneShot(clipFly);
        }
        OnMove?.Invoke(move);
    }

    public void SlowMotion(float duration)
    {
        if (coroutineSlow != null)
            StopCoroutine(coroutineSlow);
        coroutineSlow = StartCoroutine(SlowMotionRoutine(duration));
    }

    private IEnumerator SlowMotionRoutine(float duration)
    {
        Time.timeScale = 0.4f;

        float wait = duration;
        while (wait > 0f)
        {
            wait -= Time.deltaTime;

            yield return new WaitForEndOfFrame();

            while (GamePause.IsPause)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        Time.timeScale = 1f;
    }

    private IEnumerator GroundTimerCoroutine()
    {
        groundTimer = groundDealay;
        while (true)
        {
            yield return new WaitWhile(() => controllerFly.IsGrounded);
            while (controllerFly.IsGrounded)
            {
                yield return new WaitForEndOfFrame();
                groundTimer -= Time.deltaTime;
                if (groundTimer <= 0f)
                {
                    OnGroundUpdate?.Invoke(controllerFly.GroundObject.gameObject);
                    Destroy(Instantiate(vfxGround, transform.position + Vector3.down, transform.rotation), 10f);
                    groundTimer = groundDealay;
                }
            }
            groundTimer = groundDealay;
        }
    }

    public void SetSkin(Sprite sprite)
    {
        playerVisual.SetSkin(sprite);
    }
}
