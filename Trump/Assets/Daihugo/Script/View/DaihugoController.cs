using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DaihugoController : MonoBehaviour, IDaihugoObserver
{
    [SerializeField] private GameObject _daihugoObject;
    [SerializeField] private Image _bg;
    [SerializeField] private Sprite _nomalImage;
    [SerializeField] private Sprite _kakumeiImage;
    [SerializeField] private PlayerObject[] _playerObjects;
    [SerializeField] private FieldController _fieldController;
    [SerializeField] private EffectCutInController _effectCutInController;
    [SerializeField] private CemeteryController _cemeteryController;
    [SerializeField] private ResultController _resultController;
    [SerializeField] private bool _isDebug;
    [SerializeField] private bool _isDebugCard;
    private Daihugo _daihugoInstance;

    IDisposable thisDisposable;
    void Start()
    {
        _daihugoObject.SetActive(false);
    }

    public void GameStart()
    {
        _daihugoObject.SetActive(true);
        _daihugoInstance = new Daihugo(roundCount: _isDebug ? 1 : 5, isDebug: _isDebug, isDebugCard: _isDebugCard);
        thisDisposable = _daihugoInstance.Subscribe(this);
        StartRound(playerCount: 4);
    }

    private void StartRound(int playerCount)
    {
        _daihugoInstance.RoundStart(playerCount);
    }

    private void PlayHands(int playerId, List<TrumpCard> trumpCards)
    {
        if (_daihugoInstance.GetGameCurrentState == DaihugoGameRule.GameState.GamePlay)
        {
            _daihugoInstance.PlayHands(trumpCards);
            _fieldController.RefreshCards(_daihugoInstance.LastFieldCardPair);
        }
        else
        {
            _daihugoInstance.ExecuteCardExchange(playerId, trumpCards);
            var player = _playerObjects.First(p => p.PlayerId == playerId);
            player.ShowExChangeCards(trumpCards);
        }

    }
    private void EndRoundPlayer(int playerId, List<TrumpCard> lastPlayCards)
    {
        _daihugoInstance.EndRoundPlayer(playerId, lastPlayCards);
    }

    private void RefreshPlayersState(DaihugoGameRule.GameState state)
    {
        for (var i = 0; i < _playerObjects.Length; i++)
        {
            _playerObjects[i].RefreshGamePlayerState(state, _daihugoInstance.GamePlayers.First(p => p.PlayerId == i).IsMyTurn, _daihugoInstance.LastFieldCardPair);
        }
    }
    private void RefreshPlayerCards()
    {
        for (var i = 0; i < _playerObjects.Length; i++)
        {
            _playerObjects[i].RefreshCards();
        }
    }

    private void RefreshPlayerRank(int goOutPlayerIndex)
    {
        var currentPlayer = _playerObjects[goOutPlayerIndex];
        currentPlayer.SetPlayerRank(_daihugoInstance.GamePlayers[goOutPlayerIndex].PlayerRank);
    }

    public void OnStartRound(DaihugoGameRule.GameState state)
    {
        Debug.Log($"<color=red> OnStartRound DaihugoGameRule.GameState {state}, CurrentPlayerId:{_daihugoInstance.CurrentPlayerId} </color>");
        for (var i = 0; i < _daihugoInstance.EntryPlayerCount; i++)
        {
            var playerObject = _playerObjects[i];
            var player = _daihugoInstance.GamePlayers.First(p => p.PlayerId == i);
            playerObject.Init(player,
            (id, v) =>
            {
                PlayHands(id, v);
            },
            (id, v) =>
            {
                EndRoundPlayer(id, v);
            }
            );
            if (state == DaihugoGameRule.GameState.CardChange)
            {
                playerObject.UpdateSelectableCardsForExchange();
            }
        }
        _fieldController.Init();
        _cemeteryController.Init();
        RefreshPlayersState(state);
        _bg.sprite = _daihugoInstance.GetDaihugoCurrentState == DaihugoGameRule.DaihugoState.None ? _nomalImage : _kakumeiImage;
        _effectCutInController.Play(_daihugoInstance.CurrentRoundIndex + "Round", 0.5f, () =>
        {
            if (_daihugoInstance.CurrentRoundIndex > 1)
            {
                _daihugoInstance.CardChangeStart();
            }
            else
            {
                StartCoroutine(StageStart(0.0f));
            }

        });
    }

    private IEnumerator StageStart(float delay)
    {
        Debug.Log($"<color=yellow> StageStart DaihugoGameRule.GameState {_daihugoInstance.GetGameCurrentState}</color>");
        yield return new WaitForSeconds(delay);
        if (_daihugoInstance.GetGameCurrentState == DaihugoGameRule.GameState.GamePlay)
        {
            _daihugoInstance.StageStart();
        }
    }
    public void OnStartStage()
    {
        _fieldController.Init();
        Debug.Log($"<color=yellow> OnStartStage DaihugoGameRule.GameState {_daihugoInstance.GetGameCurrentState}, CurrentPlayerId:{_daihugoInstance.CurrentPlayerId} </color>");
        RefreshPlayersState(_daihugoInstance.GetGameCurrentState);
    }
    public void OnChangePlayerTurn(GamePlayer gamePlayer)
    {
        Debug.Log("<color=cyan>" + "OnChangePlayerTurn PlayerId:" + gamePlayer.PlayerId + "</color>");
        RefreshPlayersState(_daihugoInstance.GetGameCurrentState);
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
        Debug.Log("<color=yellow>" + "OnEndStage GetGameCurrentState:" + _daihugoInstance.GetGameCurrentState + "</color>");
        _fieldController.Init();
        _cemeteryController.Init();
        RefreshPlayersState(_daihugoInstance.GetGameCurrentState);
        _effectCutInController.Play("Change Dealer", 0.5f, () =>
        {
            StartCoroutine(StageStart(0.0f));
        });
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

    public void OnGameState(DaihugoGameRule.GameState state)
    {
        Debug.Log("<color=cyan>" + "OnGameState:" + state + "</color>");
        if (state == DaihugoGameRule.GameState.CardChange)
        {
            _effectCutInController.Play(state.ToString(), 0.5f, () =>
            {

            });
        }
        else if (state == DaihugoGameRule.GameState.GamePlay)
        {
            _effectCutInController.Play("ExChange Card", 0.5f, () =>
            {
                RefreshPlayerCards();
                for (var i = 0; i < _playerObjects.Length; i++)
                {
                    _playerObjects[i].DeleteExChangeCards();
                }
                _effectCutInController.Play("PLAY THE GAME", 0.5f, () =>
                {
                    StartCoroutine(StageStart(0.0f));
                });
            });
        }
        else
        {
            _effectCutInController.Play("GAME SET", 0.5f, () =>
            {
                //todo end game
            });
        }
    }

    public void OnToGoOut(int goOutPlayerIndex)
    {
        Debug.Log("<color=cyan>" + "OnToGoOut goOutPlayerIndex:" + goOutPlayerIndex + "</color>");
        RefreshPlayerRank(goOutPlayerIndex);
    }

    public void OnEndRound(DaihugoGameRule.GameState state)
    {
        Debug.Log($"<color=red>OnEndRound DaihugoGameRule.GameState {state}</color>");
        _effectCutInController.Play("End Round", 0.5f, () =>
        {
            if (_daihugoInstance.GetGameCurrentState == DaihugoGameRule.GameState.Result)
            {
                _daihugoObject.SetActive(false);
                _resultController.ShowResult(_daihugoInstance.GetRoundResults);
                thisDisposable.Dispose();
            }
            else
            {
                StartRound(_daihugoInstance.EntryPlayerCount);
            }
        });
    }
    private void OnDestroy()
    {
        thisDisposable.Dispose();
    }
}
