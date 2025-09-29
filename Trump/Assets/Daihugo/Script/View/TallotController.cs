using UnityEngine;
using UnityEngine.UI;

public class TallotController : MonoBehaviour
{
    [SerializeField] GameObject _objTitle;
    [SerializeField] GameObject GridPrefab;
    [SerializeField] CardObject TallotPrefab;
    [SerializeField] Sprite[] Tallots;

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
        _objTitle.SetActive(true);
        _btn_GameStart.onClick.AddListener(() => { GameStart(); });
        _btn_Quit.onClick.AddListener(() => { Quit(); });
    }

    private void GameStart()
    {
        _objTitle.SetActive(false);
        _daihugoController.GameStart();
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
