using System;

public interface IDaihugoObserver
{
    void OnStartSet();
    void OnStartRound();
    void OnChangePlayerTurn(GamePlayer gamePlayer);
    void OnKakumei(DaihugoGameRule.DaihugoState state);
    void OnCardEffect(DaihugoGameRule.Effect effect);
    void OnEndRound();
    void OnEndSet();
}
public interface IDaihugoObservable
{
    IDisposable Subscribe(IDaihugoObserver observer);
}