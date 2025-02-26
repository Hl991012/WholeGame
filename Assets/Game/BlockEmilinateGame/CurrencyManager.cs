using Game.Save;
using Newtonsoft.Json;
using System;

public static class CurrencyManager
    {
        #region 保存与加载
        private static SaveKey<SaveModel> SAVE_KEY = new ("CurrencyManager");

        private static SaveModel saveModel => tempSaveModel ??= SAVE_KEY.Load(new());

        private static SaveModel tempSaveModel;

        private class SaveModel
        {
            [JsonProperty("coin")]
            public int coin;
        }

        public static void Save() => SAVE_KEY.Save(saveModel);
        
        #endregion

        public static int Coin
        {
            get => saveModel.coin;
            private set => saveModel.coin = value;
        }

        public static event Action OnWealthChanged;
        
        public static bool CostCoin(int count)
        {
            if (Coin < count)
            {
                return false;
            }

            Coin -= count;
            OnWealthChanged?.Invoke();
            Save();
            return true;
        }

        public static void AddCoin(int coin)
        {
            Coin += coin;
            OnWealthChanged?.Invoke();
            Save();
        }
    }