using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "AchievimentsData", menuName = "SGames/Achieviments")]
public class AchievimentsData : ScriptableObject
{
    private static AchievimentsData achivFromResource;
    public static AchievimentsData AchivFromResource
    {
        get
        {
            if (achivFromResource == null)
                achivFromResource = Resources.Load<AchievimentsData>("AchievimentsData");
            return achivFromResource;
        }
    }

    [SerializeField] private Achiviement[] allAchiviements = new Achiviement[0];
    public Achiviement[] AllAchiviements => allAchiviements;

    // При валидации объекта в редакторе — проверяем на разделяемые ссылки и клонируем повторяющиеся элементы
    private void OnValidate()
    {
        if (allAchiviements == null || allAchiviements.Length == 0)
            return;

        bool changed = false;

        for (int i = 0; i < allAchiviements.Length; i++)
        {
            if (allAchiviements[i] == null)
                continue;

            for (int j = i + 1; j < allAchiviements.Length; j++)
            {
                if (allAchiviements[j] == null)
                    continue;

                // Если сами объекты Achiviement указывают на один экземпляр (редко, но проверим)
                if (ReferenceEquals(allAchiviements[i], allAchiviements[j]))
                {
                    allAchiviements[j] = allAchiviements[j].DeepClone();
                    changed = true;
                    continue;
                }

                // Если Info ссылаются на один экземпляр — клонируем правый
                if (allAchiviements[i].Info != null && allAchiviements[j].Info != null &&
                    ReferenceEquals(allAchiviements[i].Info, allAchiviements[j].Info))
                {
                    allAchiviements[j].Info = allAchiviements[j].Info.Clone();
                    changed = true;
                }

                // Если behaviourStart ссылаются на один экземпляр — клонируем правый behaviourStart
                if (allAchiviements[i].Behaviour != null && allAchiviements[j].Behaviour != null &&
                    ReferenceEquals(allAchiviements[i].Behaviour, allAchiviements[j].Behaviour))
                {
                    allAchiviements[j].Behaviour = DeepCloneBehaviour(allAchiviements[j].Behaviour);
                    changed = true;
                }
            }
        }

#if UNITY_EDITOR
        if (changed)
        {
            EditorUtility.SetDirty(this);
            // Также сохраняем, чтобы изменения не терялись
            AssetDatabase.SaveAssets();
        }
#endif
    }

    // Копирование полей объекта behaviourStart в новый экземпляр его типа
    private static AchievementBehaviour DeepCloneBehaviour(AchievementBehaviour source)
    {
        if (source == null)
            return null;

        Type t = source.GetType();
        var clone = (AchievementBehaviour)Activator.CreateInstance(t);

        CopyFieldsRecursive(source, clone);

        return clone;
    }

    // Рекурсивно копируем поля (включая приватные) из src в dst
    private static void CopyFieldsRecursive(object src, object dst)
    {
        if (src == null || dst == null)
            return;

        Type type = src.GetType();
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        while (type != null && type != typeof(object))
        {
            var fields = type.GetFields(flags);
            foreach (var f in fields)
            {
                // Пропускаем константы / init-only? (обычно безопасно копировать всё)
                try
                {
                    var val = f.GetValue(src);
                    f.SetValue(dst, val);
                }
                catch
                {
                    // Игнорируем поля, которые не удалось скопировать
                }
            }
            type = type.BaseType;
        }
    }
}


[System.Serializable]
public class AchivInfo
{
    public string ID;
    public string Title;
    public string InfoTxt;
    public Sprite spriteYes;
    public Sprite spriteNo;
    public Color colorYes = Color.white;
    public Color colorNo = Color.gray;

    public Sprite GetSprite(bool contains)
    {
        return contains ? spriteYes : spriteNo;
    }

    public Color GetColor(bool contains)
    {
        return contains ? colorYes : colorNo;
    }

    public void ApplyToSpriteRenderer(SpriteRenderer spriteRenderer, bool contains)
    {
        if (spriteRenderer == null)
            return;

        spriteRenderer.sprite = GetSprite(contains);
        spriteRenderer.color = GetColor(contains);
    }

    public void ApplyToImage(Image image, bool contains)
    {
        if (image == null)
            return;
        image.sprite = GetSprite(contains);
        image.color = GetColor(contains);
    }

    public void ApplyToImageBack(Image image, bool contains)
    {
        if (image == null)
            return;

        image.color = GetColor(contains);
    }

    // Простое глубокое клонирование данных (поля)
    public AchivInfo Clone()
    {
        return new AchivInfo
        {
            ID = this.ID,
            Title = this.Title,
            InfoTxt = this.InfoTxt,
            spriteYes = this.spriteYes,
            spriteNo = this.spriteNo,
            colorYes = this.colorYes,
            colorNo = this.colorNo
        };
    }
}

[System.Serializable]
public class Achiviement
{
    [SerializeField] private string Name;
    public AchivInfo Info;
    [SerializeReference] private AchievementBehaviour achievementBehaviour;

    // Публичное свойство для доступа/замены behaviourStart из других классов
    public AchievementBehaviour Behaviour
    {
        get => achievementBehaviour;
        set => achievementBehaviour = value;
    }

    public bool CheckUnlock(bool isTestCondition = false)
    {
        return achievementBehaviour == null ? true : achievementBehaviour.CheckUnlock(isTestCondition);
    }

    public void ForceUnlock ()
    {
        if (achievementBehaviour == null)
            return;
        achievementBehaviour.ForceUnlock();
    }

    public bool IsUnlocked => achievementBehaviour == null ? true : achievementBehaviour.IsUnlocked;

    // Глубокий клон Achiviement (клонирует Info и создаёт новый экземпляр behaviourStart того же типа)
    public Achiviement DeepClone()
    {
        var clone = new Achiviement();

        // Копируем приватное поле внутри класса (разрешено)
        clone.Name = this.Name;

        clone.Info = this.Info != null ? this.Info.Clone() : null;

        if (this.achievementBehaviour != null)
        {
            // Создаём новый экземпляр того же типа и копируем поля
            Type t = this.achievementBehaviour.GetType();
            var behClone = (AchievementBehaviour)Activator.CreateInstance(t);
            // Копирование полей
            CopyFieldsForBehaviour(this.achievementBehaviour, behClone);
            clone.achievementBehaviour = behClone;
        }
        else
        {
            clone.achievementBehaviour = null;
        }

        return clone;
    }

    private static void CopyFieldsForBehaviour(AchievementBehaviour src, AchievementBehaviour dst)
    {
        if (src == null || dst == null)
            return;

        Type type = src.GetType();
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        while (type != null && type != typeof(object))
        {
            var fields = type.GetFields(flags);
            foreach (var f in fields)
            {
                try
                {
                    var val = f.GetValue(src);
                    f.SetValue(dst, val);
                }
                catch
                {
                    // Игнорируем поля, которые не удалось скопировать
                }
            }
            type = type.BaseType;
        }
    }
}