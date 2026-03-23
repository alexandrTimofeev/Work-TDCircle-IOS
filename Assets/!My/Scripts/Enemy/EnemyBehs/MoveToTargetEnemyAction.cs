using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class MoveToTargetEnemyAction : EnemyBehaviourAction
{
    [SerializeField] private string MyTagTarget;

    public override void Work(EnemyBehaviourContext context)
    { }

    public override IEnumerator WorkRoutine(EnemyBehaviourContext context)
    { yield break; }

    public override void WorkStart(EnemyBehaviourContext context)
    {
        context.Mover.SetDestination(MyTagsObserver.FindFirstGameObjectWithTag(MyTagTarget).transform);
    }
}

public class MoveRandomPointEnemyAction : EnemyBehaviourAction
{
    public Vector2 zone;
    public float dealy;

    public override void Work(EnemyBehaviourContext context)
    { }

    public override IEnumerator WorkRoutine(EnemyBehaviourContext context)
    {
        while (true)
        {
            context.Mover.SetDestination(GetRandomPositionInField(context));

            float tm = dealy;
            while (tm > 0f)
            {
                tm -= Time.deltaTime;
                yield return new WaitWhile(() => GamePause.IsPause);
                yield return null;
            }

            yield return null;
            yield return new WaitWhile(() => context.Mover.IsMove());
        }
    }

    private Vector2 GetRandomPositionInField(EnemyBehaviourContext context)
    {
        Vector3 p0 = GameG.Planet.transform.position;
        while (true)
        {
            Vector2 npos = new Vector2(p0.x + UnityEngine.Random.Range(-zone.x, zone.x), p0.y + UnityEngine.Random.Range(-zone.y, zone.y));
            if(Vector2.Distance(npos, GameG.Planet.transform.position) > 1f)
                return npos;
        }
    }

    public override void WorkStart(EnemyBehaviourContext context)
    { }
}