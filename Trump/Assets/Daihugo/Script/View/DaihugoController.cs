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

    IEnumerator enumerator;

    IDisposable thisDisposable;
    void Start()
    {
        _daihugoObject.SetActive(false);
    }

    public void GameStart(List<GamePlayer> players)
    {
        _daihugoObject.SetActive(true);
        _daihugoInstance = new Daihugo(roundCount: _isDebug ? 2 : 5, isDebug: _isDebug, isDebugCard: _isDebugCard);
        thisDisposable = _daihugoInstance.Subscribe(this);
        StartRound(players);
    }

    private void StartRound(List<GamePlayer> players)
    {
        _daihugoInstance.RoundStart(players);
    }

    private void PlayHands(int playerId, List<TrumpCard> trumpCards)
    {
        if (_daihugoInstance.GetGameCurrentState == DaihugoGameRule.GameState.GamePlay)
        {
            _daihugoInstance.PlayHands(trumpCards);
            _fieldController.RefreshCards(_daihugoInstance.LastFieldCardPair);
        }
        else if (_daihugoInstance.GetGameCurrentState == DaihugoGameRule.GameState.CardChange)
        {
            foreach (var item in trumpCards)
            {
                Debug.Log("<color=cyan> CardChange trumpCard:" + item.CardName + "</color>");
            }
            _daihugoInstance.ExecuteCardExchange(playerId, trumpCards);
            if (_isDebug)
            {
                _playerObjects[playerId].ShowExChangeCards(trumpCards);
            }
            else
            {
                var player = _playerObjects.First(p => p.PlayerId == playerId);
                player.ShowExChangeCards(trumpCards);
            }
        }
        else
        {
            _fieldController.RefreshCards(trumpCards);
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
            var id = _playerObjects[i].PlayerId;
            var gamePlayerData = _daihugoInstance.GamePlayers.First(p => p.PlayerId == id);
            _playerObjects[i].RefreshGamePlayerState(state,
                                                     gamePlayerData.IsDealer,
                                                     gamePlayerData.IsMyTurn,
                                                    _daihugoInstance.LastFieldCardPair);
        }
    }
    private void RefreshPlayerCards()
    {
        for (var i = 0; i < _playerObjects.Length; i++)
        {
            _playerObjects[i].RefreshCards();
        }
    }

    private IEnumerator CpuPlay()
    {
        yield return new WaitForSeconds(2.0f);
        var player = _daihugoInstance.CurrentGamePlayer;
        _daihugoInstance.CPUCardPlay(player, v =>
            {
                var playerObject = _playerObjects.First(pObject => pObject.PlayerId == player.PlayerId);
                playerObject.AutoPlayCard(v);
            });
    }

    private void RefreshPlayerRank(int goOutPlayerIndex)
    {
        var goOutPlayer = _daihugoInstance.GamePlayers[goOutPlayerIndex];
        var currentPlayer = _playerObjects.First(pObject => pObject.PlayerId == goOutPlayer.PlayerId);
        currentPlayer.SetPlayerRank(goOutPlayer.PlayerRank);
    }

    public void OnStartRound(DaihugoGameRule.GameState state)
    {
        Debug.Log($"<color=red> OnStartRound DaihugoGameRule.GameState {state}, CurrentPlayerId:{_daihugoInstance.CurrentPlayerId} </color>");
        foreach (var player in _daihugoInstance.GamePlayers)
        {
            Debug.Log($"OnStartRound isCPU{player.IsCPU} player" + player.PlayerName);
            var playerObject = _playerObjects[player.PlayerId];
            if (!_isDebug)
            {
                if (_daihugoInstance.CurrentRoundIndex > 1)
                {
                    playerObject = _playerObjects.First(pObject => pObject.PlayerId == player.PlayerId);
                }
            }

            playerObject.Init(player,
            state,
            (id, v) =>
            {
                PlayHands(id, v);
            },
            (id, v) =>
            {
                EndRoundPlayer(id, v);
            },
            isDebug: _isDebug
            );
            playerObject.DealCard(player.HandCards);
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
                StageStart();
            }
        });
    }

    private void StageStart()
    {
        Debug.Log($"<color=yellow> StageStart DaihugoGameRule.GameState {_daihugoInstance.GetGameCurrentState}</color>");
        if (_daihugoInstance.GetGameCurrentState == DaihugoGameRule.GameState.GamePlay)
        {
            _daihugoInstance.StageStart();
        }
    }
    public void OnStartStage()
    {
        _fieldController.Init();
        _cemeteryController.RefreshCards(_daihugoInstance.CemeteryCards);
        RefreshPlayersState(_daihugoInstance.GetGameCurrentState);
        var currentPlayer = _daihugoInstance.CurrentGamePlayer;
        Debug.Log($"<color=yellow> OnStartStage DaihugoGameRule.GameState {_daihugoInstance.GetGameCurrentState},currentPlayer.IsCPU:{currentPlayer.IsCPU} CurrentPlayerId:{_daihugoInstance.CurrentPlayerId} </color>");
        if (enumerator != null)
        {
            StopCoroutine(enumerator);
        }
        if (currentPlayer.IsCPU)
        {
            enumerator = CpuPlay();
            StartCoroutine(enumerator);
        }
    }
    public void OnChangePlayerTurn(DaihugoGameRule.Effect effect, GamePlayer currentPlayer)
    {
        Debug.Log($"<color=cyan> OnChangePlayerTurn currentPlayerName:{currentPlayer.PlayerName}</color>");
        RefreshPlayersState(_daihugoInstance.GetGameCurrentState);
        if (enumerator != null)
        {
            StopCoroutine(enumerator);
        }

        if (effect == DaihugoGameRule.Effect.Eight_Enders ||
            effect == DaihugoGameRule.Effect.Counter_Spade_3) return;

        if (currentPlayer.IsCPU)
        {
            var number = _daihugoInstance.LastFieldCardPair.FirstOrDefault();
            if (number != null) Debug.Log($"<color=red> OnChangePlayerTurn IsCPU LastFieldCard {number.Number} </color>");

            enumerator = CpuPlay();
            StartCoroutine(enumerator);
        }
        else
        {
            var number = _daihugoInstance.LastFieldCardPair.FirstOrDefault();
            if (number != null) Debug.Log($"<color=cyan> OnChangePlayerTurn LastFieldCard {_daihugoInstance.LastFieldCardPair.First().Number} </color>");
        }
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

    public void OnEndStage(DaihugoGameRule.Effect effect)
    {
        Debug.Log("<color=yellow>" + "OnEndStage GetGameCurrentState:" + _daihugoInstance.GetGameCurrentState + "</color>");
        RefreshPlayersState(_daihugoInstance.GetGameCurrentState);
        _fieldController.RefreshCards(_daihugoInstance.LastFieldCardPair);
        if (effect == DaihugoGameRule.Effect.Eight_Enders ||
            effect == DaihugoGameRule.Effect.Counter_Spade_3) return;
        StageStart();
    }
    public void OnCardEffect(DaihugoGameRule.Effect effect)
    {
        Debug.Log($"<color=cyan> OnCardEffect:{effect}</color>");
        string effectName = effect switch
        {
            DaihugoGameRule.Effect.Eight_Enders => "8 ENDERS",
            DaihugoGameRule.Effect.Counter_Spade_3 => "COUNTER SPADE 3",
            DaihugoGameRule.Effect.Eleven_Back => "11 BACK",
            _ => throw new Exception(),
        };

        _effectCutInController.Play(effectName, 0.5f, () =>
        {
            if (effect == DaihugoGameRule.Effect.Eight_Enders ||
                effect == DaihugoGameRule.Effect.Counter_Spade_3)
            {
                StageStart();
            }
        });
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
                if (_isDebug)
                {
                    for (var i = 0; i < _playerObjects.Length; i++)
                    {
                        Debug.Log($"<color=cyan>_playerObjects[{i}] PlayerId: {_playerObjects[i].PlayerId} </color>");
                        _playerObjects[i].ShowHandCards();
                    }
                }
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
                if (_isDebug)
                {
                    for (var i = 0; i < _playerObjects.Length; i++)
                    {
                        Debug.Log($"<color=yellow>_playerObjects[{i}]: {_playerObjects[i].PlayerId} </color>");
                        _playerObjects[i].ShowHandCards();
                    }
                }
                _effectCutInController.Play("PLAY THE GAME", 0.5f, () =>
                {
                    StageStart();
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
        Debug.Log("<color=green>" + "OnToGoOut goOutPlayerIndex:" + goOutPlayerIndex + "</color>");
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
                thisDisposable?.Dispose();
            }
            else
            {
                StartRound(_daihugoInstance.GetRoundResults.Last().GetResultPlayers);
            }
        });
    }
    private void OnDestroy()
    {
        thisDisposable?.Dispose();
    }
}
