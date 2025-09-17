using System;

public interface IDaihugoObserver
{
    void OnStartSet();
    void OnStartRound();
    void OnChangePlayerTurn(GamePlayer gamePlayer);
    void OnEndRound();
    void OnEndSet();
}
public interface IDaihugoObservable
{
    IDisposable Subscribe(IDaihugoObserver observer);
}