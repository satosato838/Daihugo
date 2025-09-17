using System;
using System.Collections.Generic;
using UnityEngine;

public class DaihugoController : MonoBehaviour, IDaihugoObserver
{
    [SerializeField] private PlayerObject[] playerObjects;
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

    private void RefreshPlayersState()
    {
        for (var i = 0; i < _daihugoInstance.GamePlayers.Count; i++)
        {
            playerObjects[i].RefreshGamePlayerState(_daihugoInstance.GamePlayers[i].IsMyTurn, _daihugoInstance.LastFieldCardPair);
        }
    }

    public void OnStartSet()
    {
        for (var i = 0; i < _daihugoInstance.GamePlayers.Count; i++)
        {
            playerObjects[i].Init(_daihugoInstance.GamePlayers[i], v =>
            {
                PlayHands(v);
            });
        }
        _fieldController.Init();
        _cemeteryController.Init();
    }

    public void OnStartRound()
    {
        _fieldController.Init();
    }
    public void OnChangePlayerTurn(GamePlayer gamePlayer)
    {
        Debug.Log("OnChangePlayerTurn gamePlayer:" + gamePlayer.PlayerId);
        RefreshPlayersState();
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
        //todo create set result data
        throw new NotImplementedException();
    }



    private void OnDestroy()
    {
        thisDisposable.Dispose();
    }
}
