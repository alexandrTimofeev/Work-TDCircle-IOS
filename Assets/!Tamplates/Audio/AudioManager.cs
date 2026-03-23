using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public static class AudioManager
{
    private static AudioSource musicSource;
    private static AudioLowPassFilter musicPassFilter;
    private static float speedMusicNormal = 1f;

    public static AudioSource MusicSource => musicSource;

    public static void Init()
    {
        musicSource = GameObject.Find("AudioSource(Music)")?.GetComponent<AudioSource>();
        if (musicSource)
        {
            musicPassFilter = musicSource.GetComponent<AudioLowPassFilter>();
            musicPassFilter.enabled = false;
            speedMusicNormal = musicSource.pitch;
        }
    }

    public static void PlayMusic()
    {
        if (musicSource == null)
            return;

        musicSource.Play();
    }
    public static void PauseMusic()
    {
        if (musicSource == null)
            return;

        musicSource.Pause();
    }
    public static void StopMusic()
    {
        if (musicSource == null)
            return;

        musicSource.Stop();
    }

    public static void SetSpeedMusic(float speedCoef = 1f)
    {
        if (musicSource == null)
            return;

        musicSource.pitch = speedMusicNormal * speedCoef;
    }

    public static void PassFilterMusic (bool isPassFilter)
    {
        if (musicSource == null)
            return;

        musicPassFilter.enabled = isPassFilter;
    }
}