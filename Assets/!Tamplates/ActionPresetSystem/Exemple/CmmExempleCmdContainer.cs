using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CmmExempleCmdContainer
    : MonoBehaviour
{
    public ContainerCommandPresets<CmmExempleCmdAction, CmmExempleCmdContext> commands;
}

[Serializable]
public abstract class CmmExempleCmdAction : CommandPreset<CmmExempleCmdContext>
{
    public string ID;
}

[Serializable]
public class CmmExempleCmdContext : ContextCommandPreset
{
    public string DebugAdd;
}

#if UNITY_EDITOR
[CustomEditor(typeof(CmmExempleCmdContainer))]
public class CmmExempleContainerEditor
    : CommandContainerEditor<CmmExempleCmdAction>
{
    protected override string EntriesFieldName => "commands.actions";
}
#endif