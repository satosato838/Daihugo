using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EffectCutInController : MonoBehaviour
{
    [SerializeField] private Image _base;
    [SerializeField] private int _basePlaySize = 300;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Transform _startPos;
    [SerializeField] private Transform _showPos;
    [SerializeField] private Transform _endPos;

    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        BaseImageAnimation(0);
        _text.transform.localPosition = _startPos.localPosition;
        _text.transform.localScale = new Vector3(0, 0, 0);
    }

    // public void Play(string effectName, float delayTime = 0.0f)
    // {
    //     _text.transform.localScale = new Vector3(1, 1, 1);
    //     _text.text = effectName;
    //     var val = 0;
    //     DOTween.To(() => val, v => val = v, _basePlaySize, 0.3f)
    //     .SetDelay(delayTime)
    //     .OnUpdate(() => { BaseImageAnimation(val); })
    //     .OnComplete(() =>
    //     {
    //         _text.transform.DOLocalMove(_showPos.localPosition, 0.3f).OnComplete(() =>
    //         {
    //             _text.transform.DOLocalMove(_endPos.localPosition, 0.3f)
    //             .SetDelay(1.0f)
    //             .OnComplete(() => Reset());
    //         });
    //     });
    // }
    public void Play(string effectName, float delayTime = 0.0f, Action callback = null)
    {
        _text.transform.localScale = Vector3.one;
        _text.text = effectName;

        float val = 0;
        DOTween.Sequence()
        .Append(
            DOTween.To(() => val, v => val = v, _basePlaySize, 0.3f)
                .OnUpdate(() => BaseImageAnimation(val))
        )
        .Append(_text.transform.DOLocalMove(_showPos.localPosition, 0.3f))
        .AppendInterval(1.0f)
        .Append(_text.transform.DOLocalMove(_endPos.localPosition, 0.3f)).SetDelay(delayTime)
        .Append(
            DOTween.To(() => val, v => val = v, 0, 0.3f)
                .OnUpdate(() => BaseImageAnimation(val))
        )
        .OnComplete(() =>
        {
            Reset();
            callback?.Invoke();
        });
    }

    private void BaseImageAnimation(float y)
    {
        _base.rectTransform.sizeDelta = new Vector2(_base.rectTransform.sizeDelta.x, y);
    }
}
