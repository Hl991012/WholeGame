using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutBlockRankPanel : MonoBehaviour
{
    [SerializeField] private SingleRankItem[] rankItems;

    private List<WXCloudManager.SingleRankInfo> curRankInfos;

    public PutBlockRankPanel Init(List<WXCloudManager.SingleRankInfo> rankInfos)
    {
        curRankInfos = rankInfos;
        return this;
    }
    
    public void RefreshView()
    {
        if (curRankInfos is { Count: > 0 })
        {
            gameObject.SetActive(true);
            for (var i = 0; i < rankItems.Length; i++)
            {
                if (i < curRankInfos.Count)
                {
                    rankItems[i].RefreshView(i + 1, curRankInfos[i]);
                }
                else
                {
                    rankItems[i].RefreshView(i + 1, null);
                }
            }
        }
        else
        {
            gameObject.SetActive(false);
            MainSceneCenter.Instance.ShowTips("功能维护中。。。");
        }
    }
}
