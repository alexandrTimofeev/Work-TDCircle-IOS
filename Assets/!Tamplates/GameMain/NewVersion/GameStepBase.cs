using System.Collections;

public interface IGameStep
{
    string Id { get; }
    IEnumerator Execute(GameLoopContext context);
}

public abstract class GameStepBase : IGameStep
{
    public abstract string Id { get; }
    public virtual bool CanExecute(GameLoopContext context) => true;

    public IEnumerator Execute(GameLoopContext context)
    {
        if (!CanExecute(context)) yield break;
        yield return OnExecute(context);
        yield return OnContinuity(context);
    }

    protected abstract IEnumerator OnExecute(GameLoopContext context);
    protected virtual IEnumerator OnContinuity(GameLoopContext context)
    {
        yield break;
    }
}

public abstract class GameStepHold<TContinuity> : GameStepBase<TContinuity>
    where TContinuity : class, IPlayGameBase
{
    protected sealed override IEnumerator OnExecute(GameLoopContext context)
    {
        yield break;
    }

    protected sealed override IEnumerator OnContinuity(GameLoopContext context)
    {
        yield return GameMain.WaitAllContunityProcessStart<TContinuity>();

        while (!context.HasExternalStepRequest)
        {
            GameMain.UpdateAllContinuity<TContinuity>();
            yield return null;
            yield return GameMain.WaitAllContunityProcessClear<TContinuity>();
        }
    }
}

public abstract class GameStepBase<TContinuity> : IGameStep where TContinuity : IPlayGameBase
{
    public abstract string Id { get; }

    public IEnumerator Execute(GameLoopContext context)
    {
        yield return OnExecute(context);
        yield return OnContinuity(context);
    }

    protected abstract IEnumerator OnExecute(GameLoopContext context);

    protected virtual IEnumerator OnContinuity(GameLoopContext context)
    {
        yield break;
    }
}


public abstract class GameStepHoldUpdate<TContinuity> : GameStepBase
    where TContinuity : class, IPlayGameBase
{
    public abstract override string Id { get; }

    protected override IEnumerator OnExecute(GameLoopContext context)
    {
        UnityEngine.Debug.Log($"[GameStepHoldUpdate] OnExecute {Id}");
        yield break;
    }

    protected override IEnumerator OnContinuity(GameLoopContext context)
    {
        UnityEngine.Debug.Log($"[GameStepHoldUpdate] OnContinuity {Id}");

        // Одноразовый вызов PlayGame
        yield return GameMain.WaitAllContunityProcessStart<TContinuity>();

        // Каждый кадр до внешнего запроса
        while (!context.HasExternalStepRequest)
        {
            GameMain.UpdateAllContinuity<TContinuity>();
            yield return GameMain.WaitAllUpdateContunity<TContinuity>();
            yield return null;

            yield return GameMain.WaitAllContunityProcessClear<TContinuity>();
        }
    }
}