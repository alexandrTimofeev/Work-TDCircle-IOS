using System;
using UnityEngine;

public class LifeBody : MonoBehaviour, IHealth
{
    [SerializeField] private IntContainer health;

    [Space]
    [SerializeField] private DamageHurtBox2D[] hurtBox2D;
    [SerializeField] private GameObject rootToDie;

    [Space]
    [SerializeField] private GameObject vfxDead;
    [SerializeField] private GameObject vfxHeal;

    public float CurrentHealth => health.Value;
    public float MaxHealth => health.ClampRange.y;
    
    public event Action OnDead;

    private void Awake()
    {
        foreach (var hurtBox in hurtBox2D)
        {
            hurtBox.OnDamage += DamageWork;
        }

        health.OnDownfullValue += DownfullWork;

        health.UpdateValue();
    }

    private void DamageWork(DamageContext2D context)
    {
        TakeDamage(context.Dmg);
    }

    public void TakeDamage(int damage)
    {
        health.RemoveValue(damage);
    }

    public void Heal(int addHealth)
    {
        health.AddValue(addHealth);

        if(vfxHeal)
            Destroy(Instantiate(vfxHeal, transform.position, Quaternion.identity), 10f);
    }

    private void DownfullWork(int downfull)
    {
        Die();
    }

    public void Die()
    {
        if (rootToDie == null)
            rootToDie = gameObject;

        Destroy(rootToDie);

        if(vfxDead != null)
            Destroy(Instantiate(vfxDead, transform.position, transform.rotation), 10f);

        OnDead?.Invoke();
    }
}
