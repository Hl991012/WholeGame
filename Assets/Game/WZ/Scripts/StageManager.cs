using System;
using System.Collections.Generic;
using Game.Save;
using GameFrame;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    #region 数据存储相关内容

    private SaveKey<StageInfoModel> SaveKey = new (nameof(StageManager));

    private StageInfoModel tempModel;
    
    public StageInfoModel StageInfoModel => tempModel ??= SaveKey.Load(new StageInfoModel());
    
    public void Save()
    {
        SaveKey.Save(StageInfoModel);
    }

    #endregion

    public void Init()
    {
        RecoverStamina();
    }
    
    public void StagePass()
    {
        var isFirstPass = false;
        if (StageInfoModel.StageLevelRecordInfos.ContainsKey(GameCenter.Instance.CurGamingModel.CurrentStageLevel))
        {
            var stageLevelRecordInfo = StageInfoModel.StageLevelRecordInfos[GameCenter.Instance.CurGamingModel.CurrentStageLevel];
            if (GameCenter.Instance.CurGamingModel.StageLevelRecordInfo.PassSeconds < stageLevelRecordInfo.PassSeconds)
            {
                stageLevelRecordInfo.PassSeconds = GameCenter.Instance.CurGamingModel.StageLevelRecordInfo.PassSeconds;
            }
            if (GameCenter.Instance.CurGamingModel.StageLevelRecordInfo.CollectStar > stageLevelRecordInfo.CollectStar)
            {
                stageLevelRecordInfo.CollectStar = GameCenter.Instance.CurGamingModel.StageLevelRecordInfo.CollectStar;
            }
        }
        else
        {
            isFirstPass = true;
            StageInfoModel.StageLevelRecordInfos[GameCenter.Instance.CurGamingModel.CurrentStageLevel] = 
                GameCenter.Instance.CurGamingModel.StageLevelRecordInfo;
        }

        if (GameCenter.Instance.CurGamingModel.CurrentStageLevel >= StageInfoModel.CurStageLevel)
        {
            StageInfoModel.CurStageLevel++;
        }
        
        var addCoinCount = !isFirstPass ? 10 : 
            GameCenter.Instance.CurGamingModel.StageLevelRecordInfo.CollectStar switch
        {
            0 => 20,
            1 => 30,
            2 => 40,
            _ => 50,
        };
        WealthManager.Instance.AddCoin(addCoinCount);
    }

    public int CurStageLevel => StageInfoModel?.CurStageLevel ?? 1;
    
    public StageLevelRecordInfo GetStageLevelRecordInfo(int level) => StageInfoModel?.StageLevelRecordInfos?.GetValueOrDefault(level);
    
    #region 关卡体力值相关内容

    public int Stamina => StageInfoModel.StaminaCount;
    
    public long StaminaStartRecoverTs => StageInfoModel.StaminaStartRecoverTs;
    
    public Action OnStaminaValueChanged;
    
    private IDisposable timer;
    
    public void ChangeStamina(int value)
    {
        if (StageInfoModel.StaminaCount >= 10)
        {
            StageInfoModel.StaminaStartRecoverTs = BaseUtilities.GetClientMillisecondTimestamp();
        }
        
        StageInfoModel.StaminaCount = Mathf.Max(0, StageInfoModel.StaminaCount + value);
        
        if (StageInfoModel.StaminaCount >= 10)
        {
            timer?.Dispose();
            timer = null;
        }
        
        if (StageInfoModel.StaminaCount < 10 && timer == null)
        {
            timer = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ =>
            {
                if (BaseUtilities.GetClientMillisecondTimestamp() - StageInfoModel.StaminaStartRecoverTs >= 10)
                {
                    var tempValue = (int)((BaseUtilities.GetClientMillisecondTimestamp() - StageInfoModel.StaminaStartRecoverTs) / 10);
                    tempValue = Mathf.Min(tempValue, 10 - StageInfoModel.StaminaCount);
                    tempValue = tempValue <= 0 ? 0 : tempValue;
                    StageInfoModel.StaminaStartRecoverTs += tempValue * 60000;
                    ChangeStamina(tempValue);
                }
            });
        }
        
        OnStaminaValueChanged?.Invoke();
    }
    
    private void RecoverStamina()
    {
        // 恢复体力值
        if (StageInfoModel.StaminaCount < 10)
        {
            var tempValue = (int)((BaseUtilities.GetClientMillisecondTimestamp() - StageInfoModel.StaminaStartRecoverTs) / 10);
            tempValue = Mathf.Min(tempValue, 10 - StageInfoModel.StaminaCount);
            tempValue = tempValue <= 0 ? 0 : tempValue;
            StageInfoModel.StaminaStartRecoverTs += tempValue * 60000;
            ChangeStamina(tempValue);
        }
        else
        {
            timer?.Dispose();
            timer = null;
        }
    }
    
    public void BuyStamina()
    {
        
    }
    
    #endregion
}

[Serializable]
public class StageInfoModel
{
    [JsonProperty("cur_stage_level")] public int CurStageLevel { get; set; } = 1;
    
    [JsonProperty("stage_record")]
    public Dictionary<int, StageLevelRecordInfo> StageLevelRecordInfos { get; set; } = new();
    
    [JsonProperty("stamina_start_recover_ts")] public long StaminaStartRecoverTs { get; set; }
    
    [JsonProperty("stamina_count")] public int StaminaCount { get; set; } = 10;
}

[Serializable]
public class StageLevelRecordInfo
{
    [JsonProperty("level")] public int Level { get; set; }
    [JsonProperty("collect_star")] public int CollectStar { get; set; }
    [JsonProperty("pass_seconds")] public int PassSeconds { get; set; }
}
