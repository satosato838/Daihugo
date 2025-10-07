using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayer
{
    public int PlayerId;
    public bool IsCPU;
    public string PlayerName;
    public string PlayerIconImageName;
    private bool isPlay;
    public bool IsPlay => isPlay;
    private DaihugoGameRule.DaihugoState _daihugoState;
    public DaihugoGameRule.DaihugoState DaihugoState => _daihugoState;
    private DaihugoGameRule.GameState _gameState;
    public DaihugoGameRule.GameState GameState => _gameState;
    private DaihugoGameRule.GameRank _playerRank = DaihugoGameRule.GameRank.Heimin;
    public DaihugoGameRule.GameRank PlayerRank => _playerRank;
    private bool isMyTurn;
    public bool IsMyTurn => isMyTurn;

    private bool isDealer;
    public bool IsDealer => isDealer;
    public List<TrumpCard> CurrentSelectCards => handCards.Where(c => c.IsSelect).ToList();
    private List<TrumpCard> fieldCards;
    private List<TrumpCard> handCards;
    public List<TrumpCard> HandCards => handCards;
    public bool IsCardPlay => fieldCards.Count == 0 ? true : fieldCards.Count == CurrentSelectCards.Count;
    public bool HavePlayCard => handCards.Any(c => c.IsSelectable);
    public bool IsCardChangePlay => CurrentSelectCards.Count == PlayerRank switch
    {
        DaihugoGameRule.GameRank.DaiHugo => 2,
        DaihugoGameRule.GameRank.Hugo => 1,
        DaihugoGameRule.GameRank.Hinmin => 1,
        DaihugoGameRule.GameRank.DaiHinmin => 2,
        _ => 0
    };

    public GamePlayer(int id, string playerName, string iconImageName, DaihugoGameRule.GameRank rank, DaihugoGameRule.DaihugoState daihugoState, DaihugoGameRule.GameState gameState, bool isCPU = false)
    {
        PlayerId = id;
        fieldCards = new List<TrumpCard>();
        PlayerName = playerName;
        PlayerIconImageName = iconImageName;
        IsCPU = isCPU;
        RefreshIsPlay(true);
        RefreshRank(rank);
        RefreshDaihugoState(daihugoState);
        RefreshGameState(gameState);
    }
    public GamePlayer(int id)
    {
        PlayerId = id;
        fieldCards = new List<TrumpCard>();
    }

    public void RefreshDaihugoState(DaihugoGameRule.DaihugoState daihugoState)
    {
        _daihugoState = daihugoState;
    }
    public void RefreshGameState(DaihugoGameRule.GameState gameState)
    {
        _gameState = gameState;
    }

    public void RefreshRank(DaihugoGameRule.GameRank rank)
    {
        _playerRank = rank;
    }

    public void RefreshIsMyturn(bool val)
    {
        isMyTurn = val;
    }

    public void RefreshIsPlay(bool val)
    {
        isPlay = val;
    }

    public void RefreshDealer(bool val)
    {
        isDealer = val;
    }

    public void SelectCard(TrumpCard selectCard)
    {
        if (_gameState == DaihugoGameRule.GameState.GamePlay)
        {
            var index = handCards.FindIndex(c => c.Suit == selectCard.Suit && c.Number == selectCard.Number);
            handCards[index].RefreshIsSelect(selectCard.IsSelect);

            RefreshSelectableHandCards(fieldCards);
        }
        else if (_gameState == DaihugoGameRule.GameState.CardChange)
        {
            UpdateSelectableCardsForExchange();
        }
    }

    public void RefreshSelectableHandCards(List<TrumpCard> fields)
    {
        fieldCards = fields;
        if (fieldCards.Count == 0)
        {
            foreach (var card in handCards)
            {
                card.RefreshIsSelectable(true);
            }
        }
        else
        {
            var selectingOver = CurrentSelectCards.Count >= fieldCards.Count;
            if (selectingOver)
            {
                foreach (var card in handCards)
                {
                    if (!card.IsSelect) card.RefreshIsSelectable(!selectingOver);
                }
                return;
            }

            var fieldNumber = (int)fieldCards.First().Number;
            foreach (var card in handCards)
            {
                //Joker1枚ならスペードの3以外出せない
                if (fieldCards.First().Number == DaihugoGameRule.Number.Joker && fieldCards.Count == 1)
                {
                    card.RefreshIsSelectable(card.Suit == DaihugoGameRule.SuitType.Spade && card.Number == DaihugoGameRule.Number.Three);
                    continue;
                }

                if (card.Number == DaihugoGameRule.Number.Joker)
                {
                    var number = handCards.Any(v => (int)v.Number > fieldNumber);
                    card.RefreshIsSelectable(number);
                }
                else
                {
                    var handCardCount = handCards.Count(v => v.Number == card.Number || v.Number == DaihugoGameRule.Number.Joker);
                    card.RefreshIsSelectable(IsPlayCard(handCardCount, fieldCards.Count, fieldNumber, (int)card.Number));
                }
            }
        }
    }

    //カードチェンジ時に選択可能なカードのステータス管理の仕組み
    public void UpdateSelectableCardsForExchange()
    {
        if (IsCardChangePlay)
        {
            foreach (var card in handCards)
            {
                card.RefreshIsSelectable(card.IsSelect);
            }
            return;
        }

        if (PlayerRank == DaihugoGameRule.GameRank.Hinmin ||
            PlayerRank == DaihugoGameRule.GameRank.DaiHinmin)
        {
            var strongestCards = GetStrongestCards(handCards);

            if (PlayerRank == DaihugoGameRule.GameRank.DaiHinmin)
            {
                if (strongestCards.Count == 1)
                {
                    // 大貧民は 2枚 → 1番強いカード群を除外して次点の強いカードも対象にする
                    var temp = handCards.ToList();
                    temp.RemoveAll(c => c.Number == strongestCards.First().Number);
                    strongestCards.AddRange(GetStrongestCards(temp));
                }
            }

            foreach (var card in handCards)
            {
                card.RefreshIsSelectable(strongestCards.Any(c => c.Number == card.Number));
            }
        }
        else
        {
            SetAllSelectable(true);
        }
        void SetAllSelectable(bool selectable)
        {
            foreach (var card in handCards)
            {
                card.RefreshIsSelectable(selectable);
            }
        }
    }

    public void CPUAutoSelectCardForExchange()
    {
        if (PlayerRank == DaihugoGameRule.GameRank.Hinmin ||
            PlayerRank == DaihugoGameRule.GameRank.DaiHinmin)
        {
            var strongestCards = GetStrongestCards(handCards);

            if (PlayerRank == DaihugoGameRule.GameRank.DaiHinmin)
            {
                if (strongestCards.Count == 1)
                {
                    // 大貧民は 2枚 → 1番強いカード群を除外して次点の強いカードも対象にする
                    var temp = handCards.ToList();
                    temp.RemoveAll(c => c.Number == strongestCards.First().Number);
                    strongestCards.AddRange(GetStrongestCards(temp));
                }
            }
            // foreach (var card in strongestCards)
            // {
            //     Debug.Log($"{PlayerName} CPUAutoSelectCardForExchange strongestCards {card.CardName}:" + card.IsSelect);
            // }

            foreach (var card in handCards)
            {
                card.RefreshIsSelect(strongestCards.Any(c => c.Number == card.Number));
            }
        }
        else
        {
            var weakestCards = GetWeakestCards(handCards);

            if (PlayerRank == DaihugoGameRule.GameRank.DaiHugo)
            {
                if (weakestCards.Count == 1)
                {
                    // 大富豪は 2枚 → 1番弱いカード群を除外して次点の強いカードも対象にする
                    var temp = handCards.ToList();
                    temp.RemoveAll(c => c.Number == weakestCards.First().Number);
                    weakestCards.AddRange(GetWeakestCards(temp));
                }
            }
            // foreach (var card in weakestCards)
            // {
            //     Debug.Log($"{PlayerName} CPUAutoSelectCardForExchange weakestCards {card.CardName}:" + card.IsSelect);
            // }

            foreach (var card in handCards)
            {
                card.RefreshIsSelect(weakestCards.Any(c => c.Number == card.Number));
            }
        }
        // foreach (var card in handCards)
        // {
        //     Debug.Log($"{PlayerName} CPUAutoSelectCardForExchange {card.CardName}:" + card.IsSelect);
        // }
    }


    List<TrumpCard> GetStrongestCards(List<TrumpCard> trumpCards)
    {
        //Debug.Log(PlayerId + " itrumpCards:" + trumpCards.Count);
        if (trumpCards.Count == 0) return new List<TrumpCard>();
        try
        {
            // foreach (var item in trumpCards)
            // {
            //     Debug.Log(PlayerId + " item CardName:" + item.CardName);
            // }
            var maxValue = trumpCards.Max(c => (int)c.Number);
            return trumpCards.Where(c => (int)c.Number == maxValue).ToList();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    List<TrumpCard> GetWeakestCards(List<TrumpCard> trumpCards)
    {
        //Debug.Log(PlayerId + " itrumpCards:" + trumpCards.Count);
        if (trumpCards.Count == 0) return new List<TrumpCard>();
        try
        {
            // foreach (var item in trumpCards)
            // {
            //     Debug.Log(PlayerId + " item CardName:" + item.CardName);
            // }
            var minValue = trumpCards.Min(c => (int)c.Number);
            return trumpCards.Where(c => (int)c.Number == minValue).ToList();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    private bool IsPlayCard(int handCardCount, int feildCardCount, int fieldNumber, int cardNumber)
    {
        if (_daihugoState == DaihugoGameRule.DaihugoState.None)
        {
            return handCardCount >= feildCardCount && fieldNumber < cardNumber;
        }
        else //革命状態はelseの方の処理に流れる
        {
            return handCardCount >= feildCardCount && fieldNumber > cardNumber;
        }

    }
    public void AddCards(List<TrumpCard> cards)
    {
        //Debug.Log("AddCards PlayerId:" + PlayerId);
        handCards.AddRange(cards);
        AllRefreshSelectCard();
        handCards = BubbleSortCard(handCards);
        //foreach (var item in handCards) Debug.Log("AddCards item:" + item.CardName);
    }
    public void DealCard(List<TrumpCard> cards)
    {
        handCards = cards;
        AllRefreshSelectCard();
        handCards = BubbleSortCard(handCards);
        if (GameState == DaihugoGameRule.GameState.CardChange)
        {
            if (IsCPU)
            {
                CPUAutoSelectCardForExchange();
            }
            else
            {
                UpdateSelectableCardsForExchange();
            }
        }
    }

    private void AllRefreshSelectCard()
    {
        foreach (var item in handCards) item.RefreshIsSelect(false);
    }


    private List<TrumpCard> BubbleSortCard(List<TrumpCard> trumpCards)
    {
        var results = trumpCards;

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
        //Debug.Log("PlayCards PlayerId:" + PlayerId);
        handCards.RemoveAll(c => c.IsSelect);
        //foreach (var item in handCards) Debug.Log("PlayCards item:" + item.CardName);
        callback.Invoke(handCards.Count);
    }
}
