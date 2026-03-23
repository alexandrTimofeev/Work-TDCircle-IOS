using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameLoopContext
{
    public GameMain Game { get; }

    public bool RestartRequested { get; private set; }
    public string JumpToStepId { get; private set; }
    public bool HasExternalStepRequest { get; private set; }

    public GameLoopContext(GameMain game) => Game = game;

    public void Restart() => RestartRequested = true;
    public void JumpTo(string stepId) => JumpToStepId = stepId;
    public void RequestStepTransition() => HasExternalStepRequest = true;

    public event Action<IGameStep, Type, object> OnStepAction;

    public void Clear()
    {
        RestartRequested = false;
        JumpToStepId = null;
        HasExternalStepRequest = false;
    }

    public void InvokeAction (IGameStep step, Type type, object data)
    {
        OnStepAction?.Invoke(step, type, data);
    }
}

public class GameLoop
{
    private readonly List<IGameStep> _steps;
    public IGameStep[] Steps => _steps.ToArray();

    public GameLoop(IEnumerable<IGameStep> steps)
    {
        _steps = new List<IGameStep>(steps);
    }

    public IEnumerator Run(GameLoopContext context)
    {
        int index = 0;

        while (true)
        {
            if (index >= _steps.Count)
                index = 0;

            var step = _steps[index];
            context.Game.InvokeStepEvent(step);
            yield return step.Execute(context);

            if (context.RestartRequested)
            {
                context.Clear();
                index = 0;
                continue;
            }

            if (!string.IsNullOrEmpty(context.JumpToStepId))
            {
                index = _steps.FindIndex(s => s.Id == context.JumpToStepId);
                context.Clear();
                continue;
            }

            index++;
        }
    }
}