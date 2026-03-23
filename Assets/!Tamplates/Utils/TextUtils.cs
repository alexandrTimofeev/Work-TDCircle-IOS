using System;
using DG.Tweening;
using TMPro;

public static class TextUtils
{
    #region Core (Action<string>)

    public static void TypeTextAnimation(
        string text,
        float duration,
        Action<string> onUpdate,
        out Tweener tweener)
    {
        if (string.IsNullOrEmpty(text))
        {
            onUpdate?.Invoke(string.Empty);
            tweener = null;
            return;
        }

        int totalLength = text.Length;
        int currentLength = 0;

        duration = Math.Max(0.0001f, duration);

        tweener = DOTween.To(
                () => currentLength,
                x => currentLength = x,
                totalLength,
                duration
            )
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                int safeLength = Math.Clamp(currentLength, 0, totalLength);
                onUpdate?.Invoke(text.Substring(0, safeLength));
            })
            .OnComplete(() =>
            {
                onUpdate?.Invoke(text);
            });
    }

    public static void TypeTextAtSpeed(
        string text,
        float charsPerSecond,
        Action<string> onUpdate,
        out Tweener tweener)
    {
        if (charsPerSecond <= 0f)
        {
            onUpdate?.Invoke(text);
            tweener = null;
            return;
        }

        float duration = text.Length / charsPerSecond;
        TypeTextAnimation(text, duration, onUpdate, out tweener);
    }

    #endregion

    #region TMP_Text helpers

    public static void TypeToTMP(
        TMP_Text tmp,
        string text,
        float duration,
        out Tweener tweener)
    {
        if (tmp == null)
        {
            tweener = null;
            return;
        }

        tmp.text = string.Empty;

        TypeTextAnimation(
            text,
            duration,
            value => tmp.text = value,
            out tweener
        );
    }

    public static void TypeToTMPAtSpeed(
        TMP_Text tmp,
        string text,
        float charsPerSecond,
        out Tweener tweener)
    {
        if (tmp == null)
        {
            tweener = null;
            return;
        }

        tmp.text = string.Empty;

        TypeTextAtSpeed(
            text,
            charsPerSecond,
            value => tmp.text = value,
            out tweener
        );
    }

    #endregion

    #region Control

    public static bool IsTyping(Tweener tweener)
    {
        return tweener != null && tweener.IsActive() && tweener.IsPlaying();
    }

    public static void CompleteTyping(ref Tweener tweener)
    {
        if (tweener == null)
            return;

        if (tweener.IsActive())
            tweener.Complete();

        tweener = null;
    }

    public static void KillTyping(ref Tweener tweener)
    {
        if (tweener == null)
            return;

        if (tweener.IsActive())
            tweener.Kill();

        tweener = null;
    }

    #endregion
}