using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerObject : MonoBehaviour
{
    [SerializeField] TrumpCardObject trumpCardObject;
    [SerializeField] HorizontalLayoutGroup HandPos;
    [SerializeField] Button _playBtn;

    private List<TrumpCardObject> trumpCardObjects;
    private List<TrumpCard> SelectCards => _gamePlayer.CurrentSelectCards;
    private GamePlayer _gamePlayer;
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
    }
    public void Init(GamePlayer gamePlayer, Action<List<TrumpCard>> callback)
    {
        _gamePlayer = gamePlayer;
        SetInteractable(false);
        RefreshCards();
        playCardAction = callback;
    }
    private void RefreshCards()
    {
        trumpCardObjects = new List<TrumpCardObject>();
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

            if (_gamePlayer.PlayerId == 0)
            {
                hand.SetCardImage(item);
            }
            else
            {
                hand.SetBG();
            }
            //debug
            //hand.SetCardImage(item);
            trumpCardObjects.Add(hand);
        }
    }

    public void SelectCard(TrumpCard trumpCard)
    {
        for (var i = 0; i < _gamePlayer.Hand.Count; i++)
        {
            if (_gamePlayer.Hand.Count(c => c.IsSelect) == 0)
            {
                trumpCardObjects[i].RefreshOnState(null);
                SetInteractable(false);
            }
            else
            {
                trumpCardObjects[i].RefreshOnState(_gamePlayer.Hand.First(c => c.IsSelect));
                SetInteractable(true);
            }
        }
    }

    private void SetInteractable(bool val)
    {
        if (_playBtn != null) _playBtn.interactable = val;
    }

    public void OnPlayButtonClick()
    {
        playCardAction?.Invoke(SelectCards);
        _gamePlayer.PlayCards();
        RefreshCards();
    }



}
