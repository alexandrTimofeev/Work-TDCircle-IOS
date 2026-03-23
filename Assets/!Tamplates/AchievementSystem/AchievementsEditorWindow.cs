#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

public class AchievementsEditorWindow : EditorWindow
{
    private AchievimentsData targetData;
    private SerializedObject serializedData;
    private SerializedProperty achievementsProp;

    private Type[] behaviourTypes = new Type[0];
    private string[] behaviourTypeNames = new string[0];

    private Vector2 _scrollPos;

    [MenuItem("SGames/Achievements Editor")]
    public static void OpenWindow() => GetWindow<AchievementsEditorWindow>("Achievements Editor");

    private void OnEnable()
    {
        RefreshBehaviourTypes();
    }   

    private void RefreshBehaviourTypes()
    {
        // собираем все непустые, не-абстрактные наследники AchievementBehaviour
        behaviourTypes = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic)
            .SelectMany(a => {
                try { return a.GetTypes(); }
                catch { return new Type[0]; }
            })
            .Where(t => t != null && typeof(AchievementBehaviour).IsAssignableFrom(t) && !t.IsAbstract)
            .ToArray();

        behaviourTypeNames = behaviourTypes.Select(t => t.Name).ToArray();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUI.BeginChangeCheck();
        targetData = (AchievimentsData)EditorGUILayout.ObjectField("Achieviments Data", targetData, typeof(AchievimentsData), false);
        if (EditorGUI.EndChangeCheck())
        {
            serializedData = null; // пересоздадим SerializedObject при смене
        }

        if (targetData == null)
        {
            EditorGUILayout.HelpBox("Выберите AchievimentsData (ScriptableObject) для редактирования.", MessageType.Info);
            if (GUILayout.Button("Загрузить из MyResources/AchievimentsData"))
            {
                targetData = AchievimentsData.AchivFromResource;
                serializedData = null;
            }
            return;
        }

        if (serializedData == null || serializedData.targetObject != targetData)
        {
            serializedData = new SerializedObject(targetData);
            achievementsProp = serializedData.FindProperty("allAchiviements");
        }

        serializedData.Update();

        if (achievementsProp == null)
        {
            EditorGUILayout.HelpBox("Поле 'allAchiviements' не найдено в выбранном AchievimentsData. Убедитесь, что в классе оно называется именно так и помечено [SerializeField].", MessageType.Error);
            return;
        }

        EditorGUILayout.LabelField("Achievements", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Скроллируемая область для списка достижений
        float scrollHeight = Mathf.Clamp(position.height - 160f, 120f, 800f);
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Height(scrollHeight));

