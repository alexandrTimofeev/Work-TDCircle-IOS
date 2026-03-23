using System;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBehPlayer : MonoBehaviour
{
    [SerializeField] private EnemyBehaviourContext context;

    [Space]
    [SerializeField] private bool isStartToInit;
    [SerializeField] private EnemyBehaviour enemyBehaviourStart;

    [Space]
    [SerializeField] private DamageHitBox2D damageHitBox2D;
    public int Dmg = 2;

    [Space]
    [SerializeField] private EnemyMover enemyMover;

    private EnemyBehaviour behaviour;
    public EnemyBehaviour Behaviour => behaviour;

    private void Start()
    {
        if (isStartToInit)
            Init(enemyBehaviourStart);

        damageHitBox2D.Context.EnemyBeh = this;
    }

    public void Init(EnemyBehaviour enemyBehaviourStart)
    {
        SetBehaviour(enemyBehaviourStart);
    }

    private void SetBehaviour(EnemyBehaviour behaviour)
    {
        //if (behaviour != null)
        //    Destroy(behaviour.gameObject);

        //this.behaviour = Instantiate(behaviour, transform);
        this.behaviour = behaviour;
        behaviour.WorkStart(GetContext());
    }

    private void Update()
    {
        GetContext();

        context.Mover.MoveUpdate();

        if (behaviour == null)
            return;

        behaviour.Work(context);
    }

    public EnemyBehaviourContext GetContext()
    {
        context.BehPlayer = this;
        context.Mover = enemyMover;
        return context;
    }
}
