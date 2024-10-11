using System;
using System.Collections.Generic;
using PuzzleGame;

public enum GameType
{
    None,
    PutBlockGame, // 放置方块
    PutBlockGameStage, // 放置方块关卡模式
    TextAdventure, // 文字冒险
    DrawLineGame, // 一笔画成
    Game2048, // 2048
    Minesweeper, // 扫雷
}

public static class GameTypeExtension
{
    #region 游戏数据相关

    public static T GetGameData<T>(this GameType gameType) where T : GameDataSaveBaseModel, new()
    {
        return GameDataSaveHelper.Instance.GetGameData<T>(gameType);
    }
    
    

    #endregion
    
    #region 游戏状态相关内容

    public static void SaveGameData<T>(this GameType gameType) where T : GameDataSaveBaseModel, new()
    {
        GameDataSaveHelper.Instance.SaveGameData<T>(gameType);
    }
    
    public static Stack<GameStateSaveBaseModel> GetGameStates(this GameType gameType)
    {
        return GameDataSaveHelper.Instance.GetGameState(gameType);
    }
    
    public static void AddGameState(this GameType gameType, GameStateSaveBaseModel gameStateSaveBaseModel)
    {
        GameDataSaveHelper.Instance.AddState(gameType, gameStateSaveBaseModel);
    }

    public static T PopGameState<T>(this GameType gameType) where T : GameStateSaveBaseModel
    {
        return GameDataSaveHelper.Instance.PopGameState<T>(gameType);
    }

    public static T PeekGameState<T>(this GameType gameType) where T : GameStateSaveBaseModel
    {
        return GameDataSaveHelper.Instance.PeekGameState<T>(gameType);
    }

    public static void ClearGameState(this GameType gameType)
    {
        GameDataSaveHelper.Instance.ClearGameState(gameType);
    }

    #endregion
}