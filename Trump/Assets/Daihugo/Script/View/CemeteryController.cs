using System.Collections.Generic;
using UnityEngine;

public class CemeteryController : MonoBehaviour
{
    [SerializeField] TrumpCardObject trumpCardObject;
    [SerializeField] Transform FieldPos;
    private List<TrumpCardObject> trumpCardObjects;
    public void Init()
    {
        RefreshCards(new List<TrumpCard>());
    }

    public void RefreshCards(List<TrumpCard> playCards)
    {
        trumpCardObjects = new List<TrumpCardObject>();
        foreach (Transform transform in FieldPos.transform)
        {
            Destroy(transform.gameObject);
        }
        foreach (var item in playCards)
        {
            var hand = Instantiate(trumpCardObject, FieldPos.transform);
            hand.Init(v =>
            {
                //SelectCard(v);
            });

            hand.SetBG();
            trumpCardObjects.Add(hand);
        }
    }
}
