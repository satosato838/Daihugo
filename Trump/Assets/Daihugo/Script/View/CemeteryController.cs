using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class CemeteryController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _txtCemeteryCount;
    [SerializeField] TrumpCardObject _trumpCardObject;
    [SerializeField] Transform _cemeteryPos;
    private List<TrumpCardObject> trumpCardObjects;
    public void Init()
    {
        //Debug.Log("CemeteryController Init");
        RefreshCards(new List<TrumpCard>());
        ResetText();
    }

    public void RefreshCards(List<TrumpCard> playCards)
    {
        trumpCardObjects = new List<TrumpCardObject>();

        foreach (Transform transform in _cemeteryPos.transform)
        {
            Destroy(transform.gameObject);
        }
        foreach (var item in playCards)
        {
            var hand = Instantiate(_trumpCardObject, _cemeteryPos.transform);
            hand.Init(item, false, v =>
            {
                _txtCemeteryCount.text = _cemeteryPos.transform.childCount.ToString();
                Invoke(nameof(ResetText), 1.0f);
            });

            hand.SetBG();
            trumpCardObjects.Add(hand);
        }
    }

    private void ResetText()
    {
        _txtCemeteryCount.text = "";
    }


}
