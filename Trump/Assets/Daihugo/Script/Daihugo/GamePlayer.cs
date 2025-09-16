using System.Collections.Generic;

public class GamePlayer
{
    public int PlayerId;
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





}
