
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


public class Daihugo : IDaihugoObservable
{
    private bool IsDebug;
    private int GamePlayingMemberCount => GamePlayers.Count(p => p.IsPlay);
    private int StageStartGamePlayingMemberCount;
    private List<TrumpCard> DeckCards;
    private List<GamePlayer> gamePlayers;
    public List<GamePlayer> GamePlayers => gamePlayers;
    private List<List<TrumpCard>> fieldCards;
    public List<TrumpCard> LastFieldCardPair => fieldCards.Count == 0 ? new List<TrumpCard>() : fieldCards.Last();
    private List<TrumpCard> cemeteryCards;
    public List<TrumpCard> CemeteryCards => cemeteryCards;
    private List<DaihugoRoundResult> daihugoRoundResults;
    private DaihugoRoundResult GetCurrentRoundResult => daihugoRoundResults.Last();
    private DaihugoGameRule.DaihugoState defaultState = DaihugoGameRule.DaihugoState.None;
    private DaihugoGameRule.DaihugoState beforeState;
    private DaihugoGameRule.DaihugoState currentState;
    public DaihugoGameRule.DaihugoState GetCurrentState => currentState;

    private List<DaihugoGameRule.Effect> currentRoundCardEffects;
    public List<DaihugoGameRule.Effect> GetCurrentRoundCardEffects => currentRoundCardEffects;

    private int currentPlayerIndex = 0;
    public int CurrentPlayerIndex => currentPlayerIndex;
    public int CurrentPlayerId => GamePlayers[currentPlayerIndex].PlayerId;
    private int PassCount = 0;
    private int lastPlayCardPlayerId = 0;
    #region GetPlayerData
    private int GetRandomPlayerIndex()
    {
        System.Random rnd = new System.Random();
        return rnd.Next(1, GamePlayingMemberCount);
    }

    private int GetNextPlayerId(string name)
    {
        //Debug.Log(name + ":GetNextPlayerId:" + GamePlayers.All(p => p.IsPlay));
        if (GamePlayers.All(p => p.IsPlay))
        {
            return currentPlayerIndex + 1 >= GamePlayers.Count ? 0 : currentPlayerIndex + 1;
        }
        else
        {
            for (int i = 1; i <= GamePlayers.Count; i++)
            {
                int nextIndex = (currentPlayerIndex + i) % GamePlayers.Count;
                // Debug.Log($"GetNextPlayerId ({currentPlayerIndex} + {i}) % {GamePlayers.Count}:" + (currentPlayerIndex + i) % GamePlayers.Count);
                // Debug.Log($"GetNextPlayerId GamePlayers[{nextIndex}].IsPlay:" + GamePlayers[nextIndex].IsPlay);
                if (GamePlayers[nextIndex].IsPlay)
                {
                    //Debug.Log("GetNextPlayerId nextIndex:" + nextIndex);
                    return nextIndex;
                }
            }

            // 理論上ここに来ることはない（上の条件で else に入ってるから）
            throw new InvalidOperationException("未プレイのプレイヤーが見つかりませんでした。");
        }
    }
    private int GetPlayerIndex(int playerId) => GamePlayers.FindIndex(x => x.PlayerId == playerId);
    #endregion


    #region CreateDeck
    private List<TrumpCard> CreateDeck(bool isDebug = false)
    {
        var result = new List<TrumpCard>();
        var numbers = isDebug ? new DaihugoGameRule.Number[] { DaihugoGameRule.Number.Three, DaihugoGameRule.Number.Seven, DaihugoGameRule.Number.Five } :
                                    DaihugoGameRule.Numbers;

        foreach (var type in DaihugoGameRule.SuitTypes)
        {
            for (var i = 0; i < numbers.Length; i++)
            {
                if (numbers[i] == DaihugoGameRule.Number.Joker) break;
                result.Add(new TrumpCard(type, new CardNumber(numbers[i])));
            }
        }
        if (!isDebug)
        {
            result.Add(new TrumpCard(DaihugoGameRule.SuitType.Joker, new CardNumber(DaihugoGameRule.Number.Joker)));
            result = result.OrderBy(a => Guid.NewGuid()).ToList();
        }

        return result;
    }
    private List<TrumpCard> DealTheCards(bool isDebug = false)
    {
        var result = new List<TrumpCard>();
        var handCardCount = isDebug ? 3 : DaihugoGameRule.Numbers.Length;
        for (var j = 0; j < handCardCount - 1; j++)
        {
            try
            {
                var card = DeckCards.First();
                result.Add(card);
                DeckCards.Remove(card);
            }
            catch
            {
                break;
            }
        }
        return result;
    }

