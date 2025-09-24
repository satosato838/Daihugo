using System;

public interface IDaihugoObserver
{
    void OnStartRound();
    void OnStartStage();
    void OnChangePlayerTurn(GamePlayer gamePlayer);
    void OnKakumei(DaihugoGameRule.DaihugoState state);
    void OnCardEffect(DaihugoGameRule.Effect effect);
    void OnDaihugoStateEffect(DaihugoGameRule.DaihugoState state);
    void OnEndStage();
    void OnToGoOut(int goOutPlayerIndex);
    void OnEndRound();
}
public interface IDaihugoObservable
{
    IDisposable Subscribe(IDaihugoObserver observer);
}