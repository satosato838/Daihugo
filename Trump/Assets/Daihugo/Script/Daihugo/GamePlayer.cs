using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GamePlayer
{
    public int PlayerId;
    public List<TrumpCard> CurrentSelectCards => HandCards.Where(c => c.IsSelect).ToList();
    private List<TrumpCard> HandCards;
    public List<TrumpCard> Hand => HandCards;
    public GamePlayer(int id, List<TrumpCard> cards)
    {
        PlayerId = id;
        DealCard(cards);
    }

    public void DealCard(List<TrumpCard> cards)
    {
        HandCards = cards;
        HandCards = BubbleSortCard();
    }

    private List<TrumpCard> BubbleSortCard()
    {
        var results = HandCards;
        for (int i = 0; i < results.Count; i++)
        {
            for (int j = i; j < results.Count; j++)
            {
                if (results[i].cardNumber.Number < results[j].cardNumber.Number)
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

    public void SelectCard(TrumpCard trumpCard)
    {
        foreach (var item in HandCards)
        {
            if (item.cardNumber.Number == trumpCard.cardNumber.Number &&
                item.Suit == trumpCard.Suit)
            {
                item.IsSelect = !item.IsSelect;
            }
        }
    }
}
