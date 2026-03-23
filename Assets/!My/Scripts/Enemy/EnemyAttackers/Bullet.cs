using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    private int damage;
    private float speed;
    private bool chaseTarget;
    private Vector2 direction;

    [Space]
    [SerializeField] private DamageContext2D damageContext;
    [SerializeField] private DamageHitBox2D[] hitBoxes2D;

    public void Initialize(Transform target, int damage, float speed, bool? chaseTarget)
    {
        this.target = target;
        this.damage = damage;
        this.speed = speed;
        if(chaseTarget != null)
            this.chaseTarget = chaseTarget.Value;
        direction = transform.right;

        damageContext.Dmg = damage;
        foreach (var hitBox2D in hitBoxes2D)
        {
            hitBox2D.Context.Init(damageContext);
        }
    }

    private void Update()
    {
        // Движение к цели
        if (chaseTarget && target != null)
            direction = (target.position - transform.position).normalized;
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    /*private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == target)
        {
            // Наносим урон
            var health = other.GetComponent<IHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }*/
}