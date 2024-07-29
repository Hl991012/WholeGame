using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

public class StageLevelGroup : EnhancedScrollerCellView
{
    [SerializeField] private SingleStageLevelItem[] stageLevelItems;

    public void Init(List<StageConfigModel> stageLevelRecordInfos)
    {
        for (int i = 0; i < stageLevelItems.Length; i++)
        {
            stageLevelItems[i].gameObject.SetActive(i < stageLevelRecordInfos.Count);
            if (i < stageLevelRecordInfos.Count)
            {
                stageLevelItems[i].Init(stageLevelRecordInfos[i]).RefreshView();

            }
        }
    }
}
