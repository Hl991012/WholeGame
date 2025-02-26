using System;
using BlockEliminateGame;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleRewardItem : MonoBehaviour
{
    [SerializeField] private Image rewardIcon;
    [SerializeField] private TextMeshProUGUI rewardCount;

    private Sequence sequence;

    public void RefreshView(GameModel.RewardModel rewardModel)
    {
        rewardIcon.sprite = ResourcesCenter.Instance.GetRewardIcon(rewardModel);
        rewardIcon.transform.localScale = GetRewardIconScale(rewardModel.type);
        rewardIcon.preserveAspect = true;
        rewardCount.text = rewardModel.quantity.ToString();
    }
    

    private Vector3 GetRewardIconScale(GameModel.RewardModel.RewardType rewardType)
    {
        return rewardType switch
        {
            _ => Vector3.one
        };
    }
}
