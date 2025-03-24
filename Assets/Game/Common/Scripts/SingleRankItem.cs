using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleRankItem : EnhancedScrollerCellView
{
    [SerializeField] private TextMeshProUGUI rankTmp;
    [SerializeField] private TextMeshProUGUI scoreTmp;
    [SerializeField] private TextMeshProUGUI nameTmp;
    [SerializeField] private RawImage avatarImg;
    [SerializeField] private Image rankIconImg;
    [SerializeField] private List<Sprite> rankIcon;
    
    public void RefreshView(int rank, WXCloudManager.SingleRankInfo singleRankInfo)
    {
        if (rank - 1 >= 0 && rank < rankIcon.Count)
        {
            rankIconImg.gameObject.SetActive(false);
            rankTmp.text = "";
            rankIconImg.sprite = rankIcon[rank - 1];
        }
        else
        {
            rankIconImg.gameObject.SetActive(false);
            rankTmp.text = rank.ToString();
        }
        
        if (singleRankInfo == null)
        {
            scoreTmp.text = "虚位以待";
            return;
        }
        
        scoreTmp.text = singleRankInfo.Score.ToString();
        nameTmp.text = singleRankInfo.Name;
    }
}
