using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ResultController : MonoBehaviour
{
    [SerializeField] Button _btnTitle;
    [SerializeField] TallotController tallotController;
    [SerializeField] GameObject _objResult;
    [SerializeField] ResultItem[] _resultItems;
    private List<DaihugoRoundResult> daihugoRoundResults;

    void Start()
    {
        _btnTitle.onClick.AddListener(() => { tallotController.Show(); });
    }

    public void ShowResult(List<DaihugoRoundResult> results)
    {
        _objResult.SetActive(true);
        daihugoRoundResults = results;
        for (var i = 0; i < daihugoRoundResults.Count; i++)
        {
            _resultItems[i].SetResult(i + 1, daihugoRoundResults[i]);
        }
    }

    public void HideResult()
    {
        _objResult.SetActive(false);
    }

}
