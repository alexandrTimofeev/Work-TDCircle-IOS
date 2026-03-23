using System;
using UnityEditor;
using UnityEngine;

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

[CustomEditor(typeof(CmmExempleCmdContainer))]
public class CmmExempleContainerEditor
    : CommandContainerEditor<CmmExempleCmdAction>
{
    protected override string EntriesFieldName => "commands.actions";
}