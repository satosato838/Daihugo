using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerSelectObject : MonoBehaviour
{
    [SerializeField] private TMP_InputField _txt_playerName;
    [SerializeField] private Image _icon;
    [SerializeField] private Button btn_icon;
    [SerializeField] private Button btn_Select;
    private string iconTxt;
    Action onClick;
    void Start()
    {
        this.gameObject.SetActive(false);
        if (btn_Select != null)
        {
            btn_Select.onClick.AddListener(() =>
            {
                onClick?.Invoke();
            });
        }
        btn_icon.onClick.AddListener(() =>
            {
                //onClick?.Invoke();
            });
    }
    public void Init(string iconName, string playerName, Action action = null)
    {
        _txt_playerName.text = playerName;
        LoadIconImage(iconName);
        onClick = action;
        btn_Select.gameObject.SetActive(onClick != null);

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
