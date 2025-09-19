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
    public DaihugoGameRule.Number Number => TrumpCardData.Number;
    Action<TrumpCard> onClick;
    void Start()
    {
        SetDefaultPos();
        _button.onClick.AddListener(() =>
        {
            OnClick();
        });
    }

    public void Init(TrumpCard trumpCard, Action<TrumpCard> onclick)
    {
        TrumpCardData = trumpCard;
        onClick = onclick;
    }

    public void ShowFrontCardImage()
    {
        StartCoroutine(LoadImage(TrumpCardData.CardName + ".png"));
        RefreshCardImagePos();
        RefreshButtonInteractable(TrumpCardData.IsSelectable);
    }

    public void RefreshButtonInteractable(bool val)
    {
        if (TrumpCardData.IsSelectable)
        {
            _button.interactable = val;
        }
        else
        {
            _button.interactable = false;
        }
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
            RefreshButtonInteractable(true);
            return;
        }
        //選択した自分自身なら何もせず終了
        if (TrumpCardData.Number == selectFirstCard.Number &&
            TrumpCardData.Suit == selectFirstCard.Suit)
        {
            return;
        }
        //もし何か選択中なら同じ数字かジョーカーのみ選択可能にする
        bool isInteractable = TrumpCardData.Number == selectFirstCard.Number ||
                              TrumpCardData.Number == DaihugoGameRule.Number.Joker;
        RefreshButtonInteractable(isInteractable);
    }

    private void OnClick()
    {
        TrumpCardData.RefreshIsSelect(!TrumpCardData.IsSelect);
        RefreshCardImagePos();
        onClick?.Invoke(TrumpCardData);
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
