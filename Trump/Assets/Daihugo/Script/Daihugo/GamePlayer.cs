using System.Linq;
using System.Collections.Generic;
using System;
using System.Diagnostics;

public class GamePlayer
{
    public int PlayerId;
    private bool isPlay;
    public bool IsPlay => isPlay;
    private DaihugoGameRule.DaihugoState gameState;
    public DaihugoGameRule.DaihugoState GameState => gameState;
    private DaihugoGameRule.GameRank playerRank = DaihugoGameRule.GameRank.Heimin;
    public DaihugoGameRule.GameRank PlayerRank => playerRank;
    private bool isMyTurn;
    public bool IsMyTurn => isMyTurn;
    public List<TrumpCard> CurrentSelectCards => handCards.Where(c => c.IsSelect).ToList();
    private List<TrumpCard> FeildCards;
    private List<TrumpCard> handCards;
    public List<TrumpCard> HandCards => handCards;
    public bool IsCardPlay => FeildCards.Count == 0 ? true : FeildCards.Count == CurrentSelectCards.Count;

    public GamePlayer(int id, List<TrumpCard> cards, DaihugoGameRule.GameRank rank, DaihugoGameRule.DaihugoState daihugoState)
    {
        PlayerId = id;
        FeildCards = new List<TrumpCard>();
        RefreshIsPlay(true);
        RefreshRank(rank);
        DealCard(cards);
        RefreshGameState(daihugoState);
    }
    public GamePlayer(int id)
    {
        PlayerId = id;
        FeildCards = new List<TrumpCard>();
    }

    public void RefreshGameState(DaihugoGameRule.DaihugoState daihugoState)
    {
        gameState = daihugoState;
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

    public void SelectCard(TrumpCard selectCard)
    {
        var index = handCards.FindIndex(c => c.Suit == selectCard.Suit && c.Number == selectCard.Number);
        handCards[index].RefreshIsSelect(selectCard.IsSelect);

        RefreshSelectableHandCards(FeildCards);
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
            var selectingOver = CurrentSelectCards.Count >= FeildCards.Count;
            if (selectingOver)
            {
                foreach (var card in handCards)
                {
                    if (!card.IsSelect) card.RefreshIsSelectable(!selectingOver);
                }
                return;
            }

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
                    var handCardCount = handCards.Count(v => v.Number == card.Number || v.Number == DaihugoGameRule.Number.Joker);
                    card.RefreshIsSelectable(IsPlayCard(handCardCount, FeildCards.Count, fieldNumber, (int)card.Number));
                }
            }
        }
    }

    private bool IsPlayCard(int handCardCount, int feildCardCount, int fieldNumber, int cardNumber)
    {
        if (gameState == DaihugoGameRule.DaihugoState.None)
        {
            return handCardCount >= feildCardCount && fieldNumber < cardNumber;
        }
        else //革命状態はelseの方の処理に流れる
        {
            return handCardCount >= feildCardCount && fieldNumber > cardNumber;
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
