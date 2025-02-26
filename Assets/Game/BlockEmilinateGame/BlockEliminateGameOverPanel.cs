using System;
using System.Collections.Generic;
using BlockEliminateGame;
using PutBlockGame;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class BlockEliminateGameOverPanel : MonoBehaviour
{
    [SerializeField] private Button sureBtn;
    [SerializeField] private Button shareBtn;
    [SerializeField] private TextMeshProUGUI levelTmp;
    [SerializeField] private SingleRewardItem[] rewardItems;

    private List<GameModel.RewardModel> rewardModels;
    
    private void Awake()
    {
        sureBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            gameObject.SetActive(false);
            BlockEliminateStageManager.Instance.EndGame();
            MainSceneCenter.Instance.GetRewardUIPresenter.Init(rewardModels).Show();
        });
        
        shareBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            WXSDKManager.Instance.Share();
        });
    }

    public void Show()
    {
        rewardModels = BlockEliminateStageManager.Instance.CurStageConfig.rewards;
        levelTmp.text = (BlockEliminateStageManager.Instance.CurrentLevel).ToString();
        PlayRoomPresenter.Instance.BlockEliminateGame.SetInputEnable(false);
        Observable.Timer(TimeSpan.FromSeconds(1.5f))
            .Subscribe(_ =>
            {
                PlayRoomPresenter.Instance.BlockEliminateGame.SetInputEnable(true);
                WXSDKManager.Instance.ShowInterstitialVideo(null);
                gameObject.SetActive(true);
                
                // 刷新奖励
                if (BlockEliminateStageManager.Instance.CurStageConfig != null &&
                    BlockEliminateStageManager.Instance.CurStageConfig.rewards is { Count: > 0 })
                {
                    for (var i = 0; i < rewardItems.Length; i++)
                    {
                        rewardItems[i].gameObject.SetActive(i < BlockEliminateStageManager.Instance.CurStageConfig.rewards.Count);

                        if (i < BlockEliminateStageManager.Instance.CurStageConfig.rewards.Count)
                        {
                            rewardItems[i].RefreshView(BlockEliminateStageManager.Instance.CurStageConfig.rewards[i]);
                        }
                    }
                }
            }).AddTo(this);
    }
}
