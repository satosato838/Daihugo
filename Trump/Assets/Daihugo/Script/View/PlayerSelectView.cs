using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectView : MonoBehaviour
{
    [SerializeField] private GameObject _view;
    [SerializeField] private PlayerSelectObject[] playerIcon;
    [SerializeField] private string[] playerIcons = { "skullwing_fire", "slug", "worm", "narwhal" };
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
        for (int i = 0; i < playerIcon.Length; i++)
        {
            playerIcon[i].Init(playerIcons[i],
            (i == 0 ? "Player" : "CPU") + (i + 1));
        }
        _view.SetActive(false);
    }

    public void Init(bool isOnePlayerMode, Action<List<(string name, string icon)>> action)
    {
        _view.SetActive(true);

        for (int i = 0; i < playerIcon.Length; i++)
        {
            playerIcon[i].Init(playerIcons[i],
                               isOnePlayerMode ? (i == 0 ? "Player" : "CPU") + (i + 1) : "Player" + (i + 1));
        }
        onClick = action;
    }

}
