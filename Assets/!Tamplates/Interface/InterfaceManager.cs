using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using DG.Tweening;

public enum InterfaceComand { None, SwitchPause, OpenPause, ClosePause, PlayGame, Exit, GoToMenu }

public static class InterfaceManager
{
    private static Canvas mainCanvas;
    private static WinWindowUI winWindowPref;
    private static PauseWindowUI pauseWindowPref;
    private static LoseWindowUI loseWindowPref;
    private static TutorialWindowUI tutorialWindowPref;
    private static BlackScreen blackScreen;
    private static TextScoreUpLR flyingTextPref;
    private static TextScoreUpLR flyingTextUIPref;

    public static BarUIMediator BarMediator { get; private set; }
    public static Canvas MainCanvas => mainCanvas;

    public static BlackScreen MainBlackScreen => blackScreen;

    private static readonly List<WindowUI> createdWindows = new();

    public static Action<WindowUI> OnClose;
    public static Action<WindowUI> OnOpen;

    public static Action<InterfaceComand> OnClickCommand;

    /*──────────────────────────  INIT  ──────────────────────────*/

    public static void Init(Canvas canvasOverride = null)
    {
        OnOpen = null;
        OnClose = null;
        OnClickCommand = null;
        createdWindows.Clear();
        blackScrees.Clear();

        mainCanvas = canvasOverride ??
                     GameObject.FindGameObjectWithTag("MainCanvas")?.GetComponent<Canvas>();

        if (mainCanvas == null)
        {
            Debug.LogError("InterfaceManager: Canvas not found. Tag a canvas with 'MainCanvas' or pass it explicitly.");
            //return;
        }

        winWindowPref = Resources.Load<WinWindowUI>("InterfacePrefabs/WinWindowUI");
        pauseWindowPref = Resources.Load<PauseWindowUI>("InterfacePrefabs/PauseWindowUI");
        loseWindowPref = Resources.Load<LoseWindowUI>("InterfacePrefabs/LoseWindowUI");
        tutorialWindowPref = Resources.Load<TutorialWindowUI>("InterfacePrefabs/TutorialWindowUI");

        flyingTextPref = Resources.Load<TextScoreUpLR>("InterfacePrefabs/FlyingText");
        flyingTextUIPref = Resources.Load<TextScoreUpLR>("InterfacePrefabs/FlyingTextUI");

        if (winWindowPref == null)
            Debug.LogError("InterfaceManager: winWindowPref Prefabs not found in MyResources/InterfacePrefabs.");

        if (loseWindowPref == null)
            Debug.LogError("InterfaceManager: winWindowPref Prefabs not found in MyResources/InterfacePrefabs.");

        if (pauseWindowPref == null)
            Debug.LogError("InterfaceManager: pauseWindowPref Prefabs not found in MyResources/InterfacePrefabs.");

        BarMediator = new BarUIMediator();
        BarMediator.UpdateLinks();

        blackScreen = Resources.Load<BlackScreen>("InterfacePrefabs/BlackScreen");
    }

    /*─────────────────────  WINDOW FACTORY  ─────────────────────*/

    public static T CreateWindow<T>(T windowUIPref) where T : WindowUI
    {
        Debug.Log($"CreateWindow {typeof(T).ToString()}");
        if (mainCanvas == null || windowUIPref == null)
            return null;

        // Если уже открыто окно того же типа – просто активировать
        foreach (var w in createdWindows)
        {
            if (w is T existing)
            {
                existing.Open();          // «поднять» окно
                //blackScreen.Show();
                OnOpen?.Invoke(existing);
                return existing;
            }
        }

        T window = GameObject.Instantiate(windowUIPref, mainCanvas.transform);
        window.Open();
        createdWindows.Add(window);
        //blackScreen.Show();
        OnOpen?.Invoke(window);
        return window;
    }

    public static void CloseWindowWork(WindowUI window)
    {
        /*bool hideBlack = true;
        foreach (var w in createdWindows)
        {
            if (w.IsOpen && w.UseBlackScreen && w != window)
            {
                hideBlack = false;
                break;
            }
        }
        if(hideBlack && blackScreen != null)
            blackScreen.Hide();*/

        OnClose?.Invoke(window);
    }

    /*──────────────────────── SHOW HELPERS ──────────────────────*/

    public static void ShowPauseWindow()
    {
        Debug.Log($"ShowPauseWindow");
        if (pauseWindowPref == null) return;
        var pause = CreateWindow(pauseWindowPref);
        pause?.Show();
    }

    public static void ClosePauseWindow()
    {
        var pause = createdWindows.FirstOrDefault(w => w.TryGetComponent(out PauseWindowUI pause) && pause.IsOpen) as PauseWindowUI;
        if (pause == null) return;
        pause.Close();
    }

    public static void SwitchPause()
    {
        bool isOpen = createdWindows.Any(w => w.TryGetComponent(out PauseWindowUI pause) && pause.IsOpen);
        if (isOpen) ClosePauseWindow();
        else ShowPauseWindow();
    }

    public static void ShowWinWindow(int currentScore, int bestScore)
    {
        if (winWindowPref == null) return;
        var win = CreateWindow(winWindowPref);
        win?.Show(currentScore, bestScore);
    }

