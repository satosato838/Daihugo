using System;
using System.Collections.Generic;
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

        _daihugoInstance.RoundStart();
    }

    private void PlayHands(List<TrumpCard> trumpCards)
    {
        _daihugoInstance.PlayFieldCards(trumpCards);
        // foreach (var item in Daihugo.FieldCardPairs)
        // {
        //     Debug.Log("Number:" + item.Number + ": Suit" + item.Suit);
        // }
        _fieldController.RefreshCards(_daihugoInstance.LastFieldCardPair);
    }
    private void EndSet(int playerId, List<TrumpCard> lastPlayCards)
    {
        _daihugoInstance.EndRoundPlayer(playerId, lastPlayCards);
    }

    private void RefreshPlayersState()
    {
        for (var i = 0; i < _daihugoInstance.GamePlayers.Count; i++)
        {
            _playerObjects[i].RefreshGamePlayerState(_daihugoInstance.GamePlayers[i].IsMyTurn, _daihugoInstance.LastFieldCardPair);
        }
    }
    private void RefreshPlayerRank(int goOutPlayerIndex)
    {
        var currentPlayer = _playerObjects[goOutPlayerIndex];
        currentPlayer.SetPlayerRank(_daihugoInstance.GamePlayers[goOutPlayerIndex].PlayerRank);
    }

    public void OnStartRound()
    {
        for (var i = 0; i < _daihugoInstance.GamePlayers.Count; i++)
        {
            _playerObjects[i].Init(_daihugoInstance.GamePlayers[i], v =>
            {
                PlayHands(v);
            },
            (id, v) =>
            {
                EndSet(id, v);
            }
            );
        }
        _fieldController.Init();
        _cemeteryController.Init();
        Debug.Log("OnStartRound :" + _daihugoInstance.CurrentPlayerId);
        RefreshPlayersState();
        _bg.sprite = _daihugoInstance.GetCurrentState == DaihugoGameRule.DaihugoState.None ? _nomalImage : _kakumeiImage;
        _effectCutInController.Play("1Round", 0.5f);
    }

    private void CemeteryAnimationEnd()
    {
        _daihugoInstance.StageStart();
    }
    public void OnStartStage()
    {
        _fieldController.Init();
        Debug.Log("OnStartStage :" + _daihugoInstance.CurrentPlayerId);
        RefreshPlayersState();
    }
    public void OnChangePlayerTurn(GamePlayer gamePlayer)
    {
        Debug.Log("OnChangePlayerTurn gamePlayer:" + gamePlayer.PlayerId);
        RefreshPlayersState();
    }
    public void OnKakumei(DaihugoGameRule.DaihugoState state)
    {
        Debug.Log("OnKakumei:" + state);
        foreach (var item in _playerObjects)
        {
            item.Kakumei(state);
        }
        _bg.sprite = state == DaihugoGameRule.DaihugoState.None ? _nomalImage : _kakumeiImage;
    }

    public void OnEndStage()
    {
        _cemeteryController.RefreshCards(_daihugoInstance.CemeteryCards);
        //todo cemeteryAnimation
        Invoke(nameof(CemeteryAnimationEnd), 2.0f);
    }
    public void OnCardEffect(DaihugoGameRule.Effect effect)
    {
        Debug.Log("OnCardEffect:" + effect);
        _effectCutInController.Play(effect.ToString());
    }
    public void OnDaihugoStateEffect(DaihugoGameRule.DaihugoState state)
    {
        Debug.Log("OnDaihugoStateEffect:" + state);
        _effectCutInController.Play(state == DaihugoGameRule.DaihugoState.None ? "Counter Revolution" : DaihugoGameRule.DaihugoState.Revolution.ToString());
    }

    public void OnToGoOut(int goOutPlayerIndex)
    {
        Debug.Log("OnToGoOut:" + goOutPlayerIndex);
        RefreshPlayerRank(goOutPlayerIndex);
    }

    public void OnEndRound()
    {
        Debug.Log("OnEndSet");
    }
    private void OnDestroy()
    {
        thisDisposable.Dispose();
    }


}