        for (int i = 0; i < achievementsProp.arraySize; i++)
        {
            var element = achievementsProp.GetArrayElementAtIndex(i);
            var infoProp = element.FindPropertyRelative("Info");
            var behaviourProp = element.FindPropertyRelative("achievementBehaviour");
            var nameProp = element.FindPropertyRelative("Name");

            EditorGUILayout.BeginVertical("box");

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(infoProp, new GUIContent("Info"), true);
            if (EditorGUI.EndChangeCheck())
            {
                // при изменении Info.ID синхронизируем приватное Name с Info.ID
                var idProp = infoProp.FindPropertyRelative("ID");
                if (idProp != null && nameProp != null)
                {
                    nameProp.stringValue = idProp.stringValue;
                }
            }

            // Построим popup опций: [<None>, Type1, Type2, ...]
            string[] popupOptions = new string[behaviourTypeNames.Length + 1];
            popupOptions[0] = "<None>";
            for (int k = 0; k < behaviourTypeNames.Length; k++) popupOptions[k + 1] = behaviourTypeNames[k];

            int currentIndex = -1;
            var currentObj = behaviourProp.managedReferenceValue;
            if (currentObj != null)
            {
                var t = currentObj.GetType();
                currentIndex = Array.IndexOf(behaviourTypes, t);
            }

            int popupIndex = currentIndex + 1;
            int newPopupIndex = EditorGUILayout.Popup("Behaviour", popupIndex, popupOptions);

            if (newPopupIndex != popupIndex)
            {
                if (newPopupIndex == 0)
                {
                    behaviourProp.managedReferenceValue = null;
                }
                else
                {
                    Type chosen = behaviourTypes[newPopupIndex - 1];
                    behaviourProp.managedReferenceValue = Activator.CreateInstance(chosen);
                }
            }

            // Рисуем поля конкретного behaviourStart (если не null)
            if (behaviourProp.managedReferenceValue != null)
            {
                // Toggle foldout? Для простоты — прямо PropertyField (работает с [SerializeReference])
                EditorGUILayout.PropertyField(behaviourProp, new GUIContent("Behaviour"), true);
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Remove Achievement"))
            {
                achievementsProp.DeleteArrayElementAtIndex(i);
                // Если Unity оставляет "висящую" ссылку, удаляем ещё раз
                if (i < achievementsProp.arraySize && achievementsProp.GetArrayElementAtIndex(i) != null && achievementsProp.GetArrayElementAtIndex(i).isArray)
                {
                    // noop (старый код оставлен для совместимости)
                }
                serializedData.ApplyModifiedProperties();
                serializedData.Update();
                break; // выходим, потому что структура массива изменилась
            }
            if (GUILayout.Button("Clear Behaviour"))
            {
                behaviourProp.managedReferenceValue = null;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Achievement"))
        {
            // Добавляем новый элемент и инициализируем
            int insertIndex = achievementsProp.arraySize;
            // Решение: если есть существующие элементы — клонируем последний (deep clone полей),
            // иначе — создаём новый пустой элемент.
            if (achievementsProp.arraySize == 0)
            {
                achievementsProp.arraySize++;
                serializedData.ApplyModifiedProperties(); // чтобы Unity создала элемент
                serializedData.Update();

                var newElem = achievementsProp.GetArrayElementAtIndex(insertIndex);
                var newInfo = newElem.FindPropertyRelative("Info");
                if (newInfo != null)
                {
                    var idProp = newInfo.FindPropertyRelative("ID");
                    if (idProp != null) idProp.stringValue = "NewAchievement";
                }
                var newBehaviour = newElem.FindPropertyRelative("achievementBehaviour");
                if (newBehaviour != null)
                {
                    newBehaviour.managedReferenceValue = null;
                }

                var nameProp = newElem.FindPropertyRelative("Name");
                if (nameProp != null && newInfo != null)
                {
                    var idProp = newInfo.FindPropertyRelative("ID");
                    nameProp.stringValue = idProp != null ? idProp.stringValue : "NewAchievement";
                }
            }
            else
            {
                // Клонируем последний элемент в новый
                int sourceIndex = achievementsProp.arraySize - 1;
                var sourceElem = achievementsProp.GetArrayElementAtIndex(sourceIndex);

                achievementsProp.arraySize++;
                serializedData.ApplyModifiedProperties(); // чтобы Unity создала элемент
                serializedData.Update();

                var newElem = achievementsProp.GetArrayElementAtIndex(insertIndex);
                var sourceInfo = sourceElem.FindPropertyRelative("Info");
                var newInfo = newElem.FindPropertyRelative("Info");

                if (sourceInfo != null && newInfo != null)
                {
                    // Копируем поля внутри Info
                    var fields = new string[] { "ID", "Title", "InfoTxt", "spriteYes", "spriteNo", "colorYes", "colorNo" };
                    foreach (var f in fields)
                    {
                        var sProp = sourceInfo.FindPropertyRelative(f);
                        var nProp = newInfo.FindPropertyRelative(f);
                        if (sProp == null || nProp == null) continue;

                        switch (sProp.propertyType)
                        {
                            case SerializedPropertyType.String:
                                nProp.stringValue = sProp.stringValue;
                                break;
                            case SerializedPropertyType.ObjectReference:
                                nProp.objectReferenceValue = sProp.objectReferenceValue;
                                break;
                            case SerializedPropertyType.Color:
                                nProp.colorValue = sProp.colorValue;
                                break;
                            default:
                                // при необходимости можно добавить другие типы
                                break;
                        }
                    }
                }

                // Клонируем behaviourStart (managed reference) через создание нового инстанса и копирование данных через EditorJsonUtility
                var sourceBehaviour = sourceElem.FindPropertyRelative("achievementBehaviour");
                var newBehaviour = newElem.FindPropertyRelative("achievementBehaviour");
                if (sourceBehaviour != null && newBehaviour != null)
                {
                    var srcObj = sourceBehaviour.managedReferenceValue;
                    if (srcObj != null)
                    {
                        Type t = srcObj.GetType();
                        object clone = Activator.CreateInstance(t);
                        // Копируем сериализуемые поля через EditorJsonUtility (работает в редакторе)
                        try
                        {
                            string json = EditorJsonUtility.ToJson(srcObj);
                            EditorJsonUtility.FromJsonOverwrite(json, clone);
                            newBehaviour.managedReferenceValue = clone;
                        }
                        catch
                        {
                            // Если не удалось через Json, просто создадим пустой экземпляр
                            newBehaviour.managedReferenceValue = Activator.CreateInstance(t);
                        }
                    }
                    else
                    {
                        newBehaviour.managedReferenceValue = null;
                    }
                }

                // Установим Name равным Info.ID в новом элементе
                var nameProp = newElem.FindPropertyRelative("Name");
                if (nameProp != null && newInfo != null)
                {
                    var idProp = newInfo.FindPropertyRelative("ID");
                    nameProp.stringValue = idProp != null ? idProp.stringValue : "NewAchievement";
                }
            }

            // Обновляем SerializedObject после модификации
            serializedData.ApplyModifiedProperties();
            serializedData.Update();
        }

        if (GUILayout.Button("Refresh Behaviour Types"))
        {
            RefreshBehaviourTypes();
        }
        EditorGUILayout.EndHorizontal();

        serializedData.ApplyModifiedProperties();

        if (GUI.changed)
            EditorUtility.SetDirty(targetData);
    }
}
#endif