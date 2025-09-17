
using System;
using System.Collections.Generic;
using System.Linq;

public class Daihugo
{
    private int GamePlayMemberCount => 4;
    private List<TrumpCard> DeckCards;
    private List<GamePlayer> gamePlayers;
    public List<GamePlayer> GamePlayers => gamePlayers;
    private List<List<TrumpCard>> FieldCards;
    public List<TrumpCard> FieldCardPairs => FieldCards.Count == 0 ? new List<TrumpCard>() : FieldCards.Last();
    private List<TrumpCard> CemeteryCards;

    private int currentPlayerId = 0;
    public int CurrentPlayerId => currentPlayerId;

    private int GetNextPlayerId => currentPlayerId + 1 >= GamePlayMemberCount ? 0 : currentPlayerId + 1;

    public Daihugo()
    {
        RoundStart();
        // foreach (var player in gamePlayers)
        // {
        //     Debug.Log($"player:" + player.PlayerId);
        //     var cardCount = 0;
        //     foreach (var item in player.Hand)
        //     {
        //         Debug.Log($"{cardCount} item:" + item.CardName);
        //         cardCount++;
        //     }
        // }

    }

    public void RoundStart()
    {
        DeckCards = new List<TrumpCard>();
        FieldCards = new List<List<TrumpCard>>();
        CemeteryCards = new List<TrumpCard>();
        foreach (var type in DaihugoGameRule.SuitTypes)
        {
            for (var i = 1; i < DaihugoGameRule.Numbers.Length; i++)
            {
                DeckCards.Add(new TrumpCard(type, new CardNumber(i)));
            }
        }
        DeckCards.Add(new TrumpCard(DaihugoGameRule.SuitType.Joker, new CardNumber(DaihugoGameRule.Numbers.Length)));
        DeckCards = DeckCards.OrderBy(a => Guid.NewGuid()).ToList();
        gamePlayers = new List<GamePlayer>();
        for (var i = 0; i < GamePlayMemberCount; i++)
        {
            gamePlayers.Add(new GamePlayer(i, DealTheCards()));
        }
        //ランダムなプレイヤーに余ったカードを配る
        Random rnd = new Random();
        DealLastCard(rnd.Next(1, GamePlayMemberCount));

        ChangeNextPlayerTurn(GamePlayers.First().PlayerId);
    }

    private List<TrumpCard> DealTheCards()
    {
        var result = new List<TrumpCard>();
        for (var j = 0; j < DaihugoGameRule.Numbers.Length - 1; j++)
        {
            var card = DeckCards.First();
            result.Add(card);
            DeckCards.Remove(card);
        }
        return result;
    }

    private void DealLastCard(int index)
    {
        var hand = gamePlayers[index].Hand;
        var card = DeckCards.Last();
        hand.Add(card);
        DeckCards.Remove(card);
        gamePlayers[index].DealCard(hand);
    }

    public void RefreshPlayFieldCards(List<TrumpCard> playCards)
    {
        FieldCards.Add(playCards);
        ChangeNextPlayerTurn(GetNextPlayerId);
    }

    private void ChangeNextPlayerTurn(int nextPlayerId)
    {
        currentPlayerId = nextPlayerId;
        for (var i = 0; i < gamePlayers.Count; i++)
        {
            gamePlayers[i].IsMyTurn = i == nextPlayerId;
        }
    }

    public void RefreshCemeteryCards(List<TrumpCard> cards)
    {
        CemeteryCards = cards;
    }

}
