using System;
using System.Collections.Generic;
using GameFrame;
using Newtonsoft.Json;
using UnityEngine;

public class GameDataSaveHelper : Singleton<GameDataSaveHelper>
{
    private Dictionary<GameType, GameDataSaveBaseModel> allGameData = new();

    private Dictionary<GameType, Stack<GameStateSaveBaseModel>> allGameStates = new();

    public Action<GameType> OnGameDataUpdate;

    #region 游戏数据

    public T GetGameData<T>(GameType gameType) where T : GameDataSaveBaseModel, new()
    {
        if (allGameData.TryGetValue(gameType, out var value))
        {
            return (T)value;
        }

        var tempModel = JsonConvert.DeserializeObject<T>(PlayerPrefs.GetString(gameType.ToString(), ""));
        tempModel ??= new T();
        allGameData[gameType] = tempModel;
        
        return tempModel;
    }

    private void UpdateGameData<T>(GameType gameType, GameStateSaveBaseModel gameStateSaveBaseModel) where T : GameDataSaveBaseModel, new()
    {
        // 更新最高分数
        var tempData = GetGameData<T>(gameType);
        tempData.SetGameState(gameStateSaveBaseModel);
        Debug.LogError("玩家数据更新");
        OnGameDataUpdate?.Invoke(gameType);
        SaveGameData(gameType);
    }

    public void UpDataScore<T>(GameType gameType, int score) where T : GameDataSaveBaseModel, new()
    {
        var tempGameData = GetGameData<T>(gameType);
        if (tempGameData == null)
        {
            Debug.LogError("玩家数据为空");
            return;
        }

        tempGameData.CurScore += score;
        if (tempGameData.BestScore < tempGameData.CurScore)
        {
            tempGameData.BestScore = tempGameData.CurScore;
        }
        OnGameDataUpdate?.Invoke(gameType);
        SaveGameData(gameType);
    }

    #endregion
    
    #region 游戏状态相关

    public Stack<GameStateSaveBaseModel> GetGameState(GameType gameType)
    {
        if (allGameStates.TryGetValue(gameType, out var state))
        {
            return state;
        }

        state = new Stack<GameStateSaveBaseModel>();
        allGameStates[gameType] = state;
        return state;
    }

    public void AddState<T>(GameType gameType, T gameStateSaveBaseModel) where T : GameStateSaveBaseModel, new()
    {
        if (allGameStates.TryGetValue(gameType, out var state))
        {
            state.Push(gameStateSaveBaseModel);
            SaveGameData(gameType);
            return;
        }

        state = new Stack<GameStateSaveBaseModel>();
        allGameStates[gameType] = state;
        state.Push(gameStateSaveBaseModel);
        SaveGameData(gameType);
    }

    public T PopGameState<T>(GameType gameType) where T : GameStateSaveBaseModel
    {
        if (allGameStates.TryGetValue(gameType, out var temp))
        {
            if (temp.TryPop(out var model))
            {
                SaveGameData(gameType);
                return (T)model;
            }
        }
        return null;
    }
    
    public T PeekGameState<T>(GameType gameType) where T : GameStateSaveBaseModel
    {
        if (allGameStates.TryGetValue(gameType, out var temp))
        {
            if (temp.TryPeek(out var model))
            {
                return (T)model;
            }
        }
        return null;
    }

    public void ClearGameState(GameType gameType)
    {
        if (allGameStates.TryGetValue(gameType, out var temp))
        {
            temp.Clear();
        }

        if (allGameData.TryGetValue(gameType, out var tempData))
        {
            allGameData[gameType].CurScore = 0;
        }
        
        SaveGameData(gameType);
    }

    public void SaveGameData(GameType gameType)
    {
        GameDataSaveBaseModel model = null;
        if (allGameData.TryGetValue(gameType, out var tempModel))
        {
            model = tempModel;
        }

        if (model != null)
        {
            if (allGameStates.TryGetValue(gameType, out var state))
            {
                if (state.Count > 0)
                {
                    model.SetGameState(state.Peek());
                }
                else
                {
                    model.CurGameStateSaveBaseModel = null;
                }
            }
            else
            {
                model.CurGameStateSaveBaseModel = null;
            }
            
            var tempJson = JsonConvert.SerializeObject(model, Formatting.Indented);
            PlayerPrefs.SetString(gameType.ToString(), tempJson);
        }
    }

    #endregion
}
