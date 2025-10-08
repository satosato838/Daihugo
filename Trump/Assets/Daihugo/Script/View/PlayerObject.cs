using TMPro;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerObject : MonoBehaviour
{
    [SerializeField] private Image _bg;
    [SerializeField] private Image _playerIcon;
    [SerializeField] private Image _roundDealerFlag;
    [SerializeField] private TextMeshProUGUI _txt_playerName;
    [SerializeField] private TextMeshProUGUI _txt_playerRank;
    [SerializeField] private Color _activeColor = new Color(0.5f, 0.5f, 0, 0.5f);
    [SerializeField] private Color _disActiveColor = new Color(0, 0, 0, 0.5f);
    [SerializeField] private TrumpCardObject trumpCardObject;
    [SerializeField] private HorizontalLayoutGroup _HandPos;
    [SerializeField] private HorizontalLayoutGroup _ExchangeCardPos;
    [SerializeField] private Button _playBtn;
    [SerializeField] private PassBalloonObject _passBalloon;
    [SerializeField] private Button _passBtn;
    private bool IsDebug;

    private bool IsMyPlayer => _gamePlayer.PlayerId == 0;
    public int PlayerId => _gamePlayer.PlayerId;

    private List<TrumpCardObject> handCardObjects;
    private List<TrumpCard> SelectCards => _gamePlayer.CurrentSelectCards;
    private DaihugoGameRule.GameState _gameState => _gamePlayer.GameState;
    private GamePlayer _gamePlayer;
    public bool IsMyTurn => _gamePlayer.IsMyTurn;
    private Action<int, List<TrumpCard>> playCardAction;
    private Action<int, List<TrumpCard>> roundEndAction;
    private float _lastClickTime;
    void Start()
    {
        if (_playBtn != null)
        {
            _playBtn.onClick.AddListener(() =>
            {
                if (_gamePlayer.IsCPU) return;
                if (Time.time - _lastClickTime < 0.2f) return;
                _lastClickTime = Time.time;
                OnPlayButtonClick();
            });
        }
        if (_passBtn != null)
        {
            _passBtn.onClick.AddListener(() =>
            {
                if (_gamePlayer.IsCPU) return;
                if (Time.time - _lastClickTime < 0.2f) return;
                _lastClickTime = Time.time;
                OnPassButtonClick();
            });
        }
    }
    public void Init(GamePlayer gamePlayer, DaihugoGameRule.GameState state, Action<int, List<TrumpCard>> playCardCallback, Action<int, List<TrumpCard>> setEndCallback, bool isDebug)
    {
        // foreach (var item in gamePlayer.HandCards)
        // {
        //     Debug.Log(gamePlayer.PlayerId + ":PlayerItems:" + item.CardName);
        // }
        IsDebug = isDebug;
        _gamePlayer = new GamePlayer(gamePlayer.PlayerId, gamePlayer.PlayerName, gamePlayer.PlayerIconImageName,
              gamePlayer.PlayerRank, gamePlayer.DaihugoState, gamePlayer.GameState, gamePlayer.IsCPU);

        _txt_playerName.text = _gamePlayer.PlayerName;

        SetPlayerRank(_gamePlayer.PlayerRank);
        SetInteractablePlayBtn(false);
        ShowExChangeCards(new List<TrumpCard>());
        RefreshMyTurn();
        RefreshDealer(false);
        LoadIconImage();
        _gamePlayer.RefreshGameState(state);
        _passBalloon.Hide();
        playCardAction = playCardCallback;
        roundEndAction = setEndCallback;
        if (IsDebug)
        {
            _playBtn.gameObject.SetActive(true);
            _passBtn.gameObject.SetActive(true);
        }
        else
        {
            _playBtn.gameObject.SetActive(!gamePlayer.IsCPU);
            _passBtn.gameObject.SetActive(!gamePlayer.IsCPU);
        }
    }

    public void DealCard(List<TrumpCard> trumpCards)
    {
        _gamePlayer.DealCard(trumpCards);
        RefreshCards();
    }

    private void LoadIconImage()
    {
        if (_playerIcon.sprite != null) return;
        var icon = Addressables.LoadAssetAsync<Sprite>(_gamePlayer.PlayerIconImageName + ".png");
        icon.Completed += op =>
         {
             if (op.Status == AsyncOperationStatus.Succeeded)
             {
                 _playerIcon.sprite = op.Result;
                 //_playerIcon.SetNativeSize();
             }
             else if (op.Status == AsyncOperationStatus.Failed)
             {
                 Debug.LogError("LoadIconImage Failed");
             }
         };
    }

    public void SetPlayerRank(DaihugoGameRule.GameRank rank)
    {
        _txt_playerRank.text = rank == DaihugoGameRule.GameRank.Heimin ? "" : "Rank:" + rank.ToString();
    }

    public void Kakumei(DaihugoGameRule.DaihugoState state)
    {
        _gamePlayer.RefreshDaihugoState(state);
    }

    public void RefreshGamePlayerState(DaihugoGameRule.GameState gameState, bool isDealer, bool isMyTurn, List<TrumpCard> fieldLastCards)
    {
        _gamePlayer.RefreshGameState(gameState);
        if (_gamePlayer.GameState == DaihugoGameRule.GameState.CardChange)
        {
            _bg.color = _activeColor;
        }
        else
        {
            _gamePlayer.RefreshIsMyturn(isMyTurn);
            _gamePlayer.RefreshDealer(isDealer);
            //Debug.Log($"PlayerObject RefreshGamePlayerState IsMyTurn:{IsMyTurn}: GamePlayerId:{_gamePlayer.PlayerId}");
            if (IsMyTurn)
            {
                //foreach (var item in fieldLastCards) Debug.Log("fieldLastCards:" + item.CardName);
                _gamePlayer.RefreshSelectableHandCards(fieldLastCards);
                RefreshCards();
                SetInteractablePlayBtn(_gamePlayer.HavePlayCard);

            }
            else
            {
                RefreshHandCardState(false);
                SetInteractablePlayBtn(false);
            }
            RefreshDealer(_gamePlayer.IsDealer);
            RefreshMyTurn();
        }
    }

    private void RefreshMyTurn()
    {
        _bg.color = IsMyTurn ? _activeColor : _disActiveColor;
        _passBtn.interactable = IsMyTurn;
    }

    public void RefreshDealer(bool val)
    {
        _roundDealerFlag.gameObject.SetActive(val);
    }

    private void RefreshHandCardState(bool val)
    {
        foreach (var item in handCardObjects)
        {
            item.RefreshButtonInteractable(val);
        }
    }
    public void RefreshCards()
    {
        handCardObjects = new List<TrumpCardObject>();
        foreach (Transform transform in _HandPos.transform)
        {
            Destroy(transform.gameObject);
        }
        var scale = IsMyPlayer ? 1 : 0.3f;
        _HandPos.transform.localScale = new Vector3(scale, scale, scale);
        foreach (var item in _gamePlayer.HandCards)
        {
            var hand = Instantiate(trumpCardObject, _HandPos.transform);
            hand.Init(item, isHand: true, isButton: !_gamePlayer.IsCPU, v =>
            {
                SelectCard(v);
            });

            if (IsDebug)
            {
                hand.ShowFrontCardImage();
            }
            else
            {
                if (IsMyPlayer)
                {
                    hand.ShowFrontCardImage();
                }
                else
                {
                    hand.SetBG();
                }
            }

            handCardObjects.Add(hand);
        }
        _HandPos.CalculateLayoutInputHorizontal();
        _HandPos.SetLayoutHorizontal();
        SetInteractablePlayBtn(_gamePlayer.IsCardPlay);
    }

    public void DeleteExChangeCards()
    {
        foreach (Transform transform in _ExchangeCardPos.transform)
        {
            Destroy(transform.gameObject);
        }
    }

    public void ShowHandCards()
    {
        if (IsDebug)
        {
            foreach (var card in handCardObjects)
            {
                Debug.Log($"ShowHandCards {card.SuitType},{card.Number} IsSelect:" + card.IsSelect);
            }
        }
    }

    public void ShowExChangeCards(List<TrumpCard> trumpCards)
    {
        DeleteExChangeCards();
        var scale = IsMyPlayer ? 1 : 0.3f;
        _ExchangeCardPos.transform.localScale = new Vector3(scale, scale, scale);
        foreach (var item in trumpCards)
        {
            var exchangeCard = Instantiate(trumpCardObject, _ExchangeCardPos.transform);
            exchangeCard.Init(new TrumpCard(item.Suit,
                              new CardNumber(item.Number)),
                              isHand: false,
                              isButton: !_gamePlayer.IsCPU);

            if (IsDebug)
            {
                exchangeCard.ShowFrontCardImage();
            }
            else
            {
                if (IsMyPlayer)
                {
                    exchangeCard.ShowFrontCardImage();
                }
                else
                {
                    exchangeCard.SetBG();
                }
            }
        }
        _ExchangeCardPos.CalculateLayoutInputHorizontal();
        _ExchangeCardPos.SetLayoutHorizontal();

    }

    public void AutoPlayCard(List<TrumpCard> cards)
    {
        foreach (var item in cards)
        {
            handCardObjects.First(cObject => cObject.Number == item.Number && cObject.SuitType == item.Suit)
                           .AutoSelect();
        }
        // foreach (var hand in handCardObjects)
        // {
        //     Debug.Log($"AutoPlayCard :{hand.SuitType},{hand.Number}:IsSelect:{hand.IsSelect}");
        // }
        OnPlayButtonClick();
    }

    public void SelectCard(TrumpCard trumpCard)
    {
        _gamePlayer.SelectCard(trumpCard);
        if (_gameState == DaihugoGameRule.GameState.CardChange)
        {
            RefreshHandCardState(true);
            SetInteractablePlayBtn(_gamePlayer.IsCardChangePlay);
            return;
        }

        for (var i = 0; i < _gamePlayer.HandCards.Count; i++)
        {
            //Debug.Log("_gamePlayer.CurrentSelectCards.Count == 0");
            handCardObjects[i].RefreshOnState(_gamePlayer.CurrentSelectCards.Count == 0 ? null : _gamePlayer.CurrentSelectCards.First());
        }
        SetInteractablePlayBtn(_gamePlayer.IsCardPlay);
    }

    private void SetInteractablePlayBtn(bool val)
    {
        //Debug.Log($"_gamePlayer{_gamePlayer.PlayerId} SetInteractablePlayBtn {val}:");
        if (_playBtn != null) _playBtn.interactable = val;
    }

    public void UpdateSelectableCardsForExchange()
    {
        _gamePlayer.UpdateSelectableCardsForExchange();
        for (var i = 0; i < _gamePlayer.HandCards.Count; i++)
        {
            var card = handCardObjects[i];
            var cardData = _gamePlayer.HandCards[i];
            card.RefreshButtonInteractable(cardData.IsSelectable);
        }
        if (_gamePlayer.IsCPU)
        {
            AutoPlayCard(_gamePlayer.HandCards.Where(c => c.IsSelect).ToList());
        }
    }
    private void OnPlayButtonClick()
    {
        List<TrumpCard> trumpCards = new List<TrumpCard>();
        foreach (var card in SelectCards)
        {
            //Debug.Log("SelectCard:" + card.CardName);
            trumpCards.Add(new TrumpCard(card.Suit, new CardNumber(card.Number)));
        }
        _gamePlayer.PlayCards(v =>
        {
            if (_gamePlayer.GameState == DaihugoGameRule.GameState.GamePlay && v == 0)
            {
                roundEndAction?.Invoke(_gamePlayer.PlayerId, trumpCards);
            }
        });

        RefreshCards();
        if (trumpCards.Count == 0)
        {
            _passBalloon.Show();
        }
        playCardAction?.Invoke(_gamePlayer.PlayerId, trumpCards);
    }

    public void OnPassButtonClick()
    {
        _passBalloon.Show();
        playCardAction?.Invoke(_gamePlayer.PlayerId, new List<TrumpCard>());
    }
}
