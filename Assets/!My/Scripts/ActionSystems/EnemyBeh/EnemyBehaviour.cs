using System;
using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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


#if UNITY_EDITOR
[CustomEditor(typeof(EnemyBehaviour))]
public class EnemyBehaviourEditor
    : CommandContainerEditor<EnemyBehaviourAction>
{
    protected override string EntriesFieldName => "commands.actions";
}
#endif