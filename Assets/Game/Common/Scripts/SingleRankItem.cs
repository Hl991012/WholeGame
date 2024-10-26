using TMPro;
using UnityEngine;

public class SingleRankItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankTmp;
    [SerializeField] private TextMeshProUGUI scoreTmp;
    
    public void RefreshView(int rank, WXCloudManager.SingleRankInfo singleRankInfo)
    {
        rankTmp.text = $"第{rank}名";
        
        if (singleRankInfo == null)
        {
            scoreTmp.text = "虚位以待";
            return;
        }
        
        scoreTmp.text = singleRankInfo.Score.ToString();
    }
}
