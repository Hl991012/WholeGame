using Game.Save;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using GameFrame;
using PutBlockGame;

public class BlockEliminateStageManager : Singleton<BlockEliminateStageManager>
{
    #region 保存与加载
    private SaveKey<SaveModel> SAVE_KEY = new ("BlockEliminateStageManager");

    private SaveModel saveModel => tempSaveModel ??= SAVE_KEY.Load(new());
    
    private SaveModel tempSaveModel;

    private class SaveModel
    {
        [JsonProperty("current_level")]
        public int currentLevel = 1;

        [JsonProperty("boosters")]
        public Dictionary<BoosterType, BoosterSaveModel> boosters = new()
        {
            [BoosterType.Refresh] = new BoosterSaveModel
            {
                quantity = 1,
            },
            [BoosterType.RemoveVertical] = new BoosterSaveModel
            {
                quantity = 1,
            },
            [BoosterType.RemoveHorizontal] = new BoosterSaveModel
            {
                quantity = 1,
            },
        };

        public class BoosterSaveModel
        {
            [JsonProperty("quantity")]
            public int quantity;
        }
    }

    public void Save() => SAVE_KEY.Save(saveModel);
    
    #endregion

    public bool IsPlaying { get; private set; }

    public int CurrentLevel
    {
        get => saveModel?.currentLevel ?? 1;
        private set => saveModel.currentLevel = value;
    }

    public StageConfigSO CurStageConfig => ResourcesCenter.Instance.GetStageConfig(CurrentLevel);

    public int CurrentPlayingStage { get; private set; }

    public bool AllLocalStagePassed() => CurrentLevel >= ResourcesCenter.Instance.stageConfigs.Count;
    

    public void StartNewGame()
    {
        if (AllLocalStagePassed())
        {
            if (CurrentLevel >= ResourcesCenter.Instance.stageConfigs.Count)
            {// 提示更新
                MainSceneCenter.Instance.ShowTips("关卡制作中。。。");
            }
            return;
        }

        // if (!GameCenter.Instance.StaminaManager.InfinityBuffActive
        //     && GameCenter.Instance.StaminaManager.Stamina < 1)
        // {
        //     // 提示体力不足
        //     DialogManager.GetDialog<BuyStaminaDialog>().Init(null).Show();
        //     return;
        // }
        
        CurrentPlayingStage = CurrentLevel;
        GameCenter.Instance.ChangeState(GameCenter.GameState.Game, GameType.BlockEliminateGame);
        PlayRoomPresenter.Instance.BlockEliminateGame.StartNewGame(CurStageConfig);
        MainSceneCenter.Instance.BlockEliminatePlayRoomUI.RefreshView(CurStageConfig);
        IsPlaying = true;
    }

    /// <summary>
    /// 重新挑战
    /// </summary>
    public void RestartGame()
    {
        PlayRoomPresenter.Instance.BlockEliminateGame.StartNewGame(CurStageConfig);
        MainSceneCenter.Instance.BlockEliminatePlayRoomUI.RefreshView(CurStageConfig);
        IsPlaying = true;
    }

    /// <summary>
    /// 结束游戏, 返回主页
    /// </summary>
    public void EndGame()
    {
        IsPlaying = false;
        GameCenter.Instance.ChangeState(GameCenter.GameState.Home);
    }
    

    public void StagePass()
    {
        IsPlaying = false;
        BaseUtilities.DealRewards(CurStageConfig.rewards);
        CurrentLevel++;
        Save();
    }

    #region 道具
    
    public Action OnBoosterChanged;
    
    public int GetBoosterQuantity(BoosterType type)
    {
        if (saveModel == null)
        {
            return 0;
        }

        if (saveModel.boosters.TryGetValue(type, out var value))
        {
            return value.quantity;
        }

        return 0;
    }

    public int GetBoosterPrice(BoosterType type)
    {
        switch (type)
        {
            case BoosterType.Refresh:
                return 100;
            case BoosterType.RemoveHorizontal:
                return 100;
            case BoosterType.RemoveVertical:
                return 100;
        }
        return 100;
    }

    public bool UseBooster(BoosterType type)
    {
        if (GetBoosterQuantity(type) > 0)
        {
            if (PlayRoomPresenter.Instance.BlockEliminateGame.UseBooster(type))
            {
                if (type is BoosterType.RemoveHorizontal or BoosterType.RemoveHorizontal)
                {
                    PlayRoomPresenter.Instance.BlockEliminateGame.ShowUseBoosterHint(type);
                    return true;
                }
                ConsumeBooster(type);
                Save();
                return true;
            }
        }

        return false;
    }

    public void ConsumeBooster(BoosterType type)
    {
        if (GetBoosterQuantity(type) < 1)
        {
            return;
        }
        saveModel.boosters[type].quantity--;
        OnBoosterChanged?.Invoke();
        Save();
    }

    public void AddBooster(BoosterType type, int count)
    {
        saveModel.boosters[type].quantity += count;
        OnBoosterChanged?.Invoke();
        Save();
    }
    
    #endregion
}
