using System;
using GameFrame;
using Newtonsoft.Json;

namespace NumDrawLine
{
    public class NumDrawLineGameManager : Singleton<NumDrawLineGameManager>
    {
        public Action OnScoreUpdate;
        
        private SaveModel saveModel;
        public SaveModel Data
        {
            get
            {
                saveModel = JsonConvert.DeserializeObject<SaveModel>(
                    PlayerPrefs.GetString(nameof(NumDrawLineGameManager)));
                saveModel ??= new SaveModel();
                return saveModel;
            }

            private set => saveModel = value;
        }

        public void AddScore(int score)
        {
            Data.CurScore += score;
            if (Data.CurScore > Data.TopScore)
            {
                Data.TopScore = Data.CurScore;
            }
            OnScoreUpdate?.Invoke();
            SaveToLocal();
        }

        public void ResetGameSaveInfo()
        {
            Data.MapData = null;
            Data.CurScore = 0;
            SaveToLocal();
            OnScoreUpdate?.Invoke();
        }
        
        public void UpdateGameMapData(int[,] mapData)
        {
            Data.MapData = mapData;
            SaveToLocal();
        }

        private void SaveToLocal()
        {
            var tempJson = JsonConvert.SerializeObject(saveModel);
            PlayerPrefs.SetString(nameof(NumDrawLineGameManager), tempJson);
        }
        
        public class SaveModel
        {
            public int TopScore { get; set; }
            public int CurScore { get; set; }
            public int[,] MapData { get; set; }
        }
    }   
}
