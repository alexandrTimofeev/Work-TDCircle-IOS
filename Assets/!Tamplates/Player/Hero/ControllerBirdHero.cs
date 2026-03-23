using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ControllerBirdHero : Controller2DAction
{
    [Space, Header("BirdHero")]
    [SerializeField] private float forceDash = 5f;
    [SerializeField] private float durationDash = 0.5f;
    [SerializeField] private GameObject canDashFX;
    [SerializeField] private GameObject dashFX;

    private bool isDash;
    private bool canDash = false;

    public override void Update()
    {
        base.Update();

        if (isGrounded)
        {
            Jump();
            if (isDash == false)
                ReloadDash();         
        }

        transform.rotation = Quaternion.identity;
    }

    protected override void EndedOn(Vector2 point)
    {
        base.EndedOn(point);

        DashRight();
    }

    private void DashRight()
    {
        if (isDash || !canDash)
            return;

        SetCanDash(false);
        //StartCoroutine(CanDashReload());
        StartCoroutine(DashRightRoutine());
        Destroy(Instantiate(dashFX, transform.position, transform.rotation), 10f);        
    }

    private void SetCanDash (bool can)
    {
        canDash = can;
        canDashFX?.SetActive(can);
    }

    private IEnumerator DashRightRoutine ()
    {
        isDash = true;
        GetComponent<Rigidbody2D>().AddForce(Vector2.right * forceDash, ForceMode2D.Impulse);
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY;

        yield return new WaitForSeconds(durationDash);

        EndDash();
    }

    private IEnumerator CanDashReload ()
    {
        yield return new WaitForSeconds(0.5f);
        yield return new WaitWhile(() => isDash);
        ReloadDash();
    }

    public void ReloadDash()
    {
        //StopDash();
        StopCoroutine(CanDashReload());
        SetCanDash(true);
    }

    public override void Push(Vector2 force)
    {
        StopDash();
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        //base.Push(new Vector2(force.x, force.y));
        base.Push(force);
    }

    private void StopDash ()
    {
        StopCoroutine(DashRightRoutine());
        EndDash();
    }

    private void EndDash()
    {
        GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0f, GetComponent<Rigidbody2D>().linearVelocity.y);
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        isDash = false;
    }
}
