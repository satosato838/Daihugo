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
    [SerializeField] private CemeteryController _cemeteryController;
    private Daihugo _daihugoInstance;

    IDisposable thisDisposable;
    void Start()
    {
        _daihugoInstance = new Daihugo();
        thisDisposable = _daihugoInstance.Subscribe(this);

        _daihugoInstance.SetStart();
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
        _daihugoInstance.EndSetPlayer(playerId, lastPlayCards);
    }

    private void RefreshPlayersState()
    {
        for (var i = 0; i < _daihugoInstance.GamePlayers.Count; i++)
        {
            _playerObjects[i].RefreshGamePlayerState(_daihugoInstance.GamePlayers[i].IsMyTurn, _daihugoInstance.LastFieldCardPair);
        }
    }

    public void OnStartSet()
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
        RefreshPlayersState();
        _bg.sprite = _daihugoInstance.GetCurrentState == DaihugoGameRule.DaihugoState.None ? _nomalImage : _kakumeiImage;
    }

    public void OnStartRound()
    {
        _fieldController.Init();
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

    public void OnEndRound()
    {
        _cemeteryController.RefreshCards(_daihugoInstance.CemeteryCards);
        //todo cemeteryAnimation
        Invoke(nameof(CemeteryAnimationEnd), 2.0f);
    }

    private void CemeteryAnimationEnd()
    {
        _daihugoInstance.StartRound();
    }

    public void OnEndSet()
    {
        Debug.Log("OnEndSet");
    }



    private void OnDestroy()
    {
        thisDisposable.Dispose();
    }
}