    public static void ShowLoseWindow(int currentScore, int bestScore)
    {
        if (loseWindowPref == null) return;
        var win = CreateWindow(loseWindowPref);
        win?.Show(currentScore, bestScore);
    }

    public static T CreateAndShowWindow<T>() where T : WindowUI
    {
        T pref = Resources.Load<T>($"InterfacePrefabs/{typeof(T)}");

        if (pref == null)
        {
            Debug.LogError($"InterfaceManager: {typeof(T).ToString()} Prefabs not found in MyResources/InterfacePrefabs.");
            return null;
        }

        var window = CreateWindow(pref);
        return window;
    }

    /*───────────────────────  HOUSEKEEPING  ─────────────────────*/

    /// <summary>Закрывает и уничтожает все открытые окна.</summary>
    public static void ClearAllWindows()
    {
        foreach (var w in createdWindows.ToArray())
        {
            if (w == null) continue;
            w.Close();
            GameObject.Destroy(w.gameObject);
        }
        createdWindows.Clear();
    }

    /*────────────────────────  BUTTON API  ──────────────────────*/

    public static void ClickButton(InterfaceComand cmd)
    {
        switch (cmd)
        {
            case InterfaceComand.SwitchPause: SwitchPause(); break;
            case InterfaceComand.OpenPause: ShowPauseWindow(); break;
            case InterfaceComand.ClosePause: ClosePauseWindow(); break;
            case InterfaceComand.Exit: Application.Quit(); break;
            case InterfaceComand.GoToMenu: GameSceneManager.LoadMenu(); break;
            case InterfaceComand.PlayGame: GameSceneManager.LoadGame(); break;
            default: break;
        }
        OnClickCommand?.Invoke(cmd);
    }

    /*──────────────────── OPTIONAL: UI CHECK ────────────────────*/

    /// <summary>True, если курсор/тап над UI-элементом.</summary>
    public static bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;
        if (Application.isMobilePlatform)
            return Input.touchCount > 0 &&
                   EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        return EventSystem.current.IsPointerOverGameObject();
    }

    public static void CreateScoreFlyingText(int scoreChange, Vector3 point)
    {
        CreateScoreFlyingText(scoreChange, point, null, false, null);
    }

    public static void CreateScoreFlyingText(int scoreChange, Vector3 point,
        float? sizeKof)
    {
        CreateScoreFlyingText(scoreChange, point, null, false, sizeKof: sizeKof);
    }

    public static void CreateScoreFlyingText(int scoreChange, Vector3 point, Transform parent, bool isUI,
        float? sizeKof = null)
    {
        CreateScoreFlyingText("", scoreChange, "", point, parent, isUI, sizeKof: sizeKof);
    }

    public static void CreateScoreFlyingText(string prefix, int scoreChange, string postfix, Vector3 point, Transform parent, bool isUI = false,
        float? sizeKof = null)
    {
        Color color = Color.yellow;
        if (scoreChange > 0)
        {
            prefix += "+";
        }
        else
        if (scoreChange == 0)
        {
            prefix += "+";
            color = Color.gray;
        }
        else
        if (scoreChange < 0)
        {
            color = Color.red;
        }

        TextScoreUpLR textScoreUp = CreateFlyingText(prefix + scoreChange.ToString() + postfix, color, point, parent, isUI);
        if (textScoreUp && sizeKof != null)
        {
            textScoreUp.transform.localScale *= Mathf.Clamp((float)sizeKof * scoreChange, 0.65f, 3f);
        }
    }

    public static TextScoreUpLR CreateFlyingText(string text, Color color, Vector3 point, Transform parent, bool isUI = false)
    {
        if (point == Vector3.zero)        
            return null;        

        TextScoreUpLR textScoreUp;
        if (isUI)
        {
            textScoreUp = GameObject.Instantiate(flyingTextUIPref, parent ? parent : mainCanvas.transform);
            DOVirtual.DelayedCall(Time.deltaTime, () =>
            textScoreUp.transform.localPosition = point);
        }
        else
            textScoreUp = GameObject.Instantiate(flyingTextPref, point, Quaternion.identity, parent);
        textScoreUp.Init(text, color, textScoreUp.transform.position);
        GameObject.Destroy(textScoreUp.gameObject, 10f);

        return textScoreUp;
    }

    public static void ShowBlackScreen(RectTransform rectTrWindow)
    {
        if (MainBlackScreen == null)
            return;

        BlackScreen blackScreen = UnityEngine.Object.Instantiate(MainBlackScreen, rectTrWindow.parent);
        blackScreen.transform.SetSiblingIndex(rectTrWindow.GetSiblingIndex());
        blackScreen.Show();

        blackScrees.Add(rectTrWindow, blackScreen);
    }

    public static void HideBlackScreen(RectTransform rectTrWindow)
    {
        if (blackScrees.TryGetValue(rectTrWindow, out BlackScreen blackScreen))
        {
            blackScreen.Hide(true);
            blackScrees.Remove(rectTrWindow);
        }
    }

    private static Dictionary<RectTransform, BlackScreen> blackScrees = new Dictionary<RectTransform, BlackScreen>();
}