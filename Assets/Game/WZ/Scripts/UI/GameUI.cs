using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI totalGameTimeTmp;
    [SerializeField] private SingleStarItem[] starItems;
    [SerializeField] private Button backBtn;
    
    private void Awake()
    {
        GameCenter.Instance.OnGameRecordChanged += RefreshRecordInfoView;
        
        backBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            GameCenter.Instance.EndGame(false, true);
        });
    }

    public void RefreshView()
    {
        RefreshRecordInfoView();
    }

    private void RefreshRecordInfoView()
    {
        totalGameTimeTmp.text = TimeSpan.FromSeconds(GameCenter.Instance.CurGamingModel.StageLevelRecordInfo.PassSeconds)
            .ToString("mm':'ss");
        for (var i = 0; i < starItems.Length; i++)
        {
            starItems[i].RefreshView(i < GameCenter.Instance.CurGamingModel.StageLevelRecordInfo.CollectStar, false);
        }
    }
}
