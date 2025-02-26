using System;

namespace BlockEliminateGame
{
    public class GameModel
    {
        public enum TargetType
        {
            Score = 0,
            Barrier = 1,
        }
        
        public enum BarrierType
        {
            Score = 9999,
            None = 0,
            BlueDiamond1 = 1, // 钻石
            Lawn = 2, // 草
            Donut = 3, // 甜甜圈
            Clover = 4, // 叶子
            BandAid = 5, // 创可贴
            Heart = 6, // 爱心
            Flower = 7, // 花
            BlueDiamond2 = 8, // 钻石
            Ice = 9, // 冰
        }
        
        [Serializable]
        public class StageTargetItemModel
        {
            public TargetType TargetType;
            public BarrierType BarrierType;
            public int TargetCount;
        }
        
        [Serializable]
        public class RewardModel
        {
            public enum RewardType
            {
                None = 0,
                Coin = 1,
                Booster = 2,
                InfinityStaminaBuff = 3,
            }

            public RewardType type;
            public BoosterType boosterType;
            public int quantity = 1;
            public float duration;
        }
    }
}

