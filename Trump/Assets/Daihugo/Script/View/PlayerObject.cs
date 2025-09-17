using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerObject : MonoBehaviour
{
    [SerializeField] private Image _bg;
    [SerializeField] private Color _activeColor = new Color(0.5f, 0.5f, 0, 0.5f);
    [SerializeField] private Color _disActiveColor = new Color(0, 0, 0, 0.5f);
    [SerializeField] private TrumpCardObject trumpCardObject;
    [SerializeField] private HorizontalLayoutGroup HandPos;
    [SerializeField] private Button _playBtn;
    [SerializeField] private Button _passBtn;

    private List<TrumpCardObject> handCardObjects;
    private List<TrumpCard> SelectCards => _gamePlayer.CurrentSelectCards;
    private GamePlayer _gamePlayer;
    public bool IsMyTurn => _gamePlayer.IsMyTurn;

    Action<List<TrumpCard>> playCardAction;
    void Start()
    {
        if (_playBtn != null)
        {
            _playBtn.onClick.AddListener(() =>
            {
                OnPlayButtonClick();
            });
        }
        if (_passBtn != null)
        {
            _passBtn.onClick.AddListener(() =>
            {
                OnPlayButtonClick();
            });
        }
    }
    public void Init(GamePlayer gamePlayer, Action<List<TrumpCard>> callback)
    {
        _gamePlayer = gamePlayer;
        SetInteractablePlayBtn(false);
        RefreshCards();
        playCardAction = callback;
        RefreshBGColor();
    }

    public void RefreshGamePlayerState(bool isMyTurn, List<TrumpCard> fieldLastCards)
    {
        _gamePlayer.RefreshIsMyturn(isMyTurn);
        if (IsMyTurn)
        {
            foreach (var item in fieldLastCards) Debug.Log("fieldLastCards:" + item.CardName);
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
    private void RefreshCards()
    {
        handCardObjects = new List<TrumpCardObject>();
        foreach (Transform transform in HandPos.transform)
        {
            Destroy(transform.gameObject);
        }
        foreach (var item in _gamePlayer.Hand)
        {
            var hand = Instantiate(trumpCardObject, HandPos.transform);
            hand.Init(v =>
            {
                SelectCard(v);
            });

            // if (_gamePlayer.PlayerId == 0)
            // {
            //     hand.SetCardImage(item);
            // }
            // else
            // {
            //     hand.SetBG();
            // }

            //debug
            hand.SetCardImage(item);

            handCardObjects.Add(hand);
        }
    }

    public void SelectCard(TrumpCard trumpCard)
    {
        for (var i = 0; i < _gamePlayer.Hand.Count; i++)
        {
            if (_gamePlayer.Hand.Count(c => c.IsSelect) == 0)
            {
                handCardObjects[i].RefreshOnState(null);
                SetInteractablePlayBtn(false);
            }
            else
            {
                handCardObjects[i].RefreshOnState(_gamePlayer.Hand.First(c => c.IsSelect));
                SetInteractablePlayBtn(true);
            }
        }
    }

    private void SetInteractablePlayBtn(bool val)
    {
        if (_playBtn != null) _playBtn.interactable = val;
    }

    public void OnPlayButtonClick()
    {
        playCardAction?.Invoke(SelectCards);
        _gamePlayer.PlayCards();
        RefreshCards();
    }
    public void OnPassButtonClick()
    {
        playCardAction?.Invoke(new List<TrumpCard>());
    }
}
