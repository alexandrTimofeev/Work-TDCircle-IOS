using UnityEngine;

public class SmartOrientationController : MonoBehaviour
{
    [SerializeField] private bool autoRotateEnabled = true;
    [SerializeField] private DeviceOrientation lockOnStart = DeviceOrientation.Unknown;
    [SerializeField] private bool rotateOnStart = true;

    private static DeviceOrientation lastOrientation = DeviceOrientation.Unknown;
    private static DeviceOrientation forcedLockOrientation = DeviceOrientation.Unknown;

    private DeviceOrientation orientationCurrent;

    private void Start()
    {
        if (rotateOnStart == false)
            return;

        if (lockOnStart != DeviceOrientation.Unknown)
        {
            LockOrientation(lockOnStart);
        }
        else
        {
            EnableAutoRotation();
        }
    }

    private void Update()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (forcedLockOrientation != DeviceOrientation.Unknown)
        {
            if (!IsCurrentOrientationMatching(forcedLockOrientation))
            {
                SetUnityOrientation(forcedLockOrientation);
                SetNativeOrientation(forcedLockOrientation);
            }
            return;
        }

        if (!autoRotateEnabled) return;

        DeviceOrientation current = Input.deviceOrientation;
        if (current == DeviceOrientation.Unknown ||
            current == DeviceOrientation.FaceUp ||
            current == DeviceOrientation.FaceDown)
            return;

        if (current != lastOrientation)
        {
            lastOrientation = current;
            SetUnityOrientation(current);
            SetNativeOrientation(current);
        }
#endif
    }

    public void EnableAutoRotation(
        bool allowPortraitUpsideDown = true,
        bool allowLandscapeBothSides = true)
    {
        autoRotateEnabled = true;
        forcedLockOrientation = DeviceOrientation.Unknown;

#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            activity.Call("setRequestedOrientation", -1); // SCREEN_ORIENTATION_UNSPECIFIED
        }
#endif

        Screen.orientation = ScreenOrientation.AutoRotation;

        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = allowPortraitUpsideDown;
        Screen.autorotateToLandscapeLeft = allowLandscapeBothSides;
        Screen.autorotateToLandscapeRight = allowLandscapeBothSides;
    }

    public void LockOrientation(DeviceOrientation orientation)
    {
        forcedLockOrientation = orientation;
        autoRotateEnabled = false;
        SetUnityOrientation(orientation);
        SetNativeOrientation(orientation);
    }

    private void SetUnityOrientation(DeviceOrientation orientation)
    {
        orientationCurrent = orientation;

        switch (orientation)
        {
            case DeviceOrientation.Portrait:
                Screen.orientation = ScreenOrientation.Portrait;
                break;
            case DeviceOrientation.PortraitUpsideDown:
                Screen.orientation = ScreenOrientation.PortraitUpsideDown;
                break;
            case DeviceOrientation.LandscapeLeft:
                Screen.orientation = ScreenOrientation.LandscapeLeft;
                break;
            case DeviceOrientation.LandscapeRight:
                Screen.orientation = ScreenOrientation.LandscapeRight;
                break;
        }
    }

    private void SetNativeOrientation(DeviceOrientation orientation)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        int androidValue;

        switch (orientation)
        {
            case DeviceOrientation.Portrait: androidValue = 1; break;
            case DeviceOrientation.PortraitUpsideDown: androidValue = 9; break;
            case DeviceOrientation.LandscapeLeft: androidValue = 0; break;
            case DeviceOrientation.LandscapeRight: androidValue = 8; break;
            default: return;
        }

        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            activity.Call("setRequestedOrientation", androidValue);
        }
#endif
    }

    private bool IsCurrentOrientationMatching(DeviceOrientation target)
    {
        switch (target)
        {
            case DeviceOrientation.Portrait:
                return Screen.orientation == ScreenOrientation.Portrait;
            case DeviceOrientation.PortraitUpsideDown:
                return Screen.orientation == ScreenOrientation.PortraitUpsideDown;
            case DeviceOrientation.LandscapeLeft:
                return Screen.orientation == ScreenOrientation.LandscapeLeft;
            case DeviceOrientation.LandscapeRight:
                return Screen.orientation == ScreenOrientation.LandscapeRight;
            default:
                return false;
        }
    }

    // Удобные публичные методы
    public void LockPortrait() => LockOrientation(DeviceOrientation.Portrait);
    public void LockPortraitUpsideDown() => LockOrientation(DeviceOrientation.PortraitUpsideDown);
    public void LockLandscapeLeft() => LockOrientation(DeviceOrientation.LandscapeLeft);
    public void LockLandscapeRight() => LockOrientation(DeviceOrientation.LandscapeRight);
}