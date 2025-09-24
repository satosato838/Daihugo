
using System.Collections.Generic;
using System.Linq;

public class DaihugoRoundResult
{
    private List<GamePlayer> ResultPlayers;
    public int ResultPlayersCount => ResultPlayers.Count;

    public DaihugoRoundResult()
    {
        ResultPlayers = new List<GamePlayer>();
    }

    public void AddRoundEndPlayer(GamePlayer gamePlayer, bool IsForbiddenWin)
    {
        gamePlayer.RefreshRank(IsForbiddenWin ? GetNextPlayersByDescendingRank() : GetNextPlayersByAscendingRank());
        ResultPlayers.Add(gamePlayer);
    }
    public DaihugoGameRule.GameRank GetPlayerIdRank(int playerId)
    {
        if (ResultPlayers.Any(p => p.PlayerId == playerId))
        {
            return ResultPlayers.Find(p => p.PlayerId == playerId).PlayerRank;
        }
        else
        {
            throw new System.Exception("not found player");
        }
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
