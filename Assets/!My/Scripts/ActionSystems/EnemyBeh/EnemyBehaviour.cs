using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public ContainerCommandPresets<EnemyBehaviourAction, EnemyBehaviourContext> commands;

    public void Work(EnemyBehaviourContext context)
    {
        commands.WorkAll(context);
    }

    public void WorkStart(EnemyBehaviourContext context)
    {
        foreach (var action in commands.actions)
        {
            action.WorkStart(context);
            context.Mover.StartCoroutine(action.WorkRoutine(context));
        }
    }
}

[Serializable]
public abstract class EnemyBehaviourAction : CommandPreset<EnemyBehaviourContext>
{
    public string ID;

    public abstract void WorkStart(EnemyBehaviourContext context);
}

[CustomEditor(typeof(EnemyBehaviour))]
public class EnemyBehaviourEditor
    : CommandContainerEditor<EnemyBehaviourAction>
{
    protected override string EntriesFieldName => "commands.actions";
}