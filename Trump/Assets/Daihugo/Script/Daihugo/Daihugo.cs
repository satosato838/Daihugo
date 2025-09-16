
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Daihugo
{
    private int CardNumberLength => 14;

    public enum Number
    {
        Three = 0,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace,
        Two,
        Joker
    }

    public enum SuitType
    {
        Spade,
        Diamond,
        Clover,
        Heart,
        Joker
    }
    public enum Effect
    {
        None,
        Eight_Enders,
        Eleven_Back,
        Revolution,
        Counter_Revolution,
        Counter_Spade_3
    }
    SuitType[] suitTypes = { SuitType.Spade, SuitType.Diamond, SuitType.Clover, SuitType.Heart };
    private List<TrumpCard> Cards;
    private List<GamePlayer> gamePlayers;
    public List<GamePlayer> GamePlayers => gamePlayers;
    public Daihugo()
    {
        Cards = new List<TrumpCard>();
        foreach (var type in suitTypes)
        {
            for (var i = 1; i < CardNumberLength; i++)
            {
                Cards.Add(new TrumpCard(type, new CardNumber(i)));
            }
        }
        Cards.Add(new TrumpCard(SuitType.Joker, new CardNumber(14)));
        Cards = Cards.OrderBy(a => Guid.NewGuid()).ToList();
        gamePlayers = new List<GamePlayer>();
        for (var i = 0; i < 4; i++)
        {
            gamePlayers.Add(new GamePlayer(i, DealTheCards()));
        }

        DealLastCard(1);

        foreach (var player in gamePlayers)
        {
            Debug.Log($"player:" + player.PlayerId);
            var cardCount = 0;
            foreach (var item in player.Hand)
            {
                Debug.Log($"{cardCount} item:" + item.CardName);
                cardCount++;
            }
        }

    }

    private List<TrumpCard> DealTheCards()
    {
        var result = new List<TrumpCard>();
        for (var j = 0; j < 13; j++)
        {
            var card = Cards.First();
            result.Add(card);
            Cards.Remove(card);
        }
        return result;
    }

    private void DealLastCard(int index)
    {
        var hand = gamePlayers[index].Hand;
        var card = Cards.Last();
        hand.Add(card);
        Cards.Remove(card);
        gamePlayers[index].DealCard(hand);
    }

}
