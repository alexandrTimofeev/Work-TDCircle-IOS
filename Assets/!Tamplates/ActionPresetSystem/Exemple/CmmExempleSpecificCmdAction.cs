using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class CmmExempleSpecificCmdAction : CmmExempleCmdAction
{
    public string DebugEndRoutine;
    public float Delay = 1f;

    public override void Work(CmmExempleCmdContext context)
    {
        Debug.Log($"I'M WORK!!! ({context.DebugAdd})");
    }

    public override IEnumerator WorkRoutine(CmmExempleCmdContext context)
    {
        Debug.Log($"I'M WORK Routine start ({context.DebugAdd})");
        yield return new WaitForSeconds(Delay);
        Debug.Log($"I'M WORK Routine end ({DebugEndRoutine})");
    }
}