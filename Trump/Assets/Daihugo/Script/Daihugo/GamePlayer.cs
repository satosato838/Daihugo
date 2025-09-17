using System.Linq;
using System.Collections.Generic;

public class GamePlayer
{
    public int PlayerId;
    public bool IsMyTurn;
    public List<TrumpCard> CurrentSelectCards => handCards.Where(c => c.IsSelect).ToList();
    private List<TrumpCard> handCards;
    public List<TrumpCard> Hand => handCards;
    public GamePlayer(int id, List<TrumpCard> cards)
    {
        PlayerId = id;
        DealCard(cards);
    }

    public void RefreshIsMyturn(bool val)
    {
        IsMyTurn = val;
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

    public void PlayCards()
    {
        handCards.RemoveAll(c => c.IsSelect);
    }
}
