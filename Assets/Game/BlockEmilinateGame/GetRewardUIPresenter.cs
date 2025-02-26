using System;
using System.Collections.Generic;
using BlockEliminateGame;
using UnityEngine;
using UnityEngine.UI;

public class GetRewardUIPresenter : MonoBehaviour
{
    [SerializeField] private MultipleRewardBarPresenter multipleRewardBarPresenter;
    [SerializeField] private Button normalGetBtn;
    [SerializeField] private Button adGetBtn;
    [SerializeField] private Transform rewardParent;

    private List<GameModel.RewardModel> rewardModels = new ();

    private Action OnClose;

    private int getCoinRewardCount = 0;

    private void Awake()
    {
        normalGetBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            gameObject.SetActive(false);
        });
        
        adGetBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            multipleRewardBarPresenter.PauseAnimation();
            WXSDKManager.Instance.ShowRewardVideo(isSuccess =>
            {
                if (isSuccess)
                {
                    BaseUtilities.DealRewards(rewardModels, multipleRewardBarPresenter.CurrentRewardMultiple.Value);
                    gameObject.SetActive(false);
                }
                else
                {
                    multipleRewardBarPresenter.PlayAnimation();
                }
            });
        });
    }

    public GetRewardUIPresenter Init(GameModel.RewardModel rewardModel, Action onClose = null)
    {
        rewardModels.Clear();
        if (rewardModel != null)
        {
            rewardModels.Add(rewardModel);   
        }
        OnClose = onClose;
        return this;
    }

    public GetRewardUIPresenter Init(List<GameModel.RewardModel> models, Action onClose = null)
    {
        rewardModels.Clear();
        if (models != null)
        {
            rewardModels.AddRange(models);   
        }
        OnClose = onClose;
        return this;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        if (rewardModels is { Count: > 0 })
        {
            rewardParent.gameObject.SetActive(true);
            for (var i = 0; i < rewardModels.Count; i++)
            {
                var rewardModel = rewardModels[i];
                var rewardItem = i < rewardParent.childCount
                    ? rewardParent.GetChild(i).GetComponent<SingleRewardItem>()
                    : Instantiate(rewardParent.GetChild(0).gameObject, rewardParent).GetComponent<SingleRewardItem>();
                rewardItem.gameObject.SetActive(true);

                if (rewardModel.type == GameModel.RewardModel.RewardType.Coin)
                {
                    getCoinRewardCount += rewardModel.quantity;
                }
                
                rewardItem.RefreshView(rewardModel);
            }

            for (var i = rewardModels.Count; i < rewardParent.childCount; i++)
            {
                rewardParent.GetChild(i).gameObject.SetActive(false);
            }
        }
        else
        {
            rewardParent.gameObject.SetActive(false);
        }
    }
}
