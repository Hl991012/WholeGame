using System;
using NMNH.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private TextAdventureUIPresenter textAdventureUIPresenter;
    [SerializeField] private GameObject winObj;
    [SerializeField] private GameObject loseObj;
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button nextBtn;
    [SerializeField] private Button onceAgainBtn;
    [SerializeField] private TextMeshProUGUI playTimeTmp;
    [SerializeField] private SingleStarItem[] singleStarItems;
    [SerializeField] private TextMeshProUGUI rewardCoinCount;
    
    private void Awake()
    {
        homeBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            TextAdventureGameController.Instance.ChangeState(TextAdventureGameController.GameState.Home);
        });
        
        nextBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            if (TextAdventureGameController.Instance.CurGamingModel.CurrentStageLevel + 1 > StageConfigManager.Instance.TotalStageCount)
            {
                textAdventureUIPresenter.ShowPopUps("更多关卡制作中~");
            }
            else
            {
                TextAdventureGameController.Instance.StartGame(TextAdventureGameController.Instance.CurGamingModel.CurrentStageLevel + 1);
            }
        });
        
        onceAgainBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            TextAdventureGameController.Instance.StartGame(TextAdventureGameController.Instance.CurGamingModel.CurrentStageLevel);
        });
    }

    public void RefreshView()
    {
        winObj.SetActive(TextAdventureGameController.Instance.CurGamingModel.GameResult == GameResult.Win);
        loseObj.SetActive(TextAdventureGameController.Instance.CurGamingModel.GameResult == GameResult.Lose);
        playTimeTmp.text = TimeSpan.FromSeconds(TextAdventureGameController.Instance.CurGamingModel.StageLevelRecordInfo.PassSeconds).ToString("mm':'ss");
        for (var i = 0; i < singleStarItems.Length; i++)
        {
            singleStarItems[i].RefreshView(TextAdventureGameController.Instance.CurGamingModel.StageLevelRecordInfo.CollectStar > i, true);
        }

        if (TextAdventureGameController.Instance.CurGamingModel.GameResult == GameResult.Win)
        {
            AudioManager.Instance.PlayOneShot(AudioManager.SoundEffectType.Win);
            var isFirstPass = false;
            if (StageManager.Instance.StageInfoModel.StageLevelRecordInfos.ContainsKey(TextAdventureGameController
                    .Instance
                    .CurGamingModel.CurrentStageLevel))
            {
                isFirstPass = StageManager.Instance.StageInfoModel.StageLevelRecordInfos[TextAdventureGameController
                    .Instance
                    .CurGamingModel.CurrentStageLevel].ChallengeTimes <= 1;
            }
            
            rewardCoinCount.text = !isFirstPass ? "10" :
                TextAdventureGameController.Instance.CurGamingModel.StageLevelRecordInfo.CollectStar switch
            {
                0 => "20",
                1 => "30",
                2 => "40",
                _ => "50",
            };   
        }
    }
}
