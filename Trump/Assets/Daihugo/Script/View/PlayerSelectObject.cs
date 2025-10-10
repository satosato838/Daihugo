using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;

public class PlayerSelectObject : MonoBehaviour
{
    [SerializeField] private TMP_InputField _txt_playerName;
    [SerializeField] private Image _icon;
    [SerializeField] private Button btn_icon;
    [SerializeField] private Button btn_Select;
    public bool isCPU => _txt_playerName.text.Contains("CPU");
    private string iconTxt;
    private int Index;
    Action<int> onClick;
    void Start()
    {
        this.gameObject.SetActive(false);
        if (btn_Select != null)
        {
            btn_Select.onClick.AddListener(() =>
            {
                onClick?.Invoke(Index);
            });
        }
        btn_icon.onClick.AddListener(() =>
            {
                //onClick?.Invoke();
            });
    }
    public void Init(int index, string iconName, string playerName, Action<int> action = null)
    {
        Index = index;
        RefreshName(playerName);
        LoadIconImage(iconName);
        onClick = action;
        btn_Select.gameObject.SetActive(onClick != null);

    }

    public void RefreshName(string playerName)
    {
        _txt_playerName.text = playerName;
    }
    private void LoadIconImage(string iconName)
    {
        iconTxt = iconName;
        if (_icon.sprite != null) return;
        var icon = Addressables.LoadAssetAsync<Sprite>(iconName + ".png");

        icon.Completed += op =>
         {
             if (op.Status == AsyncOperationStatus.Succeeded)
             {
                 _icon.sprite = op.Result;
                 this.gameObject.SetActive(true);
             }
             else if (op.Status == AsyncOperationStatus.Failed)
             {
                 Debug.LogError("LoadIconImage Failed");
             }
         };
    }

    public (string name, string icon) GetNameIconData()
    {
        return (_txt_playerName.text, iconTxt);
    }
}
