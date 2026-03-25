using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Базовый класс всех захватываемых объектов.
/// </summary>
[CreateAssetMenu(fileName = "GrappableActionContainer", menuName = "SGames/GrapSystem/GrappableActionContainer")]
public class GrappableActionContainer : MonoBehaviour
{
    public ContainerCommandPresets<GrappableObjectAction, GrappableObjectContext> commands;

    public string ID => commands.ID;
    public Sprite Icon => commands.Icon;
    public string Name => commands.Name;
    public string Description => commands.Description;
}

[System.Serializable]
public abstract class GrappableObjectAction : CommandPreset<GrappableObjectContext>
{
    public string Title;
}

[Serializable]
public class GrappableObjectContext : ContextCommandPreset
{
    public GrapObject GrapObject;
}

#if UNITY_EDITOR
[CustomEditor(typeof(GrappableActionContainer))]
public class GrappableActionContainerEditor
    : CommandContainerEditor<GrappableObjectAction>
{
    protected override string EntriesFieldName => "commands.actions";
}
#endif
