using System.Collections.Generic;
using UnityEngine;

public class DaihugoController : MonoBehaviour
{
    [SerializeField] private PlayerObject[] playerObjects;
    [SerializeField] private FieldController fieldController;
    private Daihugo Daihugo;
    void Start()
    {
        Daihugo = new Daihugo();
        for (var i = 0; i < Daihugo.GamePlayers.Count; i++)
        {
            playerObjects[i].Init(Daihugo.GamePlayers[i], v =>
            {
                PlayHands(v);
            });
        }
        RefreshPlayersState();
    }

    private void PlayHands(List<TrumpCard> trumpCards)
    {
        Daihugo.RefreshPlayFieldCards(trumpCards);
        foreach (var item in Daihugo.FieldCardPairs)
        {
            Debug.Log("Number:" + item.cardNumber.Number + ": Suit" + item.Suit);
        }
        fieldController.RefreshCards(Daihugo.FieldCardPairs);

        RefreshPlayersState();
    }

    private void RefreshPlayersState()
    {
        for (var i = 0; i < Daihugo.GamePlayers.Count; i++)
        {
            playerObjects[i].RefreshGamePlayerState(Daihugo.GamePlayers[i].IsMyTurn, Daihugo.FieldCardPairs);
        }
    }

}
