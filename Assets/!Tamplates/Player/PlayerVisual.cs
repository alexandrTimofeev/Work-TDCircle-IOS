using System;
using UnityEngine;
using DG.Tweening;

public class PlayerVisual : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform root;
    public SpriteRenderer SpriteRenderer => spriteRenderer;

    [Header("Invulnerability Flicker (via Color)")]
    [SerializeField, Tooltip("Общая длительность мигания")]
    private float flickerDuration = 1f;
    [SerializeField, Tooltip("Интервал одного мигания (впадение + возврат)")]
    private float flickerInterval = 0.1f;
    [SerializeField, Range(0f, 1f), Tooltip("Минимальная альфа при мигании")]
    private float flickerMinAlpha = 0.3f;
    private Color _originalColor;
    private Tween _flickerTween;

    [Header("Punch Animation")]
    [SerializeField, Tooltip("Продолжительность удара")]
    private float punchDuration = 0.3f;
    [SerializeField, Tooltip("Сила удара (смещение)")]
    private Vector3 punchStrength = new Vector3(0.2f, 0.2f, 0f);
    [SerializeField, Tooltip("Вибрация (количество колебаний)")]
    private int punchVibrato = 10;
    [SerializeField, Tooltip("Эластичность (0–1)")]
    private float punchElasticity = 1f;

    [Header("Punch Position Animation")]
    [SerializeField, Tooltip("Продолжительность удара")]
    private float punchPositionDuration = 0.3f;
    [SerializeField, Tooltip("Сила удара (смещение)")]
    private Vector3 punchPositionStrength = new Vector3(0.2f, 0.2f, 0f);
    [SerializeField, Tooltip("Вибрация (количество колебаний)")]
    private int punchPositionVibrato = 10;
    [SerializeField, Tooltip("Эластичность (0–1)")]
    private float punchPositionElasticity = 1f;

    [Header("Punch Scale Animation")]
    [SerializeField, Tooltip("Продолжительность удара")]
    private float punchScaleDuration = 0.3f;
    [SerializeField, Tooltip("Сила удара (смещение)")]
    private Vector3 punchScaleStrength = new Vector3(0.2f, 0.2f, 0f);
    [SerializeField, Tooltip("Вибрация (количество колебаний)")]
    private int punchScaleVibrato = 10;
    [SerializeField, Tooltip("Эластичность (0–1)")]
    private float punchScaleElasticity = 1f;

    [Header("Walking Sway")]
    [SerializeField, Tooltip("Угол отклонения при ходьбе")]
    private float swayAngle = 10f;
    [SerializeField, Tooltip("Продолжительность одного колебания")]
    private float swayDuration = 0.5f;
    [SerializeField, Tooltip("Ease для sway")]
    private Ease swayEase = Ease.InOutSine;
    private Tween _swayTween;

    private void Awake()
    {
        _originalColor = spriteRenderer.color;
    }

    public void SetSkin(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    // ────────────────────── Invulnerability Flicker ──────────────────────

    /// <summary>Начать мигание через смену цвета (альфы).</summary>
    public void StartFlicker()
    {
        StopFlicker();

        //int loops = Mathf.CeilToInt(flickerDuration / flickerInterval);
        // Sequence: покачиваем альфу до min, обратно, loops раз
        _flickerTween = DOTween.Sequence()
            .Append(spriteRenderer.DOFade(flickerMinAlpha, flickerInterval).SetEase(Ease.Linear))
            .Append(spriteRenderer.DOFade(_originalColor.a, flickerInterval).SetEase(Ease.Linear))
            .SetLoops(-1, LoopType.Restart);
            /*.OnComplete(() =>
            {
                image.Color = _originalColor;
                _flickerTween = null;
            });*/
    }

    /// <summary>Остановить мигание и вернуть исходный цвет.</summary>
    public void StopFlicker()
    {
        if (_flickerTween != null && _flickerTween.IsActive())
        {
            _flickerTween.Kill();
            _flickerTween = null;
        }
        spriteRenderer.color = _originalColor;
    }

    // ────────────────────── Punch ──────────────────────

    /// <summary>Отыграть анимацию удара (смещение позиции).</summary>
    public void PlayPunchPosition(
        Vector3? strength = null,
        float? duration = null,
        int? vibrato = null,
        float? elasticity = null)
    {
        transform.DOKill();

        transform.DOPunchPosition(
            strength ?? punchPositionStrength,
            duration ?? punchPositionDuration,
            vibrato ?? punchPositionVibrato,
            elasticity ?? punchPositionElasticity)
            .SetEase(Ease.OutQuart);
    }

    /// <summary>Отыграть анимацию удара (смещение масштаба).</summary>
    public void PlayPunchScale(
        Vector3? strength = null,
        float? duration = null,
        int? vibrato = null,
        float? elasticity = null)
    {
        transform.DOKill();

        transform.DOPunchScale(
            strength ?? punchScaleStrength,
            duration ?? punchScaleDuration,
            vibrato ?? punchScaleVibrato,
            elasticity ?? punchScaleElasticity)
            .SetEase(Ease.Linear);
    }

    // ────────────────────── Walking Sway ──────────────────────

    /// <summary>Начать бесконечное колебание вращения.</summary>
    public void StartSway()
    {
        if (_swayTween != null && _swayTween.IsActive())
            return;

        // Текущий угол
        float currentZ = transform.localEulerAngles.z;
        if (currentZ > 180f) currentZ -= 360f; // нормализуем до -180...180

        float fromAngle = -swayAngle;
        float toAngle = swayAngle;

        _swayTween = DOTween.To(
            () => 0f,
            val =>
            {
                float sway = Mathf.Lerp(fromAngle, toAngle, Mathf.Sin(val * Mathf.PI));
                transform.localEulerAngles = new Vector3(0, 0, currentZ + sway);
            },
            1f,
            swayDuration
        ).SetLoops(-1)
         .SetEase(Ease.Linear)
         .SetTarget(transform);
    }

    /// <summary>Остановить sway и вернуть персонажа в нейтральное положение.</summary>
    public void StopSway()
    {
        if (_swayTween != null && _swayTween.IsActive())
        {
            _swayTween.Kill();
            _swayTween = null;
        }
        transform.localRotation = Quaternion.identity;
    }
}
