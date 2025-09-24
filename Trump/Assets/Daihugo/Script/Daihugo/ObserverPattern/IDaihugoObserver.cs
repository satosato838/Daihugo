using System;

public interface IDaihugoObserver
{
    void OnStartSet();
    void OnStartRound();
    void OnChangePlayerTurn(GamePlayer gamePlayer);
    void OnKakumei(DaihugoGameRule.DaihugoState state);
    void OnCardEffect(DaihugoGameRule.Effect effect);
    void OnDaihugoStateEffect(DaihugoGameRule.DaihugoState state);
    void OnEndRound();
    void OnToGoOut();
    void OnEndSet();
}
public interface IDaihugoObservable
{
    IDisposable Subscribe(IDaihugoObserver observer);
}