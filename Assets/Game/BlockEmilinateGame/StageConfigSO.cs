using System;
using System.Collections.Generic;
using BlockEliminateGame;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(StageConfigSO), menuName = "SO/" + nameof(StageConfigSO))]
public class StageConfigSO : ScriptableObject
{
    public GameModel.TargetType TargetType;

    public SerializableDictionary<GameModel.BarrierType, GameModel.StageTargetItemModel> Targets;
    
    public List<GameModel.BarrierType> extraBarriers;
    
    public List<GameModel.RewardModel> rewards;
}