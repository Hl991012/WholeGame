using System;
using GameFrame;
using UniRx;
using UnityEngine;

public class TextAdventureGameController : MonoSingleton<TextAdventureGameController>
{
    public void LoadData()
    {
        StageManager.Instance.Init();
    }

    private PlayerController curPlayer;

    public void Register(PlayerController player)
    {
        curPlayer = player;
    }

    #region 游戏状态,流程相关

    public enum GameState
    {
        Home, // 等待开始
        Playing, // 游戏中
        Settlement, // 结算中
        Reviving, // 复活中
    }
    public TextAdventureGameController.GameState CurGameState { get; private set; } = TextAdventureGameController.GameState.Home;
    public Action OnGameStateChanged;

    public void ChangeState(GameState gameState)
    {
        CurGameState = gameState;
        switch (CurGameState)
        {
            case GameState.Playing:
                ChangeGamePauseState(false);
                break;
            case GameState.Reviving:
                ChangeGamePauseState(true);
                break;
        }
        OnGameStateChanged?.Invoke();
    }

    public GamingModel CurGamingModel { get; private set; }
    
    public Action OnGameRecordChanged;

    private IDisposable gameTimer;
    
    public void StartGame(int stageLevel)
    {
        CurGamingModel = new GamingModel
        {
            CurrentStageLevel = stageLevel
        };
        CameraController.Instance.Reset();
        StageLoadPresenter.Instance.LoadStagePrefab(stageLevel);
        ChangeState(GameState.Playing);
        gameTimer?.Dispose();
        gameTimer = Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            CurGamingModel.StageLevelRecordInfo.PassSeconds++;
            OnGameRecordChanged?.Invoke();
        });
    }
    
    public void AddStar()
    {
        CurGamingModel.StageLevelRecordInfo.CollectStar++;
        OnGameRecordChanged?.Invoke();
    }
    
    public void GameDeath()
    {
        ChangeGamePauseState(true);
        if (CurGamingModel.HasRevived)
        {
            EndGame(false);
        }
        else
        {
            ChangeState(GameState.Reviving);
        }
    }

    public void Revive()
    {
        CurGamingModel.HasRevived = true;
        ChangeGamePauseState(false);
        ChangeState(GameState.Playing);
        CameraController.Instance.ForceLookPlayer();
        if (curPlayer != null)
        {
            curPlayer.Revive();
        }
        StageLoadPresenter.Instance.OnRevive();
    }
    
    public void EndGame(bool isPass, bool force = false)
    {
        gameTimer?.Dispose();
        if (force)
        {
            CurGamingModel.GameResult = GameResult.Lose;
            ChangeState(GameState.Home);
            return;
        }
        
        CurGamingModel.GameResult = isPass ? GameResult.Win : GameResult.Lose;
        if (isPass)
        {
            StageManager.Instance.StagePass();
        }
        ChangeState(GameState.Settlement);
        StageLoadPresenter.Instance.OnGameEnd();
    }

    private void ChangeGamePauseState(bool pause)
    {
        Time.timeScale = pause ? 0 : 1;
    }

    #endregion

    
    #region 数据存储相关

    public void SaveData()
    {
        StageManager.Instance.Save();
        WealthManager.Instance.Save();
    }

    #endregion
}

public enum GameResult
{
    Win,
    Lose,
}

public class GamingModel
{
    public int CurrentStageLevel { get; set; }
    public StageLevelRecordInfo StageLevelRecordInfo { get; set; } = new();
    public bool HasRevived { get; set; }

    public GameResult GameResult { get; set; }
}
