using System.Collections.Generic;
using BlockEliminateGame;
using GameFrame;
using UnityEngine;

public class ResourcesCenter : MonoSingleton<ResourcesCenter>
{
    [SerializeField] private SerializableDictionary<GameModel.BarrierType, GameObject> barriers;
    [SerializeField] private SerializableDictionary<GameModel.BarrierType, Sprite> barrierIcons;
    [SerializeField] private SerializableDictionary<string, Sprite> rewardSprites;
    
    public List<StageConfigSO> stageConfigs;

    public StageConfigSO GetStageConfig(int level)
    {
        if (level < 0 || level >= stageConfigs.Count)
        {
            return null;
        }

        return stageConfigs[level];
    }

    public GameObject GetBarrierByType(GameModel.BarrierType barrierType)
    {
        if (barriers.TryGetValue(barrierType, out var type))
        {
            return type;
        }
        
        return null;
    }
    
    public Sprite GetRewardIcon(GameModel.RewardModel rewardModel)
    {
        var tempStr = "null";
        switch (rewardModel.type)
        {
            case GameModel.RewardModel.RewardType.Coin:
                tempStr = "Coin";
                break;
            case GameModel.RewardModel.RewardType.Booster:
                tempStr = rewardModel.boosterType.ToString();
                break;
        }
        
        if (rewardSprites.TryGetValue(tempStr, out var icon))
        {
            return icon;
        }
        return null;
    }

    public Sprite GetBarrierIcon(GameModel.BarrierType barrierType)
    {
        if (barrierIcons.TryGetValue(barrierType, out var icon))
        {
            return icon;
        }
        return null;
    }
}
