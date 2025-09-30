
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


public class DaihugoRoundResult
{
    private List<GamePlayer> ResultPlayers;
    public int ResultPlayersCount => ResultPlayers.Count;
    public int CurrentRoundIndex;
    public List<GamePlayer> GetResultPlayers => ResultPlayers;

    public DaihugoRoundResult(int currentRoundIndex)
    {
        CurrentRoundIndex = currentRoundIndex;
        ResultPlayers = new List<GamePlayer>();
    }

    public void AddRoundEndPlayer(GamePlayer gamePlayer, bool IsForbiddenWin)
    {
        gamePlayer.RefreshRank(IsForbiddenWin ? GetNextPlayersByDescendingRank() : GetNextPlayersByAscendingRank());
        Debug.Log($"AddRoundEndPlayer gamePlayer Id:{gamePlayer.PlayerId} , Rank:{gamePlayer.PlayerRank}");
        ResultPlayers.Add(gamePlayer);
    }

    public void AddBoobyPlayer(GamePlayer gamePlayer)
    {
        gamePlayer.RefreshRank(ResultPlayers.Any(p => p.PlayerRank == DaihugoGameRule.GameRank.DaiHinmin) ? DaihugoGameRule.GameRank.Hinmin : DaihugoGameRule.GameRank.DaiHinmin);
        //Debug.Log($"AddBoobyPlayer gamePlayer Id:{gamePlayer.PlayerId} , Rank:{gamePlayer.PlayerRank}");
        ResultPlayers.Add(gamePlayer);
    }

    public void CreateDebugData()
    {
        var ids = new int[] { 0, 1, 2, 3 };
        var ranks = new DaihugoGameRule.GameRank[] { DaihugoGameRule.GameRank.DaiHugo, DaihugoGameRule.GameRank.Hugo, DaihugoGameRule.GameRank.Hinmin, DaihugoGameRule.GameRank.DaiHinmin };
        ids = ids.OrderBy(a => Guid.NewGuid()).ToArray();
        ranks = ranks.OrderBy(a => Guid.NewGuid()).ToArray();
        for (var i = 0; i < 4; i++)
        {
            var gamePlayer = new GamePlayer(ids[i], new List<TrumpCard>(), ranks[i], DaihugoGameRule.DaihugoState.None, DaihugoGameRule.GameState.CardChange);
            ResultPlayers.Add(gamePlayer);
        }
    }
    public DaihugoGameRule.GameRank GetPlayerIdRank(int playerId)
    {
        if (ResultPlayers.Any(p => p.PlayerId == playerId))
        {
            return ResultPlayers.Find(p => p.PlayerId == playerId).PlayerRank;
        }
        else
        {
            foreach (var item in ResultPlayers) Debug.LogError("ResultPlayer:" + item.PlayerId);
            Debug.LogError("GetPlayerIdRank CurrentRoundIndex:" + CurrentRoundIndex);
            throw new Exception("not found playerId:" + playerId);
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
    public List<GamePlayer> GetNextGamePlayers()
    {
        foreach (var player in ResultPlayers) player.RefreshIsPlay(true);
        return ResultPlayers.OrderByDescending(p => p.PlayerRank).ToList();
    }
}
