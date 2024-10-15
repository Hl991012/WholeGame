using System;
using System.Collections.Generic;
using GameFrame;
using Newtonsoft.Json;
using UnityEngine;

public class ComboManager : MonoSingleton<ComboManager>
{
    public void Init()
    {
        allComboStates =
            JsonConvert.DeserializeObject<Dictionary<GameType, ComboStateModel>>(
                PlayerPrefs.GetString(nameof(ComboManager), ""));
        allComboStates ??= new Dictionary<GameType, ComboStateModel>();
    }
    
    private Dictionary<GameType, ComboStateModel> allComboStates;

    public Action<GameType, ComboStateModel, Vector3, int> OnComboStateChanged;

    public int AddPlayerOperate(GameType gameType, int addComboCount, Vector3 pos)
    {
        var addScoreCount = 0;
        
        if (!allComboStates.ContainsKey(gameType))
        {
            allComboStates[gameType] = new ComboStateModel()
            {
                ComboCount = 0,
                ComboKeepRemainCount = 0,
                EnterComboOperateRemainCount = GetEnterComboOperateCount(gameType),
            };
        }

        var tempState = allComboStates[gameType];
        if (addComboCount > 0)
        {
            if (tempState.ComboCount > 0)
            {
                tempState.ComboCount += addComboCount;
                tempState.EnterComboOperateRemainCount = 0;
                tempState.ComboKeepRemainCount = GetComboDefaultKeepCount(gameType);
            }
            else
            {
                tempState.EnterComboOperateRemainCount--;
                if (tempState.EnterComboOperateRemainCount <= 0)
                {
                    tempState.ComboCount += addComboCount;
                    tempState.EnterComboOperateRemainCount = 0;
                    tempState.ComboKeepRemainCount = GetComboDefaultKeepCount(gameType);
                    
                }
            }

            addScoreCount = GetComboAddScoreConfig(tempState.ComboCount);
            OnComboStateChanged?.Invoke(gameType, tempState, pos, addScoreCount);
        }
        else
        {
            tempState.ComboKeepRemainCount--;
            if (tempState.ComboKeepRemainCount <= 0)
            {
                tempState.ComboKeepRemainCount = 0;
                tempState.ComboCount = 0;
                tempState.EnterComboOperateRemainCount = GetEnterComboOperateCount(gameType);
                addScoreCount = GetComboAddScoreConfig(tempState.ComboCount);
                OnComboStateChanged?.Invoke(gameType, tempState, pos, addScoreCount);
            }
        }
        
        Debug.LogError("Combo" + tempState.ComboCount);
        Save();

        return addScoreCount;
    }

    public void ResetComboState(GameType gameType)
    {
        if (allComboStates.ContainsKey(gameType))
        {
            allComboStates[gameType] = new ComboStateModel()
            {
                ComboCount = 0,
                ComboKeepRemainCount = 0,
                EnterComboOperateRemainCount = GetEnterComboOperateCount(gameType),
            };
        }
    }

    private void Save()
    {
        if (allComboStates?.Count > 0)
        {
            var tempJson = JsonConvert.SerializeObject(allComboStates);
            PlayerPrefs.SetString(nameof(ComboManager), tempJson);   
        }
    }
    

    #region 得到默认配置

    private int GetComboDefaultKeepCount(GameType gameType)
    {
        switch (gameType)
        {
            case GameType.PutBlockGame:
                return 3;
        }
        return int.MaxValue;
    }

    private int GetEnterComboOperateCount(GameType gameType)
    {
        switch (gameType)
        {
            case GameType.PutBlockGame:
                return 2;
        }
        return 1;
    }

    private int GetComboAddScoreConfig(int comboCount)
    {
        return comboCount * 10;
    }
    
    #endregion
}

public class ComboStateModel
{
    public int ComboCount { get; set;}
    public int ComboKeepRemainCount { get; set; }
    public int EnterComboOperateRemainCount { get; set; }
}
