using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerObject : MonoBehaviour
{
    [SerializeField] TrumpCardObject trumpCardObject;
    [SerializeField] HorizontalLayoutGroup HandPos;

    private List<TrumpCardObject> trumpCardObjects;
    private List<TrumpCard> SelectCards => GamePlayer.CurrentSelectCards;
    private GamePlayer GamePlayer;
    Action<List<TrumpCard>> playCardAction;
    void Start()
    {

    }

    public void Init(GamePlayer gamePlayer)
    {
        GamePlayer = gamePlayer;
        trumpCardObjects = new List<TrumpCardObject>();
        foreach (var item in GamePlayer.Hand)
        {
            var hand = Instantiate(trumpCardObject, HandPos.transform);
            hand.Init(v =>
            {
                SelectCard(v);
            });

            if (GamePlayer.PlayerId == 0)
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
        for (var i = 0; i < GamePlayer.Hand.Count; i++)
        {
            if (GamePlayer.Hand.Count(c => c.IsSelect) == 0)
            {
                trumpCardObjects[i].RefreshOnState(null);
            }
            else
            {
                trumpCardObjects[i].RefreshOnState(GamePlayer.Hand.First(c => c.IsSelect));
            }
        }
    }

    public void OnPlayButtonClick()
    {
        playCardAction?.Invoke(SelectCards);
    }

}
