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
        CurScore = 0;
        CircleConctrol.Instance.StartGame();
    }

    public void Revive()
    {
        CircleConctrol.Instance.Revive();
    }

    public void EndGame()
    {
        CircleConctrol.Instance.Reset();
        CurScore = 0;
    }

    public int CalculateRewardCoinCount()
    {
        var tempCoinCount = 0;
        if (score <= 20) tempCoinCount = 20;
        else
        {
            tempCoinCount = 20 + score / 5;
        }
        return tempCoinCount;
    }
}
