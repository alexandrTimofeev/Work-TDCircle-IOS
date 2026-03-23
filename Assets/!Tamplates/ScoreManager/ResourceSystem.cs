using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Хранилище ресурсов (IntContainer) с удобным доступом по строковому ID.
/// </summary>
public class ResourceSystem
{
    private readonly List<IntContainer> resources = new List<IntContainer>();

    /// <summary>
    /// Создать новый ресурс или перезаписать существующий.
    /// </summary>
    /// <param name="id">Строковый идентификатор ресурса.</param>
    /// <param name="value">Начальное значение.</param>
    /// <param name="minMaxValue">Диапазон (min, max).</param>
    public IntContainer CreateResource(string id, int value, Vector2Int minMaxValue)
    {
        // ищем по ID в уже существующих
        var existing = resources.FirstOrDefault(r => r.Title == id);

        if (existing != null)
        {
            // ресурс найден → обновляем диапазон и значение
            existing.SetClampRange(minMaxValue);
            existing.SetValue(value);
            return existing;
        }
        else
        {
            // создаём новый и добавляем в список
            var newRes = new IntContainer(value, minMaxValue) { Title = id };
            resources.Add(newRes);
            return newRes;
        }
    }

    /// <summary>
    /// Получить ресурс по строковому ID. Вернёт null, если не найден.
    /// </summary>
    public IntContainer GetResource(string id)
        => resources.FirstOrDefault(r => r.Title == id);

    /// <summary>
    /// Получить ресурс по индексу в списке. Вернёт null, если индекс вне диапазона.
    /// </summary>
    public IntContainer GetResource(int index)
        => (index >= 0 && index < resources.Count) ? resources[index] : null;
}