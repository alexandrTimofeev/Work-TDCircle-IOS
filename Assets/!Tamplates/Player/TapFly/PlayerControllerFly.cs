using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerFly : MonoBehaviour
{
    [SerializeField] private float forceSide = 5f;
    [SerializeField] private float forceUp = 5f;
    private new Rigidbody2D rigidbody;

    [SerializeField] private LayerMask groundedLvl;

    [Space]
    [SerializeField] private GameObject vfxFly;
    [SerializeField] private GameObject vfxOnGround;

    public bool IsGrounded => GroundObject != null;
    public Transform GroundObject { get; private set; }

    public Action<Vector2> OnMove;
    public Action OnGroundUpdate;

    public void Init(IInput input)
    {
        input.OnBegan += ClickPoint;
        rigidbody = GetComponent<Rigidbody2D>();
        GamePause.OnPauseChange += SetPause;
    }

    private void Update()
    {
        bool isGroundPrev = IsGrounded;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.3f, Vector2.down, 1f, groundedLvl);
        GroundObject = hit.transform;

        if (isGroundPrev != IsGrounded)
        {
            if (IsGrounded && rigidbody.linearVelocity.y < 0f)
            {
                Destroy(Instantiate(vfxOnGround, transform.position + (Vector3.down * 1.5f), transform.rotation), 10f);
            }
        }

        if (IsGrounded)        
            OnGroundUpdate?.Invoke();        
    }

    private void ClickPoint(Vector2 point)
    {
        if (GamePause.IsPause)
            return;

        float directionSide = 0f;
        if (point.x < Screen.width / 2f)
            directionSide = -1f;
        else
            directionSide = 1f;

        Vector2 moveForce = new Vector2(directionSide * forceSide, forceUp);
        rigidbody.AddForce(moveForce, ForceMode2D.Impulse);
        Destroy(Instantiate(vfxFly, transform.position, transform.rotation), 10f);
        OnMove?.Invoke(moveForce);
    }

    private void SetPause(bool isPause)
    {
        rigidbody.simulated = !isPause;
    }

    private void OnDestroy()
    {
        GamePause.OnPauseChange -= SetPause;
    }
}
