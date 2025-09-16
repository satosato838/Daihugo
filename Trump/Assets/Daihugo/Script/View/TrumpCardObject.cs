using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TrumpCardObject : MonoBehaviour
{
    [SerializeField] private Image _Image;
    [SerializeField] private Button _button;
    [SerializeField] private Transform _moveUpPos;
    [SerializeField] private Transform _defaultPos;
    TrumpCard TrumpCardData;
    public DaihugoGameRule.Number Number => TrumpCardData.cardNumber.Number;
    Action<TrumpCard> onClick;
    void Start()
    {
        SetDefaultPos();
        _button.onClick.AddListener(() =>
        {
            OnClick();
        });
    }

    public void Init(Action<TrumpCard> onclick)
    {
        onClick = onclick;
    }

    public void SetCardImage(TrumpCard trumpCard)
    {
        TrumpCardData = trumpCard;
        StartCoroutine(LoadImage(trumpCard.CardName + ".png"));
        RefreshCardImagePos();
        _button.interactable = true;
    }

    public void SetBG()
    {
        StartCoroutine(LoadImage("Player" + "1" + ".png"));
    }

    private IEnumerator LoadImage(string key)
    {
        var handle = Addressables.LoadAssetAsync<Sprite>(key);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var image = handle.Result;
            _Image.sprite = image;
        }
        else
        {
            Debug.LogError("Failed to load sprite: " + key);
        }
    }

    public void RefreshOnState(TrumpCard selectFirstCard)
    {

        //nullならボタンはアクティブにして終了
        if (selectFirstCard == null)
        {
            _button.interactable = true;
            return;
        }
        //選択した自分自身なら何もせず終了
        if (TrumpCardData.cardNumber.Number == selectFirstCard.cardNumber.Number &&
            TrumpCardData.Suit == selectFirstCard.Suit)
        {
            return;
        }
        //もし何か選択中なら同じ数字かジョーカーのみ選択可能にする
        bool isInteractable = TrumpCardData.cardNumber.Number == selectFirstCard.cardNumber.Number ||
                              TrumpCardData.cardNumber.Number == DaihugoGameRule.Number.Joker;
        _button.interactable = isInteractable;
    }

    private void OnClick()
    {
        TrumpCard trumpCard = new TrumpCard(TrumpCardData.Suit, TrumpCardData.cardNumber);
        trumpCard.IsSelect = !TrumpCardData.IsSelect;
        TrumpCardData.IsSelect = trumpCard.IsSelect;
        RefreshCardImagePos();

        onClick?.Invoke(trumpCard);
    }

    private void RefreshCardImagePos()
    {
        if (TrumpCardData.IsSelect)
        {
            SetMoveUpPos();
        }
        else
        {
            SetDefaultPos();
        }
    }
    private void SetMoveUpPos()
    {
        _Image.transform.localPosition = _moveUpPos.localPosition;
    }

    private void SetDefaultPos()
    {
        _Image.transform.localPosition = _defaultPos.localPosition;
    }
}
