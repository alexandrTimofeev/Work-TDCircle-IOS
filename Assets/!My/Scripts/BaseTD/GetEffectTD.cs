using System;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class GetEffectTD : MonoBehaviour
{
    private LifeBody m_LifeBody;
    [SerializeField] private GunForAttaker gun;
    [SerializeField] private GameObject powerUpGO;
    [SerializeField] private EnemyMover enemyMover;

    private float powerUp;

    private void Awake()
    {
        m_LifeBody = GetComponent<LifeBody>();
    }

    public void ApplyFirst(BuildEffectorTD buildEffectorTD)
    {
        if (gun != null && buildEffectorTD.PowerDamage != 0f)
        {
            gun.PowerDamage(buildEffectorTD.PowerDamage);
            powerUpGO?.SetActive(true);
            powerUp += buildEffectorTD.PowerDamage;
        }

        if (enemyMover != null && buildEffectorTD.PowerSpeed != 0f)
            enemyMover.PowerSpeed(buildEffectorTD.PowerSpeed);
    }

    public void Apply(BuildEffectorTD buildEffectorTD)
    {
        if (m_LifeBody != null && buildEffectorTD.AddHealth != 0)
            m_LifeBody.Heal(buildEffectorTD.AddHealth);
    }

    public void DisableEffect (BuildEffectorTD buildEffectorTD)
    {
        if (gun != null && buildEffectorTD.PowerDamage != 0f)
        {
            gun.PowerDamage(1f / buildEffectorTD.PowerDamage);
            powerUp -= buildEffectorTD.PowerDamage;
            if(powerUp == 0f)
                powerUpGO?.SetActive(false);
        }

        if (enemyMover != null && buildEffectorTD.PowerSpeed != 0f)
            enemyMover.PowerSpeed(1f / buildEffectorTD.PowerSpeed);
    }
}
