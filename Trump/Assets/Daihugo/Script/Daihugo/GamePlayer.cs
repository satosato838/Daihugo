using System.Linq;
using System.Collections.Generic;

public class GamePlayer
{
    public int PlayerId;
    public List<TrumpCard> CurrentSelectCards => handCards.Where(c => c.IsSelect).ToList();
    private List<TrumpCard> handCards;
    public List<TrumpCard> Hand => handCards;
    public GamePlayer(int id, List<TrumpCard> cards)
    {
        PlayerId = id;
        DealCard(cards);
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
        foreach (var item in handCards)
        {
            if (item.cardNumber.Number == trumpCard.cardNumber.Number &&
                item.Suit == trumpCard.Suit)
            {
                item.IsSelect = !item.IsSelect;
            }
        }
    }

    public void PlayCards()
    {
        handCards.RemoveAll(c => c.IsSelect);
    }
}
