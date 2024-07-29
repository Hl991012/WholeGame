using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleStageLevelItem : MonoBehaviour
{
    [SerializeField] private Button clickBtn;
    [SerializeField] private TextMeshProUGUI levelTmp;
    [SerializeField] private TextMeshProUGUI passBestTimeTmp;
    [SerializeField] private SingleStarItem[] stars;
    [SerializeField] private GameObject unlockObj; 
    [SerializeField] private GameObject lockObj; 
    [SerializeField] private GameObject passBestTimeObj;
    [SerializeField] private GameObject waitingChallengeObj;
    private StageConfigModel curStageConfigModel;

    private bool isUnlocked;

    private void Awake()
    {
        clickBtn.onClick.AddListener(() =>
        {
            BaseUtilities.PlayCommonClick();
            GameCenter.Instance.StartGame(curStageConfigModel.Level);
        });
    }

    public SingleStageLevelItem Init(StageConfigModel stageConfigModel)
    {
        curStageConfigModel = stageConfigModel;
        isUnlocked = StageManager.Instance.CurStageLevel >= curStageConfigModel.Level;
        return this;
    }

    public void RefreshView()
    {
        if(curStageConfigModel == null) return;

        levelTmp.text = $"第{curStageConfigModel.Level}关";

        var stageRecordInfo = StageManager.Instance.GetStageLevelRecordInfo(curStageConfigModel.Level);
        if (stageRecordInfo != null)
        {
            passBestTimeTmp.text = stageRecordInfo.PassSeconds > 0
                ? TimeSpan.FromSeconds(stageRecordInfo.PassSeconds).ToString("mm':'ss") : string.Empty;
            
            for (var i = 0; i < stars.Length; i++)
            {
                stars[i].RefreshView(i < stageRecordInfo.CollectStar, false);
            }
        }
        else
        {
            foreach (var t in stars)
            {
                t.RefreshView(false, false);
            }
        }
        passBestTimeObj.SetActive(stageRecordInfo != null);
        waitingChallengeObj.SetActive(stageRecordInfo == null);
        
        unlockObj.SetActive(isUnlocked);
        lockObj.SetActive(!isUnlocked);
    }
}
