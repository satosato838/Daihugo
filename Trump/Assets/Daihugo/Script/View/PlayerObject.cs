using TMPro;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerObject : MonoBehaviour
{
    [SerializeField] private Image _bg;
    [SerializeField] private TextMeshProUGUI _txt_playerName;
    [SerializeField] private TextMeshProUGUI _txt_playerRank;
    [SerializeField] private Color _activeColor = new Color(0.5f, 0.5f, 0, 0.5f);
    [SerializeField] private Color _disActiveColor = new Color(0, 0, 0, 0.5f);
    [SerializeField] private TrumpCardObject trumpCardObject;
    [SerializeField] private HorizontalLayoutGroup _HandPos;
    [SerializeField] private HorizontalLayoutGroup _ExchangeCardPos;
    [SerializeField] private Button _playBtn;
    [SerializeField] private Button _passBtn;

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
                if (Time.time - _lastClickTime < 0.2f) return;
                _lastClickTime = Time.time;
                OnPlayButtonClick();
            });
        }
        if (_passBtn != null)
        {
            _passBtn.onClick.AddListener(() =>
            {
                if (Time.time - _lastClickTime < 0.2f) return;
                _lastClickTime = Time.time;
                OnPassButtonClick();
            });
        }
    }
    public void Init(GamePlayer gamePlayer, Action<int, List<TrumpCard>> playCardCallback, Action<int, List<TrumpCard>> setEndCallback)
    {
        foreach (var item in gamePlayer.HandCards)
        {
            Debug.Log(gamePlayer.PlayerId + ":PlayerItems:" + item.CardName);
        }
        _gamePlayer = new GamePlayer(gamePlayer.PlayerId, gamePlayer.HandCards, gamePlayer.PlayerRank, gamePlayer.DaihugoState, gamePlayer.GameState);
        _txt_playerName.text = "GamePlayer_" + _gamePlayer.PlayerId.ToString();
        SetPlayerRank(_gamePlayer.PlayerRank);
        SetInteractablePlayBtn(false);
        RefreshCards();
        ShowExChangeCards(new List<TrumpCard>());
        playCardAction = playCardCallback;
        roundEndAction = setEndCallback;
        RefreshBGColor();
    }
    public void SetPlayerRank(DaihugoGameRule.GameRank rank)
    {
        _txt_playerRank.text = rank == DaihugoGameRule.GameRank.Heimin ? "" : "Rank:" + rank.ToString();
    }

    public void Kakumei(DaihugoGameRule.DaihugoState state)
    {
        _gamePlayer.RefreshDaihugoState(state);
    }

    public void RefreshGamePlayerState(DaihugoGameRule.GameState gameState, bool isMyTurn, List<TrumpCard> fieldLastCards)
    {
        _gamePlayer.RefreshGameState(gameState);
        if (_gamePlayer.GameState == DaihugoGameRule.GameState.CardChange)
        {
            _bg.color = _activeColor;
        }
        else
        {
            _gamePlayer.RefreshIsMyturn(isMyTurn);
            //Debug.Log($"PlayerObject RefreshGamePlayerState IsMyTurn:{IsMyTurn}: GamePlayerId:{_gamePlayer.PlayerId}");
            if (IsMyTurn)
            {
                //foreach (var item in fieldLastCards) Debug.Log("fieldLastCards:" + item.CardName);
                _gamePlayer.RefreshSelectableHandCards(fieldLastCards);
                RefreshCards();
            }
            else
            {
                RefreshHandCardState(false);
            }
            SetInteractablePlayBtn(IsMyTurn);
            RefreshBGColor();
        }
    }

    private void RefreshBGColor()
    {
        _bg.color = IsMyTurn ? _activeColor : _disActiveColor;
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
            hand.Init(item, true, v =>
            {
                SelectCard(v);
            });

            // if (IsMyPlayer)
            // {
            //     hand.SetCardImage(item);
            // }
            // else
            // {
            //     hand.SetBG();
            // }

            //debug
            hand.ShowFrontCardImage();

            handCardObjects.Add(hand);
        }
        _HandPos.CalculateLayoutInputHorizontal();
        _HandPos.SetLayoutHorizontal();
    }

    public void DeleteExChangeCards()
    {
        foreach (Transform transform in _ExchangeCardPos.transform)
        {
            Destroy(transform.gameObject);
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
            exchangeCard.Init(item, true, v =>
            {

            });

            // if (IsMyPlayer)
            // {
            //     hand.SetCardImage(item);
            // }
            // else
            // {
            //     hand.SetBG();
            // }

            //debug
            exchangeCard.ShowFrontCardImage();
        }
        _ExchangeCardPos.CalculateLayoutInputHorizontal();
        _ExchangeCardPos.SetLayoutHorizontal();

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
            if (_gamePlayer.CurrentSelectCards.Count == 0)
            {
                handCardObjects[i].RefreshOnState(null);
            }
            else
            {
                handCardObjects[i].RefreshOnState(_gamePlayer.CurrentSelectCards.First());
            }
        }
        SetInteractablePlayBtn(_gamePlayer.IsCardPlay);
    }

    private void SetInteractablePlayBtn(bool val)
    {
        if (_playBtn != null) _playBtn.interactable = val;
    }

    public void OnPlayButtonClick()
    {
        playCardAction?.Invoke(_gamePlayer.PlayerId, SelectCards);
        _gamePlayer.PlayCards(v =>
        {
            if (v == 0)
            {
                roundEndAction?.Invoke(_gamePlayer.PlayerId, SelectCards);
            }
        });
        RefreshCards();
    }
    public void OnPassButtonClick()
    {
        playCardAction?.Invoke(_gamePlayer.PlayerId, new List<TrumpCard>());
    }
}
