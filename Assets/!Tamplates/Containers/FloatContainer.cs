using System;
using UnityEngine;

/// <summary>
/// Ограничитель значения типа float с событиями на изменение.
/// </summary>
public class FloatContainer
{
    public string Title;
    private float _value;
    private Vector2 _clampRange;

    // События
    public event Action<float> OnAddValue;
    public event Action<float> OnRemoveValue;
    public event Action OnNotChangeValue;
    public event Action<float> OnChangeValue;
    public event Action<float> OnOverfullValue;
    public event Action<float> OnDownfullValue;

    /// <summary>
    /// Текущее значение (после клэмпа).
    /// </summary>
    public float Value => _value;

    /// <summary>
    /// Диапазон [минимум, максимум] для ограничения.
    /// </summary>
    public Vector2 ClampRange => _clampRange;

    public FloatContainer(float initialValue, Vector2 minMaxValue)
    {
        _clampRange = minMaxValue;
        SetValue(initialValue);
    }

    /// <summary>
    /// Добавить значение. Если Add &lt; 0 — перенаправляется в RemoveValue.
    /// Если Add == 0 — OnNotChangeValue.
    /// Иначе — OnAddValue и установка нового значения.
    /// </summary>
    public void AddValue(float add)
    {
        if (add == 0f)
        {
            OnNotChangeValue?.Invoke();
            return;
        }

        if (add < 0f)
        {
            RemoveValue(-add);
            return;
        }
      
        OnAddValue?.Invoke(add);
        SetValue(_value + add);
    }

    /// <summary>
    /// Убрать значение. Если remove &lt; 0 — перенаправляется в AddValue.
    /// Если remove == 0 — OnNotChangeValue.
    /// Иначе — OnRemoveValue и установка нового значения.
    /// </summary>
    public void RemoveValue(float remove)
    {
        if (remove == 0f)
        {
            OnNotChangeValue?.Invoke();
            return;
        }

        if (remove < 0f)
        {
            AddValue(-remove);
            return;
        }
       
        OnRemoveValue?.Invoke(remove);
        SetValue(_value - remove);
    }

    /// <summary>
    /// Полная установка значения, с учётом клэмпа и уведомлений о выходе за границы.
    /// </summary>
    public void SetValue(float value)
    {
        _value = value;

        // Проверяем переполнение
        if (_value > _clampRange.y)
        {
            float over = _value - _clampRange.y;
            OnOverfullValue?.Invoke(over);
        }
        // Проверяем недополнение
        else if (_value < _clampRange.x)
        {
            float under = _clampRange.x - _value;
            OnDownfullValue?.Invoke(under);
        }

        // Сам клэмп
        _value = Mathf.Clamp(_value, _clampRange.x, _clampRange.y);

        // Уведомляем об изменении текущего значения
        OnChangeValue?.Invoke(_value);
    }

    /// <summary>
    /// Сброс значения на указанное или на минимум.
    /// </summary>
    public void ResetValue(float? newValue = null)
    {
        float target = newValue ?? _clampRange.x;
        _value = Mathf.Clamp(target, _clampRange.x, _clampRange.y);
        OnChangeValue?.Invoke(_value);
    }

    /// <summary>
    /// Сброс диапазона ограничения (мин и макс), при этом текущее значение клэмпится.
    /// </summary>
    public void SetClampRange(Vector2 newRange)
    {
        _clampRange = newRange;
        SetValue(_value);
    }
}