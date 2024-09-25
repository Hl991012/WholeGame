using System;
using NMNH.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
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
            GameCenter.Instance.ChangeState(GameCenter.GameState.Home);
        });
        
        nextBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            if (GameCenter.Instance.CurGamingModel.CurrentStageLevel + 1 > StageConfigManager.Instance.TotalStageCount)
            {
                UIPresenter.Instance.ShowPopUps("更多关卡制作中~");
            }
            else
            {
                GameCenter.Instance.StartGame(GameCenter.Instance.CurGamingModel.CurrentStageLevel + 1);
            }
        });
        
        onceAgainBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            GameCenter.Instance.StartGame(GameCenter.Instance.CurGamingModel.CurrentStageLevel);
        });
    }

    public void RefreshView()
    {
        winObj.SetActive(GameCenter.Instance.CurGamingModel.GameResult == GameResult.Win);
        loseObj.SetActive(GameCenter.Instance.CurGamingModel.GameResult == GameResult.Lose);
        playTimeTmp.text = TimeSpan.FromSeconds(GameCenter.Instance.CurGamingModel.StageLevelRecordInfo.PassSeconds).ToString("mm':'ss");
        for (var i = 0; i < singleStarItems.Length; i++)
        {
            singleStarItems[i].RefreshView(GameCenter.Instance.CurGamingModel.StageLevelRecordInfo.CollectStar > i, true);
        }

        if (GameCenter.Instance.CurGamingModel.GameResult == GameResult.Win)
        {
            AudioManager.Instance.PlayOneShot(AudioManager.SoundEffectType.Win);
            var isFirstPass =
                !StageManager.Instance.StageInfoModel.StageLevelRecordInfos.ContainsKey(GameCenter.Instance
                    .CurGamingModel.CurrentStageLevel);
            
            rewardCoinCount.text = !isFirstPass ? "10" :
                GameCenter.Instance.CurGamingModel.StageLevelRecordInfo.CollectStar switch
            {
                0 => "20",
                1 => "30",
                2 => "40",
                _ => "50",
            };   
        }
    }
}
