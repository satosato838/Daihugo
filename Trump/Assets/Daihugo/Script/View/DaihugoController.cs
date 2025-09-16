using UnityEngine;

public class DaihugoController : MonoBehaviour
{
    [SerializeField] private PlayerObject[] playerObjects;
    void Start()
    {
        var daihugo = new Daihugo();
        for (var i = 0; i < daihugo.GamePlayers.Count; i++)
        {
            playerObjects[i].Init(daihugo.GamePlayers[i]);
        }
    }

}
