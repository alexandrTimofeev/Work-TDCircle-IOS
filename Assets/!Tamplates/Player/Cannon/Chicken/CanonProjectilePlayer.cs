using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class CanonProjectilePlayer : MonoBehaviour
{
    [SerializeField] private IntContainer jumpsCount = new IntContainer(6, new Vector2Int(0, 6));
    public int JumpsCount => jumpsCount.Value;
    public int JumpsCountMax => jumpsCount.ClampRange.y;

    [SerializeField] private Vector2 forceJump = new Vector2(0f, 15f);
    [SerializeField] private Vector2 batutForceJump = new Vector2(0f, 15f);

    [Space]
    [SerializeField] private DamageHurtBox2D conectCollider;

    [Space]
    [SerializeField] private GameObject vfxDrop;
    [SerializeField] private GameObject vfxJump;

    private Rigidbody2D rigidbody2D;
    private IInput input;
    private Vector2 pointStart;
    private static float dist;
    public float Dist => dist;

    public event Action OnJump;
    public event Action OnDrop;
    public event Action<float> OnDistChange;
    public event Action<DamageHitBox2D> OnConectCollider;
    public event Action<int> OnJumpChange;
    public event Action<Vector2> OnMove;

    public void Init(IInput input)
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        pointStart = transform.position;

        this.input = input;
        input.OnBegan += Jump;

        //conectCollider.OnDamage += DamageOn;
        jumpsCount.OnChangeValue += (j) => OnJumpChange.Invoke(j);
    }

    public void AddJumps (int add)
    {
        jumpsCount.AddValue(add);
    }

    public void GetJumps(int remove)
    {
        jumpsCount.RemoveValue(remove);
    }

    private void Jump()
    {
        Jump(Vector2.zero);
    }

    private void Jump (Vector2 point)
    {
        if (jumpsCount.Value <= 0)
            return;

        GetJumps(1);
        rigidbody2D.linearVelocity = new Vector2(rigidbody2D.linearVelocity.x, 0f);
        rigidbody2D.AddForce(forceJump, ForceMode2D.Impulse);
        Destroy(Instantiate(vfxJump, transform.position, transform.rotation), 5f);

        OnJump?.Invoke();
    }

    private void DamageOn(DamageHitBox2D damageContainer)
    {
        if (damageContainer.ContainsAnyTags("Batut"))
        {
            rigidbody2D.linearVelocity = new Vector2(rigidbody2D.linearVelocity.x, 0f);
            rigidbody2D.AddForce(batutForceJump, ForceMode2D.Impulse);
        }

        if (damageContainer.ContainsAnyTags("Spike"))
        {
            Drop();
        }

        OnConectCollider?.Invoke(damageContainer);
    }

    private void Update()
    {
        dist = Mathf.Abs(transform.position.x - pointStart.x);

        OnDistChange?.Invoke(Dist);
        if (rigidbody2D.linearVelocity.x <= 6f)
        {
            if (rigidbody2D.linearVelocity.x < 2f)
                rigidbody2D.linearVelocity = new Vector2(2f, rigidbody2D.linearVelocity.y);
            rigidbody2D.AddForce(Vector2.right * 8f * Time.deltaTime, ForceMode2D.Impulse);
        }

        OnMove?.Invoke(transform.position);
    }

    private void OnDestroy()
    {
        input.OnBegan -= Jump;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*if (collision.TryGetComponent(out DownCollider downCollider))
        {
            Drop();
        }*/
    }

    private void Drop()
    {
        OnDrop?.Invoke();
        Destroy(Instantiate(vfxDrop, transform.position, transform.rotation), 10f);
        Destroy(gameObject);
    }

    public void NotFall(float timer)
    {
        rigidbody2D.gravityScale = 0f;
        GetComponent<ConstantForce2D>().force = Vector2.zero;
        rigidbody2D.linearVelocity = new Vector2(rigidbody2D.linearVelocity.x, 0f);

        DOVirtual.DelayedCall(timer, () =>
        {
            rigidbody2D.gravityScale = 1f;
            GetComponent<ConstantForce2D>().force = new Vector2(0, -5f);
            if (rigidbody2D.linearVelocity.x < 15f)
            {
                rigidbody2D.AddForce(Vector3.right * 15f, ForceMode2D.Impulse);
            }
        });
    }
}
