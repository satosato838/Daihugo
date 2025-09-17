using System.Linq;
using System.Collections.Generic;

public class GamePlayer
{
    public int PlayerId;
    private bool isMyTurn;
    public bool IsMyTurn => isMyTurn;
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
        isMyTurn = val;
    }

    public void RefreshSelectableHandCards(List<TrumpCard> fieldCards)
    {
        if (fieldCards.Count == 0)
        {
            foreach (var card in handCards)
            {
                card.RefreshIsSelectable(true);
            }
        }
        else
        {
            var fieldNumber = (int)fieldCards.First().Number;
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
                    card.RefreshIsSelectable(cardCount >= fieldCards.Count && fieldNumber < (int)card.Number);
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

    public void PlayCards()
    {
        handCards.RemoveAll(c => c.IsSelect);
    }
}
