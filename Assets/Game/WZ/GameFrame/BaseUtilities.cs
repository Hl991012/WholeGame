using System;
using System.Collections.Generic;
using BlockEliminateGame;
using NMNH.Utility;

public class BaseUtilities
{
    public static void PlayCommonClick()
    {
        AudioManager.Instance.PlayOneShot(AudioManager.SoundEffectType.CommonClick);
        VibrateHelper.VibrateLight();
    }

    /// <summary>
    /// 返回值毫秒
    /// </summary>
    public static long GetClientMillisecondTimestamp()
    {
        return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
    }

    public static DateTime UtcTimeStampToLocalDateTime(long unixMillisecondTime)
    {
        return TimeZoneInfo
            .ConvertTime(DateTimeOffset.FromUnixTimeMilliseconds(unixMillisecondTime), TimeZoneInfo.Local).DateTime;
    }
    
    public static void DealRewards(List<GameModel.RewardModel> rewardModels)
    {
        if (rewardModels is null)
        {
            return;
        }
        foreach (var rewardModel in rewardModels)
        {
            DealReward(rewardModel);
        }
    }

    public static void DealRewards(List<GameModel.RewardModel> rewardModels, float factor)
    {
        if (rewardModels is null)
        {
            return;
        }

        foreach (var rewardModel in rewardModels)
        {
            switch (rewardModel.type)
            {
                case GameModel.RewardModel.RewardType.Coin:
                case GameModel.RewardModel.RewardType.Booster:
                    rewardModel.quantity = (int)(rewardModel.quantity * factor);
                    break;
                case GameModel.RewardModel.RewardType.InfinityStaminaBuff:
                    rewardModel.duration *= factor;
                    break;
                default:
                    break;
            }

            DealReward(rewardModel);
        }
    }
    
    public static void DealReward(GameModel.RewardModel rewardModel)
    {
        switch (rewardModel.type)
        {
            case GameModel.RewardModel.RewardType.Coin:
                CurrencyManager.AddCoin(rewardModel.quantity);
                break;
            case GameModel.RewardModel.RewardType.Booster:
                BlockEliminateStageManager.Instance.AddBooster(rewardModel.boosterType, rewardModel.quantity);
                break;
            case GameModel.RewardModel.RewardType.InfinityStaminaBuff:
                // GameCenter.Instance.StaminaManager.AddInfinityBuff(rewardModel.duration);
                break;
        }
    }
}