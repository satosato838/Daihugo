
using System;
using System.Collections.Generic;
using System.Linq;

public class Daihugo : IDaihugoObservable
{
    private int GamePlayMemberCount => 4;
    private List<TrumpCard> DeckCards;
    private List<GamePlayer> gamePlayers;
    public List<GamePlayer> GamePlayers => gamePlayers;
    private List<List<TrumpCard>> fieldCards;
    public List<TrumpCard> LastFieldCardPair => fieldCards.Count == 0 ? new List<TrumpCard>() : fieldCards.Last();
    private List<TrumpCard> cemeteryCards;
    public List<TrumpCard> CemeteryCards => cemeteryCards;

    private int currentPlayerId = 0;
    public int CurrentPlayerId => currentPlayerId;
    private int PassCount = 0;
    private int lastPlayCardPlayerId = 0;
    public int LastPlayCardPlayerId => lastPlayCardPlayerId;
    private int GetRandomPlayerIndex()
    {
        Random rnd = new Random();
        return rnd.Next(1, GamePlayMemberCount);
    }

    private int GetNextPlayerId => currentPlayerId + 1 >= GamePlayMemberCount ? 0 : currentPlayerId + 1;

    private List<IDaihugoObserver> observers = new List<IDaihugoObserver>();
    public IDisposable Subscribe(IDaihugoObserver observer)
    {
        if (!observers.Contains(observer))
            observers.Add(observer);
        return new Unsubscriber(observers, observer);
    }

    public Daihugo()
    {

    }

    public void SetStart()
    {
        DeckCards = new List<TrumpCard>();
        fieldCards = new List<List<TrumpCard>>();
        cemeteryCards = new List<TrumpCard>();

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
        DealLastCard(GetRandomPlayerIndex());
        lastPlayCardPlayerId = GamePlayers.First().PlayerId;
        PassCount = 0;
        SendStartSet();
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

    public void StartRound()
    {
        ChangeNextPlayerTurn(lastPlayCardPlayerId);
        SendStartRound();
    }
    public void PlayFieldCards(List<TrumpCard> playCards)
    {
        if (playCards.Count() == 0)
        {
            PassCount++;
        }
        else
        {
            fieldCards.Add(playCards);
            PassCount = 0;
            lastPlayCardPlayerId = CurrentPlayerId;
        }

        if (PassCount == GamePlayMemberCount - 1)
        {
            EndRound();
        }
        else
        {
            ChangeNextPlayerTurn(GetNextPlayerId);
        }
    }

    private void ChangeNextPlayerTurn(int nextPlayerId)
    {
        currentPlayerId = nextPlayerId;
        for (var i = 0; i < gamePlayers.Count; i++)
        {
            gamePlayers[i].RefreshIsMyturn(i == nextPlayerId);
        }
        SendPlayerChange();
    }

    public void RefreshCemeteryCards(List<TrumpCard> cards)
    {
        cemeteryCards = cards;
    }

    private void EndRound()
    {
        RefreshCemeteryCards(fieldCards.SelectMany(v => v).ToList());
        fieldCards = new List<List<TrumpCard>>();
        ChangeNextPlayerTurn(LastPlayCardPlayerId);
        SendEndRound();
    }

    private void EndSet()
    {
        //todo create result
        //todo NextSet
        SendEndSet();
    }

    public void SendStartSet()
    {
        foreach (var observer in observers)
        {
            observer.OnStartSet();
        }
    }

    public void SendStartRound()
    {
        foreach (var observer in observers)
        {
            observer.OnStartRound();
        }
    }
    public void SendPlayerChange()
    {
        foreach (var observer in observers)
        {
            observer.OnChangePlayerTurn(GamePlayers.First(p => p.PlayerId == GetNextPlayerId));
        }
    }

    public void SendEndRound()
    {
        foreach (var observer in observers)
        {
            observer.OnEndRound();
        }
    }

    public void SendEndSet()
    {
        foreach (var observer in observers)
        {
            observer.OnEndSet();
        }
    }
}