using System;
using Game.Save;
using GameFrame;
using Newtonsoft.Json;

public class CircleGameManager : Singleton<CircleGameManager>
{
    #region 保存与加载
    private SaveKey<SaveModel> SAVE_KEY = new ("CircleGameManager");

    private SaveModel saveModel => tempSaveModel ??= SAVE_KEY.Load(new());

    private SaveModel tempSaveModel;

    private class SaveModel
    {
        [JsonProperty("max_score")]
        public int maxScore;
    }
    private void Save() => SAVE_KEY.Save(saveModel);
        
    #endregion

    public int MaxScore => saveModel.maxScore;

    private int score;
    public int CurScore
    {
        get => score;
        set
        {
            score = value;
            if (score > saveModel.maxScore)
            {
                saveModel.maxScore = score;
                Save();
            }
            OnScoreChanged?.Invoke();
        }
    }

    public event Action OnScoreChanged;

    public void StartGame()
    {
        score = 0;
        CircleConctrol.Instance.StartGame();
    }

    public void Revive()
    {
        CircleConctrol.Instance.Revive();
    }

    public void EndGame()
    {
        CircleConctrol.Instance.Reset();
    }
}
