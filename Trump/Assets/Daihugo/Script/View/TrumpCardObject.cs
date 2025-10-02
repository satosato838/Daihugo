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
    public bool IsSelect => TrumpCardData.IsSelect;
    public DaihugoGameRule.Number Number => TrumpCardData.Number;
    public DaihugoGameRule.SuitType SuitType => TrumpCardData.Suit;
    private bool IsHand;
    private float _lastClickTime;
    private TrumpCard TrumpCardData;
    private Action<TrumpCard> onClick;
    void Start()
    {
        _button.onClick.AddListener(() =>
        {
            if (Time.time - _lastClickTime < 0.2f) return;
            _lastClickTime = Time.time;
            OnClick();
        });
    }

    public void Init(TrumpCard trumpCard, bool isHand, Action<TrumpCard> onclick = null)
    {
        TrumpCardData = trumpCard;
        IsHand = isHand;
        onClick = onclick;
        RefreshCardImagePos();
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

    public void AutoSelect()
    {
        if (onClick == null) return;
        Debug.Log("AutoSelect TrumpCardData.Number:" + TrumpCardData.Number);
        Debug.Log("AutoSelect TrumpCardData.Suit:" + TrumpCardData.Suit);
        TrumpCardData.RefreshIsSelect(true);
        RefreshCardImagePos();
        onClick?.Invoke(TrumpCardData);
    }

    private void OnClick()
    {
        if (onClick == null) return;
        if (IsHand)
        {
            TrumpCardData.RefreshIsSelect(!TrumpCardData.IsSelect);
            RefreshCardImagePos();
        }

        onClick?.Invoke(TrumpCardData);
    }

    private void RefreshCardImagePos()
    {
        //Debug.Log("RefreshCardImagePos()TrumpCardData.IsSelect:" + TrumpCardData.IsSelect);
        _Image.transform.localPosition = TrumpCardData.IsSelect ? _moveUpPos.localPosition : _defaultPos.localPosition;
    }
}
