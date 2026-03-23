
using System;
using UnityEngine;

public class TimerStarter : MonoBehaviour
{
    [SerializeField] private bool playOnStart;
    public bool IsUnTimeScale;
    public GameTimer Timer;

    void Start()
    {
        if (playOnStart)
            Play();
    }

    public void Play(int seconds)
    {
        Timer = new GameTimer(seconds, 100, TimerDirection.Down);
        Play();
    }

    public void Play()
    {
        Timer.Start();
    }

    void Update()
    {
        if(Timer != null)
            Timer.Update(IsUnTimeScale ? Time.unscaledDeltaTime : Time.deltaTime);
    }
}