using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectView : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    [SerializeField] private GameObject _view;
    [SerializeField] private PlayerSelectObject[] playerIcon;
    [SerializeField] private string[] playerIcons = { "skullwing_fire", "slug", "worm", "narwhal" };
    List<string> names = new List<string> { "Player1", "CPU2", "CPU3", "CPU4" };
    [SerializeField] private Button btn_GameStart;
    Action<List<(string name, string icon)>> onClick;
    void Start()
    {
        btn_GameStart.onClick.AddListener(() =>
        {

            _view.SetActive(false);
            List<(string name, string icon)> playerDatas = new List<(string name, string icon)>();
            foreach (var item in playerIcon)
            {
                playerDatas.Add(item.GetNameIconData());
            }
            onClick?.Invoke(playerDatas);
        });
        _view.SetActive(false);
    }

    public void Init(bool isOnePlayerMode, Action<List<(string name, string icon)>> action)
    {
        _view.SetActive(true);
        RefreshPlayers(isOnePlayerMode);
        onClick = action;
    }
    private void RefreshPlayers(bool isOnePlayerMode)
    {
        if (!isOnePlayerMode)
        {
            var index = 0;
            foreach (var item in PhotonNetwork.CurrentRoom.Players)
            {
                names[index] = item.Value.NickName;
                index++;
            }
        }

        for (int i = 0; i < playerIcon.Length; i++)
        {
            if (isOnePlayerMode)
            {
                playerIcon[i].Init(i, playerIcons[i], names[i]);
            }
            else
            {
                playerIcon[i].Init(i, playerIcons[i], names[i],
                v => { playerIcon[v].RefreshName(playerIcon[v].isCPU ? names[i] : "CPU" + (v + 1)); });
            }
        }
    }

    // ゲームサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom IsMasterClient:" + PhotonNetwork.LocalPlayer.IsMasterClient);
        Debug.Log("MasterClientId:" + PhotonNetwork.CurrentRoom.MasterClientId);
        RefreshPlayers(false);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogError($"OnJoinRandomFailed:" + message);
    }
}
