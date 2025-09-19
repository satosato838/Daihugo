
using System;
using System.Linq;
using System.Collections.Generic;


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
    private List<DaihugoSetResult> daihugoSetResults;
    private DaihugoSetResult GetCurrentSetResult => daihugoSetResults.Last();
    private DaihugoGameRule.DaihugoState currentState;
    public DaihugoGameRule.DaihugoState GetCurrentState => currentState;

    private int currentPlayerIndex = 0;
    public int CurrentPlayerIndex => currentPlayerIndex;
    public int CurrentPlayerId => GamePlayers[currentPlayerIndex].PlayerId;
    private int PassCount = 0;
    private int lastPlayCardPlayerId = 0;
    public int LastPlayCardPlayerId => lastPlayCardPlayerId;
    public Daihugo()
    {
        daihugoSetResults = new List<DaihugoSetResult>();
    }
    private int GetRandomPlayerIndex()
    {
        System.Random rnd = new System.Random();
        return rnd.Next(1, GamePlayMemberCount);
    }

    private int GetNextPlayerId()
    {
        if (GamePlayers.All(p => p.IsPlay))
        {
            return currentPlayerIndex + 1 >= GamePlayers.Count ? 0 : currentPlayerIndex + 1;
        }
        else
        {
            // まだプレイしていない人がいる → 次の未プレイのプレイヤーを探す
            for (int i = 1; i <= GamePlayers.Count; i++)
            {
                int nextIndex = (currentPlayerIndex + i) % GamePlayers.Count;
                if (!GamePlayers[nextIndex].IsPlay)
                {
                    return nextIndex;
                }
            }

            // 理論上ここに来ることはない（上の条件で else に入ってるから）
            throw new InvalidOperationException("未プレイのプレイヤーが見つかりませんでした。");
        }
    }

    private int GetPlayerIndex(int playerId) => GamePlayers.FindIndex(x => x.PlayerId == playerId);

    private List<IDaihugoObserver> observers = new List<IDaihugoObserver>();
    public IDisposable Subscribe(IDaihugoObserver observer)
    {
        if (!observers.Contains(observer))
            observers.Add(observer);
        return new Unsubscriber(observers, observer);
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
            gamePlayers.Add(new GamePlayer(i, DealTheCards(), DaihugoGameRule.GameRank.Heimin, DaihugoGameRule.DaihugoState.Revolution));
        }

        //ランダムなプレイヤーに余ったカードを配る
        DealLastCard(GetRandomPlayerIndex());
        lastPlayCardPlayerId = GamePlayers.First().PlayerId;
        currentState = GamePlayers.First().GameState;
        PassCount = 0;
        ChangeNextPlayerTurn(lastPlayCardPlayerId);
        SendStartSet();

        var resultData = new DaihugoSetResult();
        daihugoSetResults.Add(resultData);
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
        var hand = gamePlayers[index].HandCards;
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
            if (playCards.Count() == 4)
            {
                Kakumei();
            }

            fieldCards.Add(playCards);
            PassCount = 0;
            lastPlayCardPlayerId = CurrentPlayerIndex;
        }

        if (PassCount == GamePlayMemberCount - 1)
        {
            EndRound();
        }
        else
        {
            ChangeNextPlayerTurn(GetNextPlayerId());
        }
        SendPlayerChange();
    }
    private void Kakumei()
    {
        currentState = GetCurrentState == DaihugoGameRule.DaihugoState.None ? DaihugoGameRule.DaihugoState.Revolution : DaihugoGameRule.DaihugoState.None;
        SendKakumei();
    }

    /// playerのそのセット終了検知処理
    public void EndSetPlayer(int playerId, List<TrumpCard> lastPlayCards)
    {
        //通常通りに終わっていれば大富豪から順にランクつける
        //反則上がりの場合は大貧民からランクつける
        gamePlayers[GetPlayerIndex(playerId)].RefreshIsPlay(false);
        gamePlayers[GetPlayerIndex(playerId)].RefreshIsMyturn(false);
        var endPlayer = new GamePlayer(playerId);
        GetCurrentSetResult.AddSetEndPlayer(endPlayer, IsForbiddenWin(lastPlayCards));
        if (GetCurrentSetResult.ResultPlayersCount == GamePlayMemberCount - 1)
        {
            EndSet();
        }
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
            observer.OnChangePlayerTurn(GamePlayers[currentPlayerIndex]);
        }
    }
    public void SendKakumei()
    {
        foreach (var observer in observers)
        {
            observer.OnKakumei(currentState);
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