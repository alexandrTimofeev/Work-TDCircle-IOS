using System.Collections;
using UnityEngine;

[RequireComponent(typeof(GameTimerText))]
public class TimerBar : BarUI
{
    [SerializeField] private bool useMaxValue;
    private GameTimerText gameTimerText;

    private void Start()
    {
        gameTimerText = GetComponent<GameTimerText>();
    }

    public override void ShowMaxValueInInterface(float maxValue)
    {
        if (useMaxValue)
            gameTimerText.SetTimeText((int)maxValue);
    }

    public override void ShowValueInInterface(float currentValue)
    {
        gameTimerText.SetTimeText((int)currentValue);
    }
}