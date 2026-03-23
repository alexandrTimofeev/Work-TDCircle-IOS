using System;
using UnityEngine;

public abstract class EnemyMover : MonoBehaviour
{
    public abstract void SetDestination(Vector3 destination);
    public abstract void SetDestination(Transform destinationTarget);
    public abstract void MoveUpdate();
    public abstract void SetStopDistance(float distance);
    public abstract bool IsMove();
    public abstract float GetDistToTarget();
    public abstract void DoCommand(string command);
    public abstract void PowerSpeed(float powerSpeed);
}
