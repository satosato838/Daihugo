using DG.Tweening;
using UnityEngine;

public class PassBalloonObject : MonoBehaviour
{
    private float _showSize = 1;
    public void Show()
    {
        this.gameObject.SetActive(true);
        float val = 0;

        DOTween.Sequence()
       .Append(
           DOTween.To(() => val, v => val = v, _showSize, 0.3f)
               .OnUpdate(() => ImageAnimation(val))
       )
       .OnComplete(() =>
       {
           Invoke(nameof(Hide), 1.0f);
       });
    }

    public void Hide()
    {
        ImageAnimation(0);
        this.gameObject.SetActive(false);
    }
    private void ImageAnimation(float xy)
    {
        this.gameObject.transform.localScale = new Vector3(xy, xy);
    }

}
