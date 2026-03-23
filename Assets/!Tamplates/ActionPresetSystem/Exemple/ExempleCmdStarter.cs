using System;
using System.Collections;
using UnityEngine;

public class ExempleCmdStarter : MonoBehaviour
{
    public CmmExempleCmdContainer ContainerStart;
    public bool IsRoutine;
    public CmmExempleCmdContext Context;

    // Use this for initialization
    void Start()
    {
        if (IsRoutine)
            StartCoroutine(RoutineWorkAll());
        else
            ContainerStart.commands.WorkAll(Context);
    }

    private IEnumerator RoutineWorkAll()
    {
        Debug.Log("Start Starter Routine");
        yield return ContainerStart.commands.WorkAllRoutine(Context);
        Debug.Log("End Starter Routine");
    }
}