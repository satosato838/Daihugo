
using System.Collections.Generic;

public class DaihugoSetResult
{
    private List<GamePlayer> ResultPlayers;
    public int ResultPlayersCount => ResultPlayers.Count;

    public DaihugoSetResult()
    {
        ResultPlayers = new List<GamePlayer>();
    }

    public void AddSetEndPlayer(GamePlayer gamePlayer, bool IsForbiddenWin)
    {
        gamePlayer.RefreshRank(IsForbiddenWin ? GetNextPlayersByDescendingRank() : GetNextPlayersByAscendingRank());
        ResultPlayers.Add(gamePlayer);
    }
    public DaihugoGameRule.GameRank GetNextPlayersByAscendingRank()
    {
        var ranks = new DaihugoGameRule.GameRank[] { DaihugoGameRule.GameRank.DaiHugo, DaihugoGameRule.GameRank.Hugo, DaihugoGameRule.GameRank.Hinmin, DaihugoGameRule.GameRank.DaiHinmin };
        return ranks[ResultPlayers.Count];
    }
    public DaihugoGameRule.GameRank GetNextPlayersByDescendingRank()
    {
        var ranks = new DaihugoGameRule.GameRank[] { DaihugoGameRule.GameRank.DaiHinmin, DaihugoGameRule.GameRank.Hinmin, DaihugoGameRule.GameRank.Hugo, DaihugoGameRule.GameRank.DaiHugo };
        return ranks[ResultPlayers.Count];
    }
}
