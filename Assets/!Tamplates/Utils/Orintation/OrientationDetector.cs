using UnityEngine;
using UnityEngine.Events;

public class OrientationDetector : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent<bool> onPortraitCheck;
    public UnityEvent<bool> onLandscapeCheck;

    [Header("Portrait")]
    public UnityEvent onAnyPortrait;
    public UnityEvent onPortraitUp;
    public UnityEvent onPortraitUpsideDown;

    [Header("Landscape")]
    public UnityEvent onLandscape;
    public UnityEvent onLandscapeLeft;
    public UnityEvent onLandscapeRight;

    private ScreenOrientation _lastOrientation;

    void Start()
    {
        _lastOrientation = Screen.orientation;
        InvokeOrientationEvent(_lastOrientation);
    }

    void Update()
    {
        if (Screen.orientation != _lastOrientation)
        {
            _lastOrientation = Screen.orientation;
            InvokeOrientationEvent(_lastOrientation);
        }
    }

    private void InvokeOrientationEvent(ScreenOrientation orientation)
    {
        switch (orientation)
        {
            case ScreenOrientation.Portrait:
                onPortraitUp?.Invoke();
                onAnyPortrait?.Invoke();

                onPortraitCheck?.Invoke(true);
                onLandscapeCheck?.Invoke(false);
                break;

            case ScreenOrientation.PortraitUpsideDown:
                onPortraitUpsideDown?.Invoke();
                onAnyPortrait?.Invoke();

                onPortraitCheck?.Invoke(true);
                onLandscapeCheck?.Invoke(false);
                break;

            case ScreenOrientation.LandscapeLeft:
                onLandscape?.Invoke();
                onLandscapeLeft?.Invoke();

                onPortraitCheck?.Invoke(false);
                onLandscapeCheck?.Invoke(true);
                break;
            case ScreenOrientation.LandscapeRight:
                onLandscape?.Invoke();
                onLandscapeRight?.Invoke();

                onPortraitCheck?.Invoke(false);
                onLandscapeCheck?.Invoke(true);
                break;
            default:
                break;
        }
    }
}