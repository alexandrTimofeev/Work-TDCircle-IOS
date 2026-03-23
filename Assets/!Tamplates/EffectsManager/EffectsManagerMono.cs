using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class EffectsManagerMono : MonoBehaviour
{
    [SerializeField] private float defaultHitStop = 0.5f;

    public static EffectsManagerMono Instance { get; private set; }

    public static bool IsHitStop => Instance != null && Instance._hitStopRoutine != null;

    private Coroutine _hitStopRoutine;
    private float _cachedTimeScale = 1f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void HitStop(float duration = -1f, float delay = 0.25f)
    {
        if (Instance == null)
            return;

        DOVirtual.DelayedCall(delay, () => Instance.StartHitStop(duration));
    }

    public static void CameraShake(Vector3 cameraShakePunch, float duration = 0.5f, Camera camera = null, bool isIndependentUpdate = false)
    {
        if (Instance == null)
            return;

        Instance.StartCameraShake(cameraShakePunch, duration, camera, isIndependentUpdate);
    }

    private void StartCameraShake(Vector3 cameraShakePunch, float duration = 0.5f, Camera camera = null, bool isIndependentUpdate = false)
    {
        if(camera == null)
            camera = Camera.main;   

        camera.transform.DOPunchPosition(cameraShakePunch, duration).SetUpdate(isIndependentUpdate);
    }

    private void StartHitStop(float duration)
    {
        if (_hitStopRoutine != null)
        {
            StopCoroutine(_hitStopRoutine);
            RestoreTime();
        }

        if (duration < 0f)
            duration = defaultHitStop;

        _hitStopRoutine = StartCoroutine(HitStopRoutine(duration));
    }

    private IEnumerator HitStopRoutine(float duration)
    {
        _cachedTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        RestoreTime();
        _hitStopRoutine = null;
    }

    private void RestoreTime()
    {
        Time.timeScale = _cachedTimeScale;
    }
}