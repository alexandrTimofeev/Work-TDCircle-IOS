using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Audio;
using DG.Tweening;

public static class GameSettings
{
    public static AudioMixer GlobalMixer;

    public static Action<bool> OnSoundChange;
    public static Action<bool> OnVibrationChange;

    public static bool IsSoundPlay { get; private set; } = true;
    public static bool IsVibrationPlay { get; private set; } = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void AutoInit()
    {
        Debug.Log("AutoInit Settings");
        DOVirtual.DelayedCall(Time.deltaTime, () =>
        {
            AutoLoadMixer();
            LoadSettings();
            UpdateSettings();
        });
    }

    public static void SetSound (bool play)
    {
        if (IsSoundPlay == play)
            return;

        IsSoundPlay = play;
        PlayerPrefs.SetInt("Settings_Sound", play ? 1 : 0);

        if (GlobalMixer)
        {
            GlobalMixer.SetFloat("_GlobalVolume", play ? 0f : -80f);
        }

        OnSoundChange?.Invoke(play);
    }

    public static void SetVibration(bool play)
    {
        if (IsVibrationPlay == play)
            return;

        IsVibrationPlay = play;
        PlayerPrefs.SetInt("Settings_Vibration", play ? 1 : 0);

        OnVibrationChange?.Invoke(play);
    }

    public static void UpdateSettings ()
    {
        if (GlobalMixer)
            GlobalMixer.SetFloat("_GlobalVolume", IsSoundPlay ? 0f : -80f);

        OnSoundChange?.Invoke(IsSoundPlay);
        OnVibrationChange?.Invoke(IsVibrationPlay);
    }

    public static void LoadSettings ()
    {
        SetSound(PlayerPrefs.GetInt("Settings_Sound", 1) == 1);
        SetVibration(PlayerPrefs.GetInt("Settings_Vibration", 1) == 1);
    }

    public static void AutoLoadMixer()
    {
        GlobalMixer = Resources.Load<AudioMixer>("GlobalMixer");
    }
}