using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class TallotController : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject _objTitle;
    [SerializeField] GameObject GridPrefab;
    [SerializeField] CardObject TallotPrefab;
    [SerializeField] Sprite[] Tallots;
    [SerializeField] private string[] playerNames = { "Player1", "Player2", "Player3", "Player4" };
    [SerializeField] private string[] playerIcons = { "owl", "owl", "owl", "owl" };

    [SerializeField] private PlayerSelectView _selectView;


    [SerializeField] DaihugoController _daihugoController;

    [SerializeField] Button _btn_GameStart;
    [SerializeField] Button _btn_Online;
    [SerializeField] Button _btn_Quit;
    void Start()
    {
        PhotonNetwork.Disconnect();
        for (var i = 0; i < 4; i++)
        {
            foreach (var item in Tallots)
            {
                var tallotPrefab = Instantiate(TallotPrefab.gameObject, GridPrefab.transform).GetComponent<CardObject>();
                tallotPrefab.Init(new Tallot(item));
            }
        }
        _btn_GameStart.onClick.AddListener(() => { _selectView.Init(true, v => { OnePlayerGameStart(v); }); });
        _btn_Quit.onClick.AddListener(() => { Quit(); });
        _btn_Online.onClick.AddListener(() =>
        {
            PhotonNetwork.NickName = "Player";
            //PhotonNetwork.GetCustomRoomList(TypedLobby.Default,);
            // "Room"という名前のルームに参加する（ルームが存在しなければ作成して参加する）
            PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
            //SoundManager.Instance.PlaySE(SESoundData.SE.buttonpush);
            _selectView.Init(false, v => { OnePlayerGameStart(v); });
        });
        Show();
    }

    public void Show()
    {
        _objTitle.SetActive(true);
    }

    private void OnePlayerGameStart(List<(string name, string icon)> playerDatas)
    {
        _objTitle.SetActive(false);
        List<GamePlayer> players = new List<GamePlayer>();
        for (int i = 0; i < playerNames.Length; i++)
        {
            players.Add(new GamePlayer(i, playerDatas[i].name, playerDatas[i].icon,
            DaihugoGameRule.GameRank.Heimin, DaihugoGameRule.DaihugoState.None, DaihugoGameRule.GameState.None, isCPU: playerNames[i].Contains("CPU")));
        }
        _daihugoController.GameStart(players);
    }


    // マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        //_modalview.gameObject.SetActive(true);
    }

    // ゲームサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        // _onlineView.Show();
        // Hide();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogError($"OnJoinRandomFailed:" + message);
    }

    private void Quit()
    {
        PhotonNetwork.Disconnect();
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
