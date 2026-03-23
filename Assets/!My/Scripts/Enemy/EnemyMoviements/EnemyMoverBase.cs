using UnityEngine;

public class EnemyMoverBase : EnemyMover
{
    public float Speed = 2f;
    public float StopDistance = 0.2f;
    public Transform lookAtTr;

    private Transform TargetTr;
    private Vector2 TargetPos;
    private bool isMove;

    public override void DoCommand(string command)
    { }

    public override float GetDistToTarget()
    {
        return Vector2.Distance(transform.position, GetTargetPos());
    }

    public override bool IsMove()
    {
        return isMove;
    }

    public override void MoveUpdate()
    {
        isMove = GetDistToTarget() > StopDistance;

        if (isMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, GetTargetPos(), Speed * Time.deltaTime);
            if (lookAtTr != null)
                lookAtTr.rotation = Quaternion.LookRotation(transform.forward, Vector3.Cross(transform.forward, (Vector3)TargetPos - transform.position));
        }
    }

    public override void SetDestination(Vector3 destination)
    {
        TargetTr = null;
        TargetPos = destination;
    }

    public override void SetDestination(Transform destinationTarget)
    {
        TargetTr = destinationTarget;
        TargetPos = destinationTarget.position;
    }

    public override void SetStopDistance(float distance)
    {
        StopDistance = distance;
    }

    public virtual Vector2 GetTargetPos()
    {
        if (TargetTr != null)
            TargetPos = TargetTr.transform.position;
        return TargetPos;
    }

    public override void PowerSpeed(float powerSpeed)
    {
        Speed *= powerSpeed;
    }
}