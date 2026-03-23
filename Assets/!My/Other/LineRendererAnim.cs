using DG.Tweening;
using System;
using UnityEngine;

public enum LineAnimateType
{
    None,
    MoveToNext,
    MoveToPrevious
}

public class LineRendererAnim : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    private Tweener tweener;

    public LineRenderer LineRenderer => lineRenderer;

    private LineAnimateType lineAnimateType;

    // Кешированные массивы для избежания аллокаций
    private Vector3[] starts;
    private Vector3[] deltas;
    private Vector3[] currentPositions;

    public bool IsPlaying => tweener != null && tweener.IsActive() && tweener.IsPlaying();  

    public void SetTexture(Texture2D startTexure)
    {
        if (lineRenderer == null || lineRenderer.material == null) return;
        lineRenderer.material.SetTexture("_MainTex", startTexure);
    }

    public void StartAnimate(bool onPlay, LineAnimateType lineAnimateType = LineAnimateType.MoveToNext)
    {
        if (lineRenderer == null)
            return;

        this.lineAnimateType = lineAnimateType;

        if (onPlay)
        {
            if (tweener != null && tweener.IsPlaying())
                tweener.Kill();

            switch (lineAnimateType)
            {
                case LineAnimateType.MoveToNext:
                    PlayLineToOtherPoint(1);
                    break;
                case LineAnimateType.MoveToPrevious:
                    PlayLineToOtherPoint(-1);
                    break;
                default:
                    break;
            }
        }
        else
        {
            if (tweener != null)
                tweener.Kill();
            tweener = null;
        }
    }

    public void ReplayAnimations()
    {
        StartAnimate(false);
        StartAnimate(true, lineAnimateType);
    }

    private void PlayLineToOtherPoint(int offset = 1)
    {
        if (lineRenderer == null) return;

        int count = lineRenderer.positionCount;
        if (count < 2)
            return;

        // Инициализация кешей
        if (starts == null || starts.Length != count)
        {
            starts = new Vector3[count];
            deltas = new Vector3[count];
            currentPositions = new Vector3[count];
        }

        for (int i = 0; i < count; i++)
            starts[i] = lineRenderer.GetPosition(i);

        for (int i = 0; i < count - 1; i++)
            deltas[i] = starts[i + 1] - starts[i];
        deltas[count - 1] = starts[0] - starts[count - 1];

        float duration = 1f;

        if (tweener != null)
            tweener.Kill();

        // Твик обновляет один раз за кадр массив currentPositions и устанавливает все точки разом
        tweener = DOVirtual.Float(0f, 1f, duration, (value) =>
        {
            for (int i = 0; i < count; i++)
            {
                currentPositions[i] = starts[i] + deltas[i] * value;
            }
            lineRenderer.SetPositions(currentPositions);
        })
        .SetLoops(-1, LoopType.Restart)
        .SetEase(Ease.Linear)
        .OnStepComplete(() =>
        {
            // Сдвигаем starts на offset, чтобы при сбросе tween'а к 0 визуально не было скачка
            if (starts == null || starts.Length != count) return;

            Vector3[] newStarts = new Vector3[count];
            for (int i = 0; i < count; i++)
            {
                int srcIndex = (i + offset) % count;
                if (srcIndex < 0) srcIndex += count;
                newStarts[i] = starts[srcIndex];
            }

            Array.Copy(newStarts, starts, count);
            for (int i = 0; i < count - 1; i++)
                deltas[i] = starts[i + 1] - starts[i];
            deltas[count - 1] = starts[0] - starts[count - 1];
        });
    }
}
