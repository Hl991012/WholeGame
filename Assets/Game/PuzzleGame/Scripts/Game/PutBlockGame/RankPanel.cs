using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.UI;

public class RankPanel : MonoBehaviour, IEnhancedScrollerDelegate
{
    [SerializeField] private EnhancedScroller rankScroller;
    [SerializeField] private SingleRankItem singleRankItem;
    [SerializeField] private Button closeBtn;
    [SerializeField] private SingleRankItem selfRankItem;
    

    private List<WXCloudManager.SingleRankInfo> curRankInfos = new (100);

    private void Awake()
    {
        closeBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            gameObject.SetActive(false);
        });

        rankScroller.Delegate = this;
    }

    public void Show(List<WXCloudManager.SingleRankInfo> rankInfos, WXCloudManager.SingleRankInfo selfInfo)
    {
        curRankInfos.Clear();
        curRankInfos.AddRange(rankInfos);
        for (var i = curRankInfos.Count; i < 100; i++)
        {
            curRankInfos.Add(null);
        }
        
        selfRankItem.RefreshView(selfInfo.Rank, selfInfo);
        
        rankScroller.ReloadData();
    }
    
    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return curRankInfos.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 100;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        var temp = scroller.GetCellView(singleRankItem);
        if (temp is SingleRankItem)
        {
            (temp as SingleRankItem).RefreshView(dataIndex + 1, curRankInfos[dataIndex]);
        }

        return temp;
    }
}