    private void DealLastCard(int index)
    {
        if (DeckCards.Count > 1)
        {
            var hand = gamePlayers[index].HandCards;
            var card = DeckCards.Last();
            hand.Add(card);
            DeckCards.Remove(card);
            gamePlayers[index].DealCard(hand);
        }
    }
    #endregion

    #region PlayCard
    //プレイヤーが手札のカードを出した時の処理
    public void PlayHands(List<TrumpCard> playCards)
    {
        //空のリストがきたらパス判定
        if (playCards.Count() == 0)
        {
            PassCount++;
        }
        else
        {
            if (playCards.Count() == DaihugoGameRule.KakumeiPlayCardCount)
            {
                Kakumei();
                SendDaihugoStateEffect();
            }

            fieldCards.Add(playCards);
            PassCount = 0;
            lastPlayCardPlayerId = CurrentPlayerIndex;
        }

        ActivateCardEffect(playCards);
        //今のステージ開始した人数時の参加中数-1のプレイヤーがパスしたら次のステージに遷移
        //Debug.Log($"PassCount {PassCount} == StageStartMemberCount - 1 {StageStartGamePlayingMemberCount - 1}");
        if (PassCount == StageStartGamePlayingMemberCount - 1)
        {
            StageEnd();
        }
        else
        {
            ChangeNextPlayerTurn(GetNextPlayerId("PlayHands"));
        }
        SendPlayerChange();
    }

    private void ActivateCardEffect(List<TrumpCard> playCards)
    {
        if (playCards.Any(card => card.Effect == DaihugoGameRule.Effect.Eight_Enders))
        {
            //8切りなので強制的に全員パスしたことにする
            PassCount = StageStartGamePlayingMemberCount - 1;
            currentRoundCardEffects.Add(DaihugoGameRule.Effect.Eight_Enders);
            SendCardEffect();
        }
        else if (playCards.Any(card => card.Effect == DaihugoGameRule.Effect.Eleven_Back))
        {
            beforeState = currentState;
            currentRoundCardEffects.Add(DaihugoGameRule.Effect.Eleven_Back);
            Kakumei();
            SendCardEffect();
        }
        else if (LastFieldCardPair.Count == 1 &&
                 LastFieldCardPair.First().Number == DaihugoGameRule.Number.Joker &&
                 playCards.First().Effect == DaihugoGameRule.Effect.Counter_Spade_3)
        {
            currentRoundCardEffects.Add(DaihugoGameRule.Effect.Counter_Spade_3);
            SendCardEffect();
        }
    }

    private void Kakumei()
    {
        currentState = GetCurrentState == DaihugoGameRule.DaihugoState.None ? DaihugoGameRule.DaihugoState.Revolution : DaihugoGameRule.DaihugoState.None;
        SendKakumei();
    }
    //反則上がりしてないかのチェック処理
    //Joker(1枚出し時)
    //2（通常時）
    //3（革命時）
    //8
    //だった場合反則上がりとして大貧民からランクつけする
    private bool IsForbiddenWin(List<TrumpCard> lastPlayCards)
    {
        if (lastPlayCards.Any(v => v.Number == DaihugoGameRule.Number.Eight))
        {
            return true;
        }
        else if (GetCurrentState == DaihugoGameRule.DaihugoState.None && lastPlayCards.Any(v => v.Number == DaihugoGameRule.Number.Two))
        {
            return true;
        }
        else if (GetCurrentState == DaihugoGameRule.DaihugoState.Revolution && lastPlayCards.Any(v => v.Number == DaihugoGameRule.Number.Three))
        {
            return true;
        }
        else if (lastPlayCards.Count == 1 && lastPlayCards[0].Number == DaihugoGameRule.Number.Joker)
        {
            return true;
        }

        return false;
    }
    private void EndCardEffect()
    {
        if (currentRoundCardEffects.Any(v => v == DaihugoGameRule.Effect.Eleven_Back))
        {
            currentState = beforeState;
            SendKakumei();
        }
    }
    #endregion

    public void RefreshCemeteryCards(List<TrumpCard> cards)
    {
        cemeteryCards = cards;
    }

    private List<IDaihugoObserver> observers = new List<IDaihugoObserver>();
    public IDisposable Subscribe(IDaihugoObserver observer)
    {
        if (!observers.Contains(observer))
            observers.Add(observer);
        return new Unsubscriber(observers, observer);
    }
    public Daihugo(bool isDebug = false)
    {
        IsDebug = isDebug;
        daihugoRoundResults = new List<DaihugoRoundResult>();
    }

    public void RoundStart(int playerCount)
    {
        fieldCards = new List<List<TrumpCard>>();
        cemeteryCards = new List<TrumpCard>();

        DeckCards = CreateDeck(isDebug: IsDebug);
        gamePlayers = new List<GamePlayer>();
        for (var i = 0; i < playerCount; i++)
        {
            gamePlayers.Add(new GamePlayer(i, DealTheCards(isDebug: IsDebug), DaihugoGameRule.GameRank.Heimin, defaultState));
        }

        //ランダムなプレイヤーに余ったカードを配る
        DealLastCard(GetRandomPlayerIndex());
        var resultData = new DaihugoRoundResult();
        daihugoRoundResults.Add(resultData);

        lastPlayCardPlayerId = GamePlayers.First().PlayerId;
        currentState = GamePlayers.First().GameState;
        SendStartRound();

        StageStart();
    }

    public void StageStart()
    {
        currentRoundCardEffects = new List<DaihugoGameRule.Effect>
        {
            DaihugoGameRule.Effect.None
        };
        PassCount = 0;
        //ステージ開始時のゲーム参加中の人数を覚えておく
        StageStartGamePlayingMemberCount = GamePlayingMemberCount;
        ChangeNextPlayerTurn(lastPlayCardPlayerId);
        SendStartStage();
    }

    private void ChangeNextPlayerTurn(int nextPlayerIndex)
    {
        currentPlayerIndex = nextPlayerIndex;
        foreach (var p in gamePlayers)
        {
            p.RefreshIsMyturn(false);
        }
        if (gamePlayers[currentPlayerIndex].IsPlay)
        {
            gamePlayers[currentPlayerIndex].RefreshIsMyturn(true);
        }
        else
        {
            var playerId = gamePlayers.First(p => p.IsPlay).PlayerId;
            currentPlayerIndex = GetPlayerIndex(playerId);
        }
        //Debug.Log($"ChangeNextPlayerTurn nextPlayerIndex {nextPlayerIndex} currentPlayerIndex{currentPlayerIndex}");
    }

    ///プレイヤーが上がった場合の処理
    public void EndRoundPlayer(int playerId, List<TrumpCard> lastPlayCards)
    {
        var endPlayerIndex = GetPlayerIndex(playerId);
        //通常通りに終わっていれば大富豪から順にランクつける
        //反則上がりの場合は大貧民からランクつける
        gamePlayers[endPlayerIndex].RefreshIsPlay(false);
        gamePlayers[endPlayerIndex].RefreshIsMyturn(false);

        var endPlayer = new GamePlayer(playerId);
        GetCurrentRoundResult.AddRoundEndPlayer(endPlayer, IsForbiddenWin(lastPlayCards));

        gamePlayers[endPlayerIndex].RefreshRank(GetCurrentRoundResult.GetPlayerIdRank(endPlayerIndex));
        SendToGoOut(endPlayerIndex);
        if (GetCurrentRoundResult.ResultPlayersCount == GamePlayingMemberCount - 1)
        {
            RoundEnd();
        }
    }

    private void StageEnd()
    {
        EndCardEffect();
        RefreshCemeteryCards(fieldCards.SelectMany(v => v).ToList());
        fieldCards = new List<List<TrumpCard>>();
        if (GamePlayers[GetPlayerIndex(lastPlayCardPlayerId)].IsPlay)
        {
            lastPlayCardPlayerId = GetNextPlayerId("StageEnd");
        }
        else
        {
            lastPlayCardPlayerId = GamePlayers.First(p => p.IsPlay).PlayerId;
        }
        //Debug.Log("Stage End lastPlayCardPlayerId:" + lastPlayCardPlayerId);
        ChangeNextPlayerTurn(lastPlayCardPlayerId);
        SendEndStage();
    }

    private void RoundEnd()
    {
        SendEndRound();
    }

    public void SendStartRound()
    {
        foreach (var observer in observers)
        {
            observer.OnStartRound();
        }
    }

    public void SendStartStage()
    {
        foreach (var observer in observers)
        {
            observer.OnStartStage();
        }
    }
    public void SendPlayerChange()
    {
        foreach (var observer in observers)
        {
            observer.OnChangePlayerTurn(GamePlayers[currentPlayerIndex]);
        }
    }
    public void SendKakumei()
    {
        foreach (var observer in observers)
        {
            observer.OnKakumei(GetCurrentState);
        }
    }

    public void SendCardEffect()
    {

        foreach (var observer in observers)
        {
            observer.OnCardEffect(GetCurrentRoundCardEffects.Last());
        }
    }
    public void SendDaihugoStateEffect()
    {
        foreach (var observer in observers)
        {
            observer.OnDaihugoStateEffect(currentState);
        }
    }

    public void SendToGoOut(int playerIndex)
    {
        foreach (var observer in observers)
        {
            observer.OnToGoOut(playerIndex);
        }
    }

    public void SendEndStage()
    {
        foreach (var observer in observers)
        {
            observer.OnEndStage();
        }
    }

    public void SendEndRound()
    {
        foreach (var observer in observers)
        {
            observer.OnEndRound();
        }
    }
}