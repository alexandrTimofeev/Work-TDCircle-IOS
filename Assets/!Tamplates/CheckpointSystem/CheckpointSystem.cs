using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CheckpointSystem
{
    private Checkpoint[] checkpoints;
    private Checkpoint lastCheckpoint;

    private List<Checkpoint> prevChecks = new List<Checkpoint>();
    private List<ICheckpointObject> checkpointObjects = new List<ICheckpointObject>();

    public Action<Checkpoint> OnCheck;
    public Action<Checkpoint> OnReload;
    public Action<ICheckpointObject> OnReloadObject;

    public void Init()
    {
        checkpoints = GameObject.FindObjectsOfType<Checkpoint>();
        for (int i = 0; i < checkpoints.Length; i++)
        {
            Checkpoint checkpoint = checkpoints[i];
            checkpoint.OnCheck += Check;
        }
    }

    private void Check(Checkpoint checkpoint)
    {
        if (prevChecks.Contains(checkpoint))        
            return;

        lastCheckpoint = checkpoint;
        prevChecks.Add(checkpoint);

        OnCheck?.Invoke(checkpoint);

        checkpointObjects.Clear();
    }

    public void Reload()
    {
        OnReload?.Invoke(lastCheckpoint);

        for (int i = 0; i < checkpointObjects.Count; i++)
        {
            ICheckpointObject checkpointObject = checkpointObjects[i];
            ReloadCheckpointObject(checkpointObject);
            i--;
        }
    }

    public void HideCheckpointObject(ICheckpointObject checkpointObject)
    {
        checkpointObjects.Add(checkpointObject);
    }

    public void ReloadCheckpointObject (ICheckpointObject checkpointObject)
    {
        checkpointObject.Reload();
        OnReloadObject?.Invoke(checkpointObject);
        checkpointObjects.Remove(checkpointObject);
    }
}

public interface ICheckpointObject
{
    public abstract void InitCheckpoint(CheckpointSystem checkpointSystem);
    public abstract void Reload();
    public abstract void CheckHide();
}