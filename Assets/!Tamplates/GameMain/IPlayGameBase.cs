using System.Collections;

public interface IPlayGameBase : IOrdered
{
    IEnumerator PlayGame();
}
public interface IPlayGameUpdateBase : IOrdered
{
    IEnumerator PlayGameUpdate();
}

public interface IPlayGame<in TStage>
    where TStage : class, IPlayGameBase
{
    IEnumerator PlayGame()
    {
        yield return (this as TStage).PlayGame();
    }
}

public interface IPlayGameUpdate<in TStage>
    where TStage : IPlayGameBase
{
    bool IsUseUpateRoutine();

    void UpdatePlayGame();
    IEnumerator PlayGameUpdate();
}

//-----------------------------------------
public interface IOrdered
{
    int Order();
}