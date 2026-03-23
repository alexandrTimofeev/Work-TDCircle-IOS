using System;
using System.Collections;
using UnityEngine;

public class Controller2DAction : MonoBehaviour
{
    [SerializeField] protected Vector2 forceJump = new Vector2(0, 2f);
    protected virtual Vector2 ForceJump => forceJump;


    [SerializeField] protected float distDown = 0.5f;
    [SerializeField] private LayerMask layersGround;

    [Space]
    [SerializeField] private GameObject infinityFlyVFX;
    private float infinityFlyTimer = 0f;

    [Space]
    [SerializeField] private GameObject groundFlyVFXPref;
    [SerializeField] private GameObject stepVFX;

    [Space]
    [SerializeField] private float delayStepSound = 0.5f;
    [SerializeField] private AudioSource sourceStep;

    private new Rigidbody2D rigidbody2D;
    public Rigidbody2D Rigidbody2D => rigidbody2D;
    protected IInput input;
    protected bool isGrounded = true;
    protected bool isHoldButton;

    public virtual void Init(IInput input)
    {
        this.input = input;
        rigidbody2D = GetComponent<Rigidbody2D>();

        input.OnBegan += BeganOn;
        input.OnEnded += EndedOn;
        //input.OnMoved += MovedOn;
        input.OnClickAnyPosition += AnyPositionOn;

        StartCoroutine(StepSoundEn());
    }


    protected virtual void OnPointerMoved(IInput.InputMoveScreenInfo info)
    { }
    protected virtual void EndedOn(Vector2 point)
    {
        isHoldButton = false;
    }
    protected virtual void BeganOn(Vector2 point)
    {
        isHoldButton = true;
    }
    protected virtual void AnyPositionOn(Vector2 point)
    { }

    protected virtual void Jump()
    {
        if (isGrounded)
        {
            //isGrounded = false;
            Vector2 force = ForceJump;
            rigidbody2D.linearVelocity = Vector2.zero;
            rigidbody2D.AddForce(force, ForceMode2D.Impulse);
        }
    }

    public virtual void Update()
    {
        RaycastHit2D hit2D = Physics2D.CircleCast(transform.position, 0.15f, Vector2.down, distDown, layersGround);
        if (hit2D.transform != null)
        {
            if (isGrounded == false)
                DropGround(hit2D.point);
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        stepVFX.SetActive(isGrounded);
    }

    private void DropGround(Vector3 point)
    {
        if(groundFlyVFXPref)
            Destroy(Instantiate(groundFlyVFXPref, point, transform.rotation), 10f);        
    }

    public virtual void InfinityFly(float duration)
    {
        infinityFlyTimer = duration;
        if(infinityFlyVFX)
            infinityFlyVFX.SetActive(true);
    }

    public virtual void StopImmidiatly()
    {
        rigidbody2D.linearVelocity = Vector2.zero;
    }

    public virtual void Push(Vector2 force)
    {
        rigidbody2D.AddForce(force, ForceMode2D.Impulse);
    }

    private IEnumerator StepSoundEn ()
    {
        while (true)
        {
            yield return new WaitForSeconds(delayStepSound);
            if(isGrounded && sourceStep && GamePause.IsPause == false)
                sourceStep.Play();
        }
    }
}