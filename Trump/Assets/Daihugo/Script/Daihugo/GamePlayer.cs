using System.Linq;
using System.Collections.Generic;
using System;

public class GamePlayer
{
    public int PlayerId;
    private bool isPlay;
    public bool IsPlay => isPlay;
    private DaihugoGameRule.GameRank playerRank = DaihugoGameRule.GameRank.Heimin;
    public DaihugoGameRule.GameRank PlayerRank => playerRank;
    private bool isMyTurn;
    public bool IsMyTurn => isMyTurn;
    public List<TrumpCard> CurrentSelectCards => handCards.Where(c => c.IsSelect).ToList();
    private List<TrumpCard> FeildCards;
    private List<TrumpCard> handCards;
    public List<TrumpCard> Hand => handCards;
    public bool IsCardPlay => FeildCards.Count == 0 ? true : FeildCards.Count == CurrentSelectCards.Count;

    public GamePlayer(int id, List<TrumpCard> cards, DaihugoGameRule.GameRank rank)
    {
        PlayerId = id;
        FeildCards = new List<TrumpCard>();
        RefreshIsPlay(true);
        RefreshRank(rank);
        DealCard(cards);
    }
    public GamePlayer(int id)
    {
        PlayerId = id;
        FeildCards = new List<TrumpCard>();
    }

    public void RefreshRank(DaihugoGameRule.GameRank rank)
    {
        playerRank = rank;
    }

    public void RefreshIsMyturn(bool val)
    {
        isMyTurn = val;
    }

    public void RefreshIsPlay(bool val)
    {
        isPlay = val;
    }



    public void RefreshSelectableHandCards(List<TrumpCard> fieldCards)
    {
        FeildCards = fieldCards;
        if (FeildCards.Count == 0)
        {
            foreach (var card in handCards)
            {
                card.RefreshIsSelectable(true);
            }
        }
        else
        {
            var fieldNumber = (int)FeildCards.First().Number;
            foreach (var card in handCards)
            {
                if (card.Number == DaihugoGameRule.Number.Joker)
                {
                    var number = handCards.Any(v => (int)v.Number > fieldNumber);
                    card.RefreshIsSelectable(number);
                }
                else
                {
                    var cardCount = handCards.Count(v => v.Number == card.Number || v.Number == DaihugoGameRule.Number.Joker);
                    card.RefreshIsSelectable(cardCount >= FeildCards.Count && fieldNumber < (int)card.Number);
                }
            }
        }
    }


    public void DealCard(List<TrumpCard> cards)
    {
        handCards = cards;
        handCards = BubbleSortCard();
    }

    private List<TrumpCard> BubbleSortCard()
    {
        var results = handCards;
        for (int i = 0; i < results.Count; i++)
        {
            for (int j = i; j < results.Count; j++)
            {
                if (results[i].Number < results[j].Number)
                {
                    var x = results[j];
                    results[j] = results[i];
                    results[i] = x;
                }
            }
        }
        results.Reverse();
        return results;
    }

    public void PlayCards(Action<int> callback)
    {
        handCards.RemoveAll(c => c.IsSelect);
        callback.Invoke(handCards.Count);
    }
}
