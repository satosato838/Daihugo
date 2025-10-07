using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FieldController : MonoBehaviour
{
    [SerializeField] TrumpCardObject _trumpCardObject;
    [SerializeField] HorizontalLayoutGroup _fieldPos;
    private List<TrumpCardObject> trumpCardObjects;
    public void Init()
    {
        RefreshCards(new List<TrumpCard>());
    }

    public void RefreshCards(List<TrumpCard> playCards)
    {
        //Debug.Log("FieldController RefreshCards playCardsCount:" + playCards.Count);
        trumpCardObjects = new List<TrumpCardObject>();
        foreach (Transform transform in _fieldPos.transform)
        {
            Destroy(transform.gameObject);
        }
        foreach (var item in playCards)
        {
            var hand = Instantiate(_trumpCardObject, _fieldPos.transform);
            hand.Init(new TrumpCard(item.Suit, new CardNumber(item.Number)), isHand: false, isButton: false, v =>
            {
                //SelectCard(v);
            });

            hand.ShowFrontCardImage();

            trumpCardObjects.Add(hand);
        }
    }
}