using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Collider2D))]
public class DamageHitBox2D : MonoBehaviour
{
    [SerializeField] private List<string> colliderTargetTags = new List<string>();
    [SerializeField] private List<string> destoryMeTags = new List<string>();
    [SerializeField] private bool deliteIfCollisionTarget = true;

    [Space]
    [SerializeField] private bool destroyGOForDelite = true;
    [SerializeField] private GameObject rootForDelite;
    public GameObject RootForDelite => rootForDelite ? rootForDelite : gameObject;

    [Space, Header("VFX")]
    [SerializeField] private GameObject vfxWallHitPref;
    [SerializeField] private GameObject vfxTargetHitPref;

    [Space, Header("Context")]
    public DamageContext2D Context;


    public event Action<DamageContext2D> OnCollisionDamageCollider;
    public event Action<DamageContext2D> OnHitDamageCollider;
    public event Action<DamageHitBox2D> OnDelite;

    private void Awake()
    {
        Context.DamageHitBox = this;
    }

    public void CollisionDamageCollider(DamageHurtBox2D damageCollider)
    {
        OnCollisionDamageCollider?.Invoke(GetContext(damageCollider));
        if (destoryMeTags.Any((s) => damageCollider.ColliderTags.Contains(s)))
        {
            Delite();

            if (vfxWallHitPref)
                Destroy(Instantiate(vfxWallHitPref, RootForDelite.transform.position, RootForDelite.transform.rotation), 10f);
        }
    }

    public void HitDamageCollider(DamageHurtBox2D damageCollider)
    {
        OnHitDamageCollider?.Invoke(GetContext(damageCollider));

        if (deliteIfCollisionTarget)        
            Delite();

        if (vfxTargetHitPref)
            Destroy(Instantiate(vfxTargetHitPref, RootForDelite.transform.position, RootForDelite.transform.rotation), 10f);
    }

    private void Delite()
    {
        OnDelite?.Invoke(this);
        if (destroyGOForDelite)
        {
            Destroy(RootForDelite);
        }
    }

    public bool ContainsAnyTags(params string[] tags)
    {
        return colliderTargetTags.Any((s) => tags.Contains(s));
    }

    public DamageContext2D GetContext(DamageHurtBox2D damageCollider)
    {
        return Context.CloneForAction(this, damageCollider);
    }
}
