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
        TextAdventureGameController.Instance.OnGameRecordChanged += RefreshRecordInfoView;
        
        backBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            TextAdventureGameController.Instance.EndGame(false, true);
        });
    }

    public void RefreshView()
    {
        RefreshRecordInfoView();
    }

    private void RefreshRecordInfoView()
    {
        totalGameTimeTmp.text = TimeSpan.FromSeconds(TextAdventureGameController.Instance.CurGamingModel.StageLevelRecordInfo.PassSeconds)
            .ToString("mm':'ss");
        for (var i = 0; i < starItems.Length; i++)
        {
            starItems[i].RefreshView(i < TextAdventureGameController.Instance.CurGamingModel.StageLevelRecordInfo.CollectStar, false);
        }
    }
}
