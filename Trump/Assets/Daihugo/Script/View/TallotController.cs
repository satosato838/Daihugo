using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class TallotController : MonoBehaviour
{
    [SerializeField] GameObject GridPrefab;
    [SerializeField] CardObject TallotPrefab;
    [SerializeField] Sprite[] Tallots;
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
