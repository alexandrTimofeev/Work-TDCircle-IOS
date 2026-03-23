using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

[CustomEditor(typeof(GrappableActionContainer))]
public class GrappableActionContainerEditor
    : CommandContainerEditor<GrappableObjectAction>
{
    protected override string EntriesFieldName => "commands.actions";
}
