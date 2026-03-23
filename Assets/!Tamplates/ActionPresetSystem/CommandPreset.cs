using System;
using System.Collections;

[Serializable]
public abstract class CommandPreset<C> where C : ContextCommandPreset
{
    public abstract void Work(C context);

    public abstract IEnumerator WorkRoutine(C context);
}

[Serializable]
public abstract class ContextCommandPreset
{

}