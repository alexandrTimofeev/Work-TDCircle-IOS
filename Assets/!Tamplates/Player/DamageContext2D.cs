using System;

[Serializable]
public class DamageContext2D
{
    public DamageHitBox2D DamageHitBox;
    public DamageHurtBox2D DamageHurtBox;
    public EnemyBehPlayer EnemyBeh;
    public int Dmg;

    public DamageContext2D CloneForAction(DamageHitBox2D damageHit = null, DamageHurtBox2D damageHurt = null)
    {
        return new DamageContext2D()
        {
            DamageHitBox = damageHit ? damageHit : DamageHitBox,
            DamageHurtBox = damageHurt ? damageHurt : DamageHurtBox,
            EnemyBeh = EnemyBeh,
            Dmg = Dmg
        };
    }

    public void Init(DamageContext2D damageContext)
    {
        if(DamageHitBox)
            DamageHitBox = damageContext.DamageHitBox;
        if (DamageHurtBox)
            DamageHurtBox = damageContext.DamageHurtBox;
        if (EnemyBeh)
            EnemyBeh = damageContext.EnemyBeh;
        if(Dmg != -1)
            Dmg = damageContext.Dmg;
    }
}