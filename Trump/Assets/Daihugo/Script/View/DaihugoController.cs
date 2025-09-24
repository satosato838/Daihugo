using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DaihugoController : MonoBehaviour, IDaihugoObserver
{
    [SerializeField] private Image _bg;
    [SerializeField] private Sprite _nomalImage;
    [SerializeField] private Sprite _kakumeiImage;
    [SerializeField] private PlayerObject[] _playerObjects;
    [SerializeField] private FieldController _fieldController;
    [SerializeField] private EffectCutInController _effectCutInController;
    [SerializeField] private CemeteryController _cemeteryController;
    [SerializeField] private bool _isDebug;
    private Daihugo _daihugoInstance;

    IDisposable thisDisposable;
    void Start()
    {
        _daihugoInstance = new Daihugo(isDebug: _isDebug);
        thisDisposable = _daihugoInstance.Subscribe(this);

        StartRound();
    }

    private void StartRound()
    {
        _daihugoInstance.RoundStart(playerCount: 4);
    }

    private void PlayHands(List<TrumpCard> trumpCards)
    {
        _daihugoInstance.PlayHands(trumpCards);
        // foreach (var item in Daihugo.FieldCardPairs)
        // {
        //     Debug.Log("Number:" + item.Number + ": Suit" + item.Suit);
        // }
        _fieldController.RefreshCards(_daihugoInstance.LastFieldCardPair);
    }
    private void EndRoundPlayer(int playerId, List<TrumpCard> lastPlayCards)
    {
        _daihugoInstance.EndRoundPlayer(playerId, lastPlayCards);
    }

    private void RefreshPlayersState()
    {
        for (var i = 0; i < _playerObjects.Length; i++)
        {
            _playerObjects[i].RefreshGamePlayerState(_daihugoInstance.GamePlayers.First(p => p.PlayerId == i).IsMyTurn, _daihugoInstance.LastFieldCardPair);
        }
    }
    private void RefreshPlayerRank(int goOutPlayerIndex)
    {
        var currentPlayer = _playerObjects[goOutPlayerIndex];
        currentPlayer.SetPlayerRank(_daihugoInstance.GamePlayers[goOutPlayerIndex].PlayerRank);
    }

    public void OnStartRound()
    {
        Debug.Log("<color=cyan>" + "OnStartRound CurrentPlayerId:" + _daihugoInstance.CurrentPlayerId + "</color>");
        for (var i = 0; i < _daihugoInstance.GamePlayers.Count; i++)
        {
            _playerObjects[i].Init(_daihugoInstance.GamePlayers.First(p => p.PlayerId == i), v =>
            {
                PlayHands(v);
            },
            (id, v) =>
            {
                EndRoundPlayer(id, v);
            }
            );
        }
        _fieldController.Init();
        _cemeteryController.Init();
        RefreshPlayersState();
        _bg.sprite = _daihugoInstance.GetCurrentState == DaihugoGameRule.DaihugoState.None ? _nomalImage : _kakumeiImage;
        _effectCutInController.Play(_daihugoInstance.CurrentRoundIndex + "Round", 0.5f, () =>
        {
            _daihugoInstance.StageStart();
        });
    }

    private void CemeteryAnimationEnd()
    {
        _daihugoInstance.StageStart();
    }
    public void OnStartStage()
    {
        _fieldController.Init();
        Debug.Log("<color=cyan>" + "OnStartStage CurrentPlayerId:" + _daihugoInstance.CurrentPlayerId + "</color>");
        RefreshPlayersState();
    }
    public void OnChangePlayerTurn(GamePlayer gamePlayer)
    {
        Debug.Log("<color=cyan>" + "OnChangePlayerTurn PlayerId:" + gamePlayer.PlayerId + "</color>");
        RefreshPlayersState();
    }
    public void OnKakumei(DaihugoGameRule.DaihugoState state)
    {
        Debug.Log("<color=cyan>" + "OnKakumei DaihugoState:" + state + "</color>");
        foreach (var item in _playerObjects)
        {
            item.Kakumei(state);
        }
        _bg.sprite = state == DaihugoGameRule.DaihugoState.None ? _nomalImage : _kakumeiImage;
    }

    public void OnEndStage()
    {
        Debug.Log("<color=cyan>" + "OnEndStage:" + "</color>");
        _cemeteryController.RefreshCards(_daihugoInstance.CemeteryCards);
        //todo cemeteryAnimation
        Invoke(nameof(CemeteryAnimationEnd), 2.0f);
    }
    public void OnCardEffect(DaihugoGameRule.Effect effect)
    {
        Debug.Log("<color=cyan>" + "OnCardEffect:" + effect + "</color>");
        _effectCutInController.Play(effect.ToString());
    }
    public void OnDaihugoStateEffect(DaihugoGameRule.DaihugoState state)
    {
        Debug.Log("<color=cyan>" + "OnDaihugoStateEffect:" + state + "</color>");
        _effectCutInController.Play(state == DaihugoGameRule.DaihugoState.None ? "Counter Revolution" : DaihugoGameRule.DaihugoState.Revolution.ToString());
    }

    public void OnToGoOut(int goOutPlayerIndex)
    {
        Debug.Log("<color=cyan>" + "OnToGoOut goOutPlayerIndex:" + goOutPlayerIndex + "</color>");
        RefreshPlayerRank(goOutPlayerIndex);
    }

    public void OnEndRound()
    {
        Debug.Log("<color=cyan>" + "OnEndRound" + "</color>");
        _effectCutInController.Play("End Round", 0.5f, () =>
        {
            StartRound();
        });

    }
    private void OnDestroy()
    {
        thisDisposable.Dispose();
    }
}
