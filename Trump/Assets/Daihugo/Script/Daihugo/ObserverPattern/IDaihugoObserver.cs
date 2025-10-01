using System;

public interface IDaihugoObserver
{
    void OnStartRound(DaihugoGameRule.GameState state);
    void OnStartStage();
    void OnChangePlayerTurn(GamePlayer gamePlayer);
    void OnKakumei(DaihugoGameRule.DaihugoState state);
    void OnCardEffect(DaihugoGameRule.Effect effect);
    void OnGameState(DaihugoGameRule.GameState state);
    void OnDaihugoStateEffect(DaihugoGameRule.DaihugoState state);
    void OnEndStage(DaihugoGameRule.Effect effect);
    void OnToGoOut(int goOutPlayerIndex);
    void OnEndRound(DaihugoGameRule.GameState state);
}
public interface IDaihugoObservable
{
    IDisposable Subscribe(IDaihugoObserver observer);
}