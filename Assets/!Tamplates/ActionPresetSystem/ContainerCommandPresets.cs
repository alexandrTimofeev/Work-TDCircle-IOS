using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[Serializable]
public class ContainerCommandPresets<A, C>
    where A : CommandPreset<C>
    where C : ContextCommandPreset
{
    public string ID;
    public Sprite Icon;
    public string Name; 
    public string Description;

    [Space]
    [SerializeReference]
    public List<A> actions = new();

    public virtual void WorkAll(C context)
    {
        foreach (var action in actions)
            action.Work(context);
    }

    public virtual IEnumerator WorkAllRoutine(C context)
    {
        foreach (var action in actions)
            yield return action.WorkRoutine(context);
    }
}