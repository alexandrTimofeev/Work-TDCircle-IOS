using System;
using UnityEngine;

/// <summary>
/// Ограничитель значения типа int с событиями на изменение.
/// Логика полностью повторяет FloatContainer, но работает с целыми числами
/// и диапазоном <see cref="Vector2Int"/>.
/// </summary>
[Serializable]
public class IntContainer
{
    public string Title;

    [SerializeField] private int _value;
    [SerializeField] private Vector2Int _clampRange;
    private ReadOnlyIntContainer lifeContainer;

    // ─────────────────────────  События  ─────────────────────────

    public event Action<int> OnAddValue;       // сколько прибавили

    public event Action<int> OnRemoveValue;    // сколько убрали
    public event Action OnNotChangeValue; // вызов, если 0
    public event Action<int> OnChangeValue;    // новое значение (после клэмпа)
    public event Action<int> OnOverfullValue;  // переполнение: на сколько превысили максимум
    public event Action<int> OnDownfullValue;  // недополнение: на сколько ниже минимума

    // ─────────────────────────  Свойства  ─────────────────────────

    /// <summary>Текущее значение (после клэмпа).</summary>
    public int Value => _value;

    /// <summary> Диапазон [min, max] для ограничения.</summary>
    public Vector2Int ClampRange => _clampRange;

    // ─────────────────────────  Конструктор  ─────────────────────────

    public IntContainer(int initialValue, Vector2Int minMaxRange)
    {
        _clampRange = minMaxRange;
        SetValue(initialValue);
    }

    public IntContainer(ReadOnlyIntContainer lifeContainer)
    {
        _value = lifeContainer.Value;
        _clampRange = lifeContainer.ClampRange;
    }

    // ─────────────────────────  Публичные методы  ─────────────────────────

    /// <summary>
    /// Прибавить значение. Отрицательные/нулевые обрабатываются
    /// как в <see cref="RemoveValue"/> или <see cref="OnNotChangeValue"/>.
    /// </summary>
    public void AddValue(int add)
    {
        if (add == 0)
        {
            OnNotChangeValue?.Invoke();
            return;
        }

        if (add < 0)
        {
            RemoveValue(-add);
            return;
        }

        OnAddValue?.Invoke(add);
        SetValue(_value + add);
    }

    /// <summary>
    /// Уменьшить значение. Отрицательные/нулевые аналогично
    /// перенаправляются или вызывают <see cref="OnNotChangeValue"/>.
    /// </summary>
    public void RemoveValue(int remove)
    {
        if (remove == 0)
        {
            OnNotChangeValue?.Invoke();
            return;
        }

        if (remove < 0)
        {
            AddValue(-remove);
            return;
        }

        OnRemoveValue?.Invoke(remove);
        SetValue(_value - remove);
    }

    /// <summary>
    /// Жёстко установить значение с учётом клэмпа, уведомляя о
    /// переполнении/недополнении и самом изменении.
    /// </summary>
    public void SetValue(int value)
    {
        _value = value;

        if (_value > _clampRange.y)
            OnOverfullValue?.Invoke(_value - _clampRange.y);
        else if (_value <= _clampRange.x)
            OnDownfullValue?.Invoke(_clampRange.x - _value);

        _value = Mathf.Clamp(_value, _clampRange.x, _clampRange.y);
        OnChangeValue?.Invoke(_value);
    }

    /// <summary>
    /// Сбросить до указанного значения (или минимума, если null).
    /// </summary>
    public void ResetValue(int? newValue = null)
    {
        int target = newValue ?? _clampRange.x;
        _value = Mathf.Clamp(target, _clampRange.x, _clampRange.y);
        OnChangeValue?.Invoke(_value);
    }

    /// <summary>
    /// Изменить диапазон ограничения; текущее значение автоматически
    /// клэмпится к новому диапазону.
    /// </summary>
    public void SetClampRange(Vector2Int newRange)
    {
        _clampRange = newRange;
        SetValue(_value);
    }

    public void UpdateValue() => SetValue(_value);
}

public interface IReadOnlyIntContainer
{
    // Только чтение
    int Value { get; }
    Vector2Int ClampRange { get; }

    // События (только уведомления)
    event Action<int> OnChangeValue;
    event Action<int> OnOverfullValue;
    event Action<int> OnDownfullValue;

    /// <summary>
    /// Создать полную копию IntContainer для локального редактирования.
    /// </summary>
    IntContainer CloneForEdit();
}

[Serializable]
public class ReadOnlyIntContainer : IReadOnlyIntContainer
{
    private readonly IntContainer _target;

    public ReadOnlyIntContainer(IntContainer target)
    {
        _target = target;
    }

    // Только чтение
    public string Title => _target.Title;
    public int Value => _target.Value;
    public Vector2Int ClampRange => _target.ClampRange;

    // Подписка на события
    public event Action<int> OnChangeValue
    {
        add { _target.OnChangeValue += value; }
        remove { _target.OnChangeValue -= value; }
    }

    public event Action<int> OnOverfullValue
    {
        add { _target.OnOverfullValue += value; }
        remove { _target.OnOverfullValue -= value; }
    }

    public event Action<int> OnDownfullValue
    {
        add { _target.OnDownfullValue += value; }
        remove { _target.OnDownfullValue -= value; }
    }

    /// <summary>
    /// Создаёт копию IntContainer для локального редактирования.
    /// Изменения копии не затрагивают оригинал.
    /// </summary>
    public IntContainer CloneForEdit()
    {
        return new IntContainer(_target.Value, _target.ClampRange) { Title = _target.Title };
    }
}