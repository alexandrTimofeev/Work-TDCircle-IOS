using System;
using UnityEngine;

public class GunForAttaker : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private int damage = 10;
    [SerializeField] private bool chaseTarget = false;

    public void Shoot(Transform target)
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // Создаём пулю
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // Настраиваем пулю
            var bulletComponent = bullet.GetComponent<Bullet>();
            if (bulletComponent != null)
            {
                bulletComponent.Initialize(target, damage, bulletSpeed, chaseTarget);
            }
            else
            {
                // Или просто задаём направление
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 direction = (target.position - firePoint.position).normalized;
                    rb.linearVelocity = direction * bulletSpeed;
                }
            }
        }
    }

    public float GetBulletSpeed()
    {
        return bulletSpeed;
    }

    public void PowerDamage(float powerDamage)
    {
        damage = (int)(damage * powerDamage);
    }
}
