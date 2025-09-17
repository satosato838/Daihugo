using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldController : MonoBehaviour
{
    [SerializeField] TrumpCardObject trumpCardObject;
    [SerializeField] HorizontalLayoutGroup HandPos;
    private List<TrumpCardObject> trumpCardObjects;
    void Start()
    {
        RefreshCards(new List<TrumpCard>());
    }

    public void RefreshCards(List<TrumpCard> playCards)
    {
        trumpCardObjects = new List<TrumpCardObject>();
        foreach (Transform transform in HandPos.transform)
        {
            Destroy(transform.gameObject);
        }
        foreach (var item in playCards)
        {
            var hand = Instantiate(trumpCardObject, HandPos.transform);
            hand.Init(v =>
            {
                //SelectCard(v);
            });

            //debug
            hand.SetCardImage(item);
            trumpCardObjects.Add(hand);
        }
    }
}