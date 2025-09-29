
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


public class Daihugo : IDaihugoObservable
{
    private int _gameRoundCount;
    private bool IsDebug;
    private bool IsDebugCard;
    public int EntryPlayerCount => GamePlayers.Count();
    private int GamePlayingMemberCount => GamePlayers.Count(p => p.IsPlay);
    private int StageStartGamePlayingMemberCount;
    private List<TrumpCard> DeckCards;
    private List<GamePlayer> gamePlayers;
    public List<GamePlayer> GamePlayers => gamePlayers;
    private List<List<TrumpCard>> fieldCards;
    public List<TrumpCard> LastFieldCardPair => fieldCards.Count == 0 ? new List<TrumpCard>() : fieldCards.Last();
    private List<TrumpCard> cemeteryCards;
    public List<TrumpCard> CemeteryCards => cemeteryCards;

    private List<(int playerId, List<TrumpCard> cards)> cardChangeList;
    private List<DaihugoRoundResult> daihugoRoundResults;
    private DaihugoRoundResult GetCurrentRoundResult => daihugoRoundResults.Last();
    public List<DaihugoRoundResult> GetRoundResults => daihugoRoundResults;
    private (DaihugoGameRule.DaihugoState daihugoState, DaihugoGameRule.GameRank rank, DaihugoGameRule.GameState gameState) defaultValue = (DaihugoGameRule.DaihugoState.None, DaihugoGameRule.GameRank.Heimin, DaihugoGameRule.GameState.None);
    private DaihugoGameRule.DaihugoState beforeState;
    private DaihugoGameRule.DaihugoState currentState;
    public DaihugoGameRule.DaihugoState GetDaihugoCurrentState => currentState;
    private DaihugoGameRule.GameState currentGameState;
    public DaihugoGameRule.GameState GetGameCurrentState => currentGameState;

    private List<DaihugoGameRule.Effect> currentRoundCardEffects;
    public List<DaihugoGameRule.Effect> GetCurrentRoundCardEffects => currentRoundCardEffects;

    private int currentPlayerIndex = 0;
    public int CurrentPlayerId => GamePlayers[currentPlayerIndex].PlayerId;
    public int CurrentRoundIndex => daihugoRoundResults.Count;
    private int PassCount = 0;
    private int lastPlayCardPlayerId = 0;
    #region GetPlayerData

    private void CreatePlayerData(int playerCount)
    {
        gamePlayers = new List<GamePlayer>();
        var resultData = new DaihugoRoundResult();
        if (IsDebug)
        {
            // var resultData1 = new DaihugoRoundResult();
            // resultData1.CreateDebugData();
            // daihugoRoundResults.Add(resultData1);
            // //Debug.Log("1roundCount:" + CurrentRoundIndex);
            // var resultData2 = new DaihugoRoundResult();
            // resultData2.CreateDebugData();
            // daihugoRoundResults.Add(resultData2);
            // //Debug.Log("2roundCount:" + CurrentRoundIndex);
            // var resultData3 = new DaihugoRoundResult();
            // resultData3.CreateDebugData();
            // daihugoRoundResults.Add(resultData3);
            // //Debug.Log("3roundCount:" + CurrentRoundIndex);
            // var resultData4 = new DaihugoRoundResult();
            // resultData4.CreateDebugData();
            // daihugoRoundResults.Add(resultData4);
            // //Debug.Log("4roundCount:" + CurrentRoundIndex);
        }

        if (daihugoRoundResults.Count == 0)
        {
            for (var i = 0; i < playerCount; i++)
            {
                gamePlayers.Add(new GamePlayer(i, DealTheCards(isDebug: IsDebugCard), defaultValue.rank, defaultValue.daihugoState, defaultValue.gameState));
            }
        }
        else
        {
            gamePlayers = daihugoRoundResults.Last().GetNextGamePlayers();
            foreach (var player in gamePlayers)
            {
                player.DealCard(DealTheCards(isDebug: IsDebugCard));
            }
        }
        daihugoRoundResults.Add(resultData);
        // foreach (var item in daihugoRoundResults)
        // {
        //     Debug.Log("daihugoRoundResults itemCount:" + item.ResultPlayersCount);
        // }
    }

    private int GetNextPlayerId()
    {
        //Debug.Log("GetNextPlayerId GamePlayers.All(p => p.IsPlay):" + GamePlayers.All(p => p.IsPlay));
        if (GamePlayers.All(p => p.IsPlay))
        {
            return currentPlayerIndex + 1 >= GamePlayers.Count ? 0 : currentPlayerIndex + 1;
        }
        else
        {
            for (int i = 1; i <= GamePlayers.Count; i++)
            {
                int nextIndex = (currentPlayerIndex + i) % GamePlayers.Count;
                // Debug.Log($"GetNextPlayerId ({CurrentPlayerIndex} + {i}) % {GamePlayers.Count}:" + (CurrentPlayerIndex + i) % GamePlayers.Count);
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
            lastPlayCardPlayerId = GamePlayers[currentPlayerIndex].PlayerId;
        }

        ActivateCardEffect(playCards);

        Debug.Log($"StageStartGamePlayingMemberCount {StageStartGamePlayingMemberCount} PassCount {PassCount} == StageStartMemberCount - 1 {StageStartGamePlayingMemberCount - 1}");
        if (StageStartGamePlayingMemberCount == 2)
        {
            if (PassCount >= 1)
            {
                StageEnd();
            }
            else
            {
                ChangeNextPlayerTurn(GetNextPlayerId());
            }
        }
        else if (PassCount == StageStartGamePlayingMemberCount - 1)
        {
            StageEnd();
        }
        else
        {
            ChangeNextPlayerTurn(GetNextPlayerId());
        }
        SendPlayerChange();
    }
    public void ExecuteCardExchange(int playerId, List<TrumpCard> playCards)
    {
        // Debug.Log($"CardExchange: playerId={playerId}");
        // foreach (var item in playCards)
        //     Debug.Log($"CardExchange cardName={item.CardName}");

        cardChangeList.Add((playerId, playCards));

        // 全員分の提出が揃ったら交換処理
        if (cardChangeList.Count < EntryPlayerCount) return;

        // ランクごとの交換先マップ
        var exchangePairs = new Dictionary<DaihugoGameRule.GameRank, DaihugoGameRule.GameRank>
    {
        { DaihugoGameRule.GameRank.DaiHinmin, DaihugoGameRule.GameRank.DaiHugo },
        { DaihugoGameRule.GameRank.Hinmin,    DaihugoGameRule.GameRank.Hugo },
        { DaihugoGameRule.GameRank.Hugo,      DaihugoGameRule.GameRank.Hinmin },
        { DaihugoGameRule.GameRank.DaiHugo,   DaihugoGameRule.GameRank.DaiHinmin }
    };

        // ランク→プレイヤーの辞書を作成
        var playerByRank = gamePlayers.ToDictionary(p => p.PlayerRank, p => p);

        foreach (var (pid, cards) in cardChangeList)
        {
            var fromPlayer = gamePlayers[GetPlayerIndex(pid)];
            if (exchangePairs.TryGetValue(fromPlayer.PlayerRank, out var targetRank) &&
                playerByRank.TryGetValue(targetRank, out var toPlayer))
            {
                toPlayer.AddCards(cards);
            }
            else
            {
                throw new Exception($"Exchange target not found for {fromPlayer.PlayerRank}");
            }
        }

        currentGameState = DaihugoGameRule.GameState.GamePlay;
        SendGameState();
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
        currentState = GetDaihugoCurrentState == DaihugoGameRule.DaihugoState.None ? DaihugoGameRule.DaihugoState.Revolution : DaihugoGameRule.DaihugoState.None;
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
        else if (GetDaihugoCurrentState == DaihugoGameRule.DaihugoState.None && lastPlayCards.Any(v => v.Number == DaihugoGameRule.Number.Two))
        {
            return true;
        }
        else if (GetDaihugoCurrentState == DaihugoGameRule.DaihugoState.Revolution && lastPlayCards.Any(v => v.Number == DaihugoGameRule.Number.Three))
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
    public Daihugo(int roundCount, bool isDebug = false, bool isDebugCard = false)
    {
        _gameRoundCount = roundCount;
        IsDebug = isDebug;
        IsDebugCard = isDebugCard;
        daihugoRoundResults = new List<DaihugoRoundResult>();
    }

    public void RoundStart(int playerCount)
    {
        fieldCards = new List<List<TrumpCard>>();
        cemeteryCards = new List<TrumpCard>();
        cardChangeList = new List<(int playerId, List<TrumpCard>)>();

        DeckCards = CreateDeck(isDebug: IsDebugCard);
        CreatePlayerData(playerCount);

        //ランダムなプレイヤーに余ったカードを配る
        System.Random rnd = new System.Random();
        DealLastCard(rnd.Next(1, EntryPlayerCount));

        lastPlayCardPlayerId = GamePlayers.First().PlayerId;
        currentState = DaihugoGameRule.DaihugoState.None;
        currentGameState = CurrentRoundIndex == 1 ? DaihugoGameRule.GameState.GamePlay : DaihugoGameRule.GameState.CardChange;
        //Debug.Log($"RoundStart CurrentRoundIndex:{CurrentRoundIndex} currentGameState:{currentGameState}");
        SendStartRound();
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
        ChangeNextPlayerTurn(GetPlayerIndex(lastPlayCardPlayerId));
        SendStartStage();
    }
    public void CardChangeStart()
    {
        currentGameState = DaihugoGameRule.GameState.CardChange;
        SendGameState();
    }

    private void ChangeNextPlayerTurn(int nextPlayerIndex)
    {
        //Debug.Log($"before ChangeNextPlayerTurn nextPlayerIndex {nextPlayerIndex} CurrentPlayerIndex {currentPlayerIndex}");
        currentPlayerIndex = nextPlayerIndex;
        foreach (var p in gamePlayers)
        {
            p.RefreshIsMyturn(false);
        }
        //もしこのラウンド開始するはずだったプレイヤーが上がってたら
        //リストの先頭にいたまだ試合中の人を親にする
        if (!gamePlayers[currentPlayerIndex].IsPlay)
        {
            //Debug.Log($"!gamePlayers[currentPlayerIndex].IsPlay ChangeNextPlayerTurn nextPlayerIndex {nextPlayerIndex} CurrentPlayerIndex {currentPlayerIndex}");
            currentPlayerIndex = GetPlayerIndex(gamePlayers.First(p => p.IsPlay).PlayerId);
        }
        else
        {
            //Debug.Log($"gamePlayers[currentPlayerIndex].IsPlay ChangeNextPlayerTurn nextPlayerIndex {nextPlayerIndex} CurrentPlayerIndex {currentPlayerIndex}");
        }
        gamePlayers[currentPlayerIndex].RefreshIsMyturn(true);
        //Debug.Log($"after ChangeNextPlayerTurn nextPlayerIndex {nextPlayerIndex} CurrentPlayerIndex {currentPlayerIndex}");
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
        //Debug.Log($"EndRoundPlayer ResultPlayersCount:{GetCurrentRoundResult.ResultPlayersCount} == EntryPlayerCount - 1:{EntryPlayerCount - 1}");
        if (GetCurrentRoundResult.ResultPlayersCount == EntryPlayerCount - 1)
        {
            //最後まで残ったプレイヤーは最下位プレイヤーとして登録
            GetCurrentRoundResult.AddBoobyPlayer(gamePlayers.First(p => p.IsPlay));
            RoundEnd();
        }
    }

    private void StageEnd()
    {
        EndCardEffect();
        RefreshCemeteryCards(fieldCards.SelectMany(v => v).ToList());
        fieldCards = new List<List<TrumpCard>>();
        if (!GamePlayers[GetPlayerIndex(lastPlayCardPlayerId)].IsPlay)
        {
            lastPlayCardPlayerId = GamePlayers.First(p => p.IsPlay).PlayerId;
        }
        //Debug.Log("Stage End lastPlayCardPlayerId:" + lastPlayCardPlayerId);
        ChangeNextPlayerTurn(lastPlayCardPlayerId);
        SendEndStage();
    }

    private void RoundEnd()
    {
        currentGameState = CurrentRoundIndex == _gameRoundCount ? DaihugoGameRule.GameState.Result : DaihugoGameRule.GameState.None;
        //Debug.Log("RoundEnd currentGameState:" + currentGameState);
        SendEndRound();
    }

    public void SendStartRound()
    {
        foreach (var observer in observers)
        {
            //Debug.Log("SendStartRound OnStartRound:" + GetGameCurrentState);
            observer.OnStartRound(GetGameCurrentState);
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
            observer.OnKakumei(GetDaihugoCurrentState);
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
    public void SendGameState()
    {

        foreach (var observer in observers)
        {
            observer.OnGameState(GetGameCurrentState);
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
            observer.OnEndRound(GetGameCurrentState);
        }
    }
}