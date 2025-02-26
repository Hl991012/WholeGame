using TMPro;
using UnityEngine;

public class SingleStageItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageLevelTmp;
    
    public void RefreshView(int stageLevel)
    {
        stageLevelTmp.text = stageLevel >= ResourcesCenter.Instance.stageConfigs.Count ? "?" : stageLevel.ToString();
    }
}
