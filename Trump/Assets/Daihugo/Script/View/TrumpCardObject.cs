using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class TrumpCardObject : MonoBehaviour
{
    [SerializeField] private Image _Image;
    TrumpCard TrumpCard;
    void Start()
    {

    }

    public void Init(int playerId)
    {
        StartCoroutine(LoadImage("Player" + "1" + ".png"));
    }

    public void SetCardImage(TrumpCard trumpCard)
    {
        TrumpCard = trumpCard;
        StartCoroutine(LoadImage(trumpCard.CardName + ".png"));
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
}
