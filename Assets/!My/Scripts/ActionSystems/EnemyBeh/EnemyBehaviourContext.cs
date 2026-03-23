using System;

[Serializable]
public class EnemyBehaviourContext : ContextCommandPreset
{
    public EnemyBehPlayer BehPlayer;
    public float Speed = 2f;
    public EnemyMover Mover;
}
