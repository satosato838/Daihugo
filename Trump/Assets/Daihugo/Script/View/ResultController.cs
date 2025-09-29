using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class ResultController : MonoBehaviour
{
    [SerializeField] Button _btnTitle;
    [SerializeField] TallotController tallotController;
    [SerializeField] GameObject _objResult;
    [SerializeField] ResultItem[] _resultItems;
    [SerializeField] WinnerCutInController _winnerCutIn;
    private List<DaihugoRoundResult> daihugoRoundResults;

    void Start()
    {
        _btnTitle.onClick.AddListener(() =>
        {
            HideResult();
            tallotController.Show();
        });
        HideResult();
    }

    public void ShowResult(List<DaihugoRoundResult> results)
    {
        _objResult.SetActive(true);
        daihugoRoundResults = results;
        for (var i = 0; i < daihugoRoundResults.Count; i++)
        {
            _resultItems[i].SetResult(i + 1, daihugoRoundResults[i]);
        }
        var winner = daihugoRoundResults.Last().GetResultPlayers.Find(v => v.PlayerRank == DaihugoGameRule.GameRank.DaiHugo);
        _winnerCutIn.Play("WINNER Player" + winner.PlayerId.ToString());
    }

    public void HideResult()
    {
        _objResult.SetActive(false);
        _winnerCutIn.Reset();
        foreach (var item in _resultItems)
        {
            item.Init();
        }
    }

}
