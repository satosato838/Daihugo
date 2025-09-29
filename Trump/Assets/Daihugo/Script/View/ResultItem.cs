using System.Linq;
using TMPro;
using UnityEngine;

public class ResultItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _txtRound;
    [SerializeField] private TMP_Text[] _txtPlayerName;
    [SerializeField] private TMP_Text[] _txtPlayerRank;

    public void SetResult(int round, DaihugoRoundResult daihugoRoundResult)
    {
        _txtRound.text = "R" + round.ToString();
        var results = daihugoRoundResult.GetResultPlayers;
        for (var i = 0; i < results.Count; i++)
        {
            _txtPlayerName[i].text = "Player" + results[i].PlayerId.ToString();
            _txtPlayerRank[i].text = results[i].PlayerRank.ToString();
        }
    }

    public void Init()
    {
        _txtRound.text = "--";
        for (var i = 0; i < _txtPlayerName.Length; i++)
        {
            _txtPlayerName[i].text = "--";
            _txtPlayerRank[i].text = "--";
        }
    }
}
