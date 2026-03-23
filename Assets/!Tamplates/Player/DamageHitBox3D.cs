using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

//[RequireComponent(typeof(Collider))]
public class DamageHitBox3D : MonoBehaviour
{
    [SerializeField] private List<string> colliderTargetTags = new List<string>();
    [SerializeField] private List<string> destroyMeTags = new List<string>();
    [SerializeField] private bool deleteIfCollisionTarget = true;

    [Space]
    [SerializeField] private bool destroyGOForDelete = true;
    [SerializeField] private GameObject deliteMain;
    [SerializeField] private GameObject deliteVFX;
    [SerializeField] private UnityEvent<DamageHitBox3D> OnDeliteEv;

    public event Action<DamageHurtBox3D> OnCollisionDamageCollider;
    public event Action<DamageHurtBox3D> OnHitDamageCollider;
    public event Action<DamageHitBox3D> OnDelete;

    public void CollisionDamageCollider(DamageHurtBox3D damageCollider)
    {
        OnCollisionDamageCollider?.Invoke(damageCollider);

        if (destroyMeTags.Any(tag => damageCollider.ColliderTags.Contains(tag)))
            Delete();
    }

    public void HitDamageCollider(DamageHurtBox3D damageCollider)
    {
        OnHitDamageCollider?.Invoke(damageCollider);

        if (deleteIfCollisionTarget)
            Delete();
    }

    private void Delete()
    {
        OnDelete?.Invoke(this);
        OnDeliteEv?.Invoke(this);

        if (deliteVFX)
            Instantiate(deliteVFX, transform.position, Quaternion.identity);

        if (destroyGOForDelete)
        {
            Destroy(gameObject);
            if(deliteMain)
                Destroy(deliteMain);
        }
    }

    public bool ContainsAnyTags(params string[] tags)
    {
        return colliderTargetTags.Any(tag => tags.Contains(tag));
    }
}