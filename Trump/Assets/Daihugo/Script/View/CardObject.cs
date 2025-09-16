using UnityEngine;
using UnityEngine.UI;

public class CardObject : MonoBehaviour
{
    [SerializeField] private Image _image;

    public void Init(Tallot tallot)
    {
        _image.sprite = tallot.Sprite;
    }
}
