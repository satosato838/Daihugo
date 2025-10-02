using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TallotController : MonoBehaviour
{
    [SerializeField] GameObject _objTitle;
    [SerializeField] GameObject GridPrefab;
    [SerializeField] CardObject TallotPrefab;
    [SerializeField] Sprite[] Tallots;
    [SerializeField] private string[] playerNames = { "Player1", "Player2", "Player3", "Player4" };
    [SerializeField] private string[] playerIcons = { "owl", "owl", "owl", "owl" };


    [SerializeField] DaihugoController _daihugoController;

    [SerializeField] Button _btn_GameStart;
    [SerializeField] Button _btn_Quit;
    void Start()
    {
        for (var i = 0; i < 4; i++)
        {
            foreach (var item in Tallots)
            {
                var tallotPrefab = Instantiate(TallotPrefab.gameObject, GridPrefab.transform).GetComponent<CardObject>();
                tallotPrefab.Init(new Tallot(item));
            }
        }
        _btn_GameStart.onClick.AddListener(() => { GameStart(); });
        _btn_Quit.onClick.AddListener(() => { Quit(); });
        Show();
    }

    public void Show()
    {
        _objTitle.SetActive(true);
    }

    private void GameStart()
    {
        _objTitle.SetActive(false);
        List<GamePlayer> players = new List<GamePlayer>();
        for (int i = 0; i < playerNames.Length; i++)
        {
            players.Add(new GamePlayer(i, playerNames[i], playerIcons[i], DaihugoGameRule.GameRank.Heimin, DaihugoGameRule.DaihugoState.None, DaihugoGameRule.GameState.None));

        }
        _daihugoController.GameStart(players);
    }

    private void Quit()
    {
        Application.Quit();
    }

}

public class Tallot
{
    public Sprite Sprite;
    public Tallot(Sprite sprite)
    {
        Sprite = sprite;
    }
}
