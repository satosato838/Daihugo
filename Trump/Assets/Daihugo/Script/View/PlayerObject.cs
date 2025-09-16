using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerObject : MonoBehaviour
{
    [SerializeField] TrumpCardObject trumpCardObject;
    [SerializeField] HorizontalLayoutGroup HandPos;

    private GamePlayer GamePlayer;
    void Start()
    {

    }

    public void Init(GamePlayer gamePlayer)
    {
        GamePlayer = gamePlayer;
        foreach (var item in GamePlayer.Hand)
        {
            var hand = Instantiate(trumpCardObject, HandPos.transform);

            // if (GamePlayer.PlayerId == 0)
            // {
            //     hand.SetCardImage(item);
            // }
            // else
            // {
            //     hand.Init(GamePlayer.PlayerId);
            // }
            hand.SetCardImage(item);
        }
    }

}
