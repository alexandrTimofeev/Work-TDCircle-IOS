using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamageHurtBox3D : MonoBehaviour
{
    [SerializeField] private List<string> colliderTags = new List<string>();

    [Space]
    [SerializeField] private bool deliteIsHit;
    [SerializeField] private GameObject deliteMain;

    [Space]
    [SerializeField] private GameObject hitVFX;

    public event Action<DamageHitBox3D> OnDamage;

    public List<string> ColliderTags => colliderTags;

    private void OnTriggerEnter(Collider collision)
    {
        if(enabled == false)
            return;

        if (collision.TryGetComponent(out DamageHitBox3D damageContainer))
        {
            damageContainer.CollisionDamageCollider(this);

            if (damageContainer.ContainsAnyTags(colliderTags.ToArray()))
            {
                Vector3 point = collision.ClosestPoint(transform.position);
                Hit(damageContainer, point);
            }
        }
    }

    private void Hit(DamageHitBox3D damageContainer, Vector3 point)
    {
        if (enabled == false)
            return;

        damageContainer.HitDamageCollider(this);
        OnDamage?.Invoke(damageContainer);

        if (hitVFX != null)
            Destroy(Instantiate(hitVFX, point, transform.rotation), 10f);
        
        if(deliteIsHit)
            Destroy(deliteMain ? deliteMain : gameObject);
    }
}