using System;
using TMPro;
using UnityEngine;

public class CoinUIPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinCountTmp;

    private void OnEnable()
    {
        RefreshView();
    }

    private void Awake()
    {
        WealthManager.Instance.OnWealthChanged += RefreshView;
    }
    
    private void RefreshView()
    {
        coinCountTmp.text = WealthManager.Instance.CurWealthModel.CoinCount.ToString();
    }
}
