using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DamageHurtBox2D : MonoBehaviour
{
    [SerializeField] private List<string> colliderTags = new List<string>();

    [Space]
    [SerializeField] private GameObject hitVFX;

    public event Action<DamageContext2D> OnDamage;

    public List<string> ColliderTags => colliderTags;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out DamageHitBox2D damageContriner))
        {
            damageContriner.CollisionDamageCollider(this);
            if (damageContriner.ContainsAnyTags(colliderTags.ToArray()))
            {
                Hit(damageContriner, collision.ClosestPoint(transform.position));
            }
        }
    }

    private void Hit(DamageHitBox2D damageContriner, Vector3 point)
    {
        damageContriner.HitDamageCollider(this);
        OnDamage?.Invoke(damageContriner.GetContext(this));

        if (hitVFX)
            Destroy(Instantiate(hitVFX, point, transform.rotation), 10f);
    }
}
