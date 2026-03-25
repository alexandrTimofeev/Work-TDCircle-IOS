using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public abstract class CommandContainerEditor<TCommand> : Editor
{
    protected SerializedProperty entriesProp;
    protected List<Type> commandTypes;

    protected abstract string EntriesFieldName { get; }

    protected virtual void OnEnable()
    {
        entriesProp = serializedObject.FindProperty(EntriesFieldName);

        CollectCommandTypes();
    }

    void CollectCommandTypes()
    {
        var baseType = typeof(TCommand);

        commandTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch { return Array.Empty<Type>(); }
            })
            .Where(t =>
                baseType.IsAssignableFrom(t) &&
                !t.IsAbstract &&
                t != baseType)
            .OrderBy(t => t.Name)
            .ToList();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspectorExceptContainer();

        EditorGUILayout.Space();

        DrawActions();

        serializedObject.ApplyModifiedProperties();
    }

    void DrawDefaultInspectorExceptContainer()
    {
        var prop = serializedObject.GetIterator();

        bool enterChildren = true;

        while (prop.NextVisible(enterChildren))
        {
            enterChildren = false;

            if (prop.name == "m_Script" || prop.name == EntriesFieldName)
            {
                if (prop.name == "m_Script")
                    EditorGUILayout.PropertyField(prop);

                continue;
            }

            EditorGUILayout.PropertyField(prop, true);
        }
    }

    void DrawActions()
    {
        if (entriesProp == null)
        {
            EditorGUILayout.HelpBox(
                $"Property '{EntriesFieldName}' not found in {target.GetType().Name}",
                MessageType.Error);

            return;
        }

        EditorGUILayout.LabelField("Commands", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Command", GUILayout.Width(120)))
            ShowAddMenu();

        if (GUILayout.Button("Refresh Types", GUILayout.Width(120)))
            CollectCommandTypes();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        int removeIndex = -1;

        for (int i = 0; i < entriesProp.arraySize; i++)
        {
            var element = entriesProp.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();

            string typeName = element.managedReferenceFullTypename;

            string title = "<null>";

            if (!string.IsNullOrEmpty(typeName))
            {
                var split = typeName.Split(' ');
                title = split.Length > 1 ? split[1] : typeName;
                title = title.Split('.').Last();
            }

            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

            if (GUILayout.Button("Remove", GUILayout.Width(70)))
                removeIndex = i;

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(element, true);

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
        }

        if (removeIndex >= 0)
        {
            entriesProp.DeleteArrayElementAtIndex(removeIndex);
        }
    }

    void ShowAddMenu()
    {
        var menu = new GenericMenu();

        foreach (var type in commandTypes)
        {
            var localType = type;

            menu.AddItem(
                new GUIContent(type.Name),
                false,
                () => AddCommand(localType));
        }

        menu.ShowAsContext();
    }

    void AddCommand(Type type)
    {
        if (entriesProp == null)
        {
            Debug.LogError("entriesProp is null");
            return;
        }

        serializedObject.Update();

        entriesProp.arraySize++;

        var element = entriesProp.GetArrayElementAtIndex(entriesProp.arraySize - 1);

        element.managedReferenceValue = Activator.CreateInstance(type);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif