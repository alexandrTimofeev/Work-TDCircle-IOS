using System.Collections;
using UnityEngine;

public static class BrowserUtils
{
    /// <summary>
    /// Открывает URL в стандартном браузере устройства
    /// </summary>
    /// <param name="url">Ссылка (с http/https или без)</param>
    public static void OpenUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("[BrowserUtils] URL is empty!");
            return;
        }

        // Добавляем протокол если отсутствует
        if (!url.StartsWith("http://") && !url.StartsWith("https://"))
        {
            url = "https://" + url;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        OpenInAndroidBrowser(url);
#elif UNITY_IOS && !UNITY_EDITOR
        OpenInIOSBrowser(url);
#else
        Application.OpenURL(url); // Для редактора и других платформ
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private static void OpenInAndroidBrowser(string url)
    {
        try 
        {
            using (var intent = new AndroidJavaObject("android.content.Intent", "android.intent.action.VIEW",
                   new AndroidJavaClass("android.net.Uri").CallStatic<AndroidJavaObject>("parse", url)))
            {
                intent.Call<AndroidJavaObject>("addFlags", 0x10000000); // FLAG_ACTIVITY_NEW_TASK
                GetCurrentActivity().Call("startActivity", intent);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("[BrowserUtils] Android error: " + e.Message);
            Application.OpenURL(url); // Fallback
        }
    }

    private static AndroidJavaObject GetCurrentActivity()
    {
        return new AndroidJavaClass("com.unity3d.player.UnityPlayer")
            .GetStatic<AndroidJavaObject>("currentActivity");
    }
#endif

#if UNITY_IOS && !UNITY_EDITOR
    private static void OpenInIOSBrowser(string url)
    {
        Application.OpenURL(url); // На iOS всегда открывает в Safari
    }
#endif

    /// <summary>
    /// Проверяет, поддерживается ли открытие ссылок на текущей платформе
    /// </summary>
    public static bool IsBrowserSupported()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try 
        {
            using (var intent = new AndroidJavaObject("android.content.Intent", "android.intent.action.VIEW"))
            {
                var manager = GetCurrentActivity().Call<AndroidJavaObject>("getPackageManager");
                return intent.Call<int>("resolveActivity", manager) != 0;
            }
        }
        catch
        {
            return false;
        }
#else
        return true; // iOS и редактор всегда поддерживают
#endif
    }
}