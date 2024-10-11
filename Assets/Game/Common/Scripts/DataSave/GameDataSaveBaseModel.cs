public class GameDataSaveBaseModel
{
    public int BestScore { get; set; }

    public int StageLevel { get; set; }

    public int Coin { get; set; }

    public GameStateSaveBaseModel CurGameStateSaveBaseModel { get; set; }

    public void SetGameState(GameStateSaveBaseModel gameStateSaveBaseModel)
    {
        CurGameStateSaveBaseModel = gameStateSaveBaseModel;
        if (gameStateSaveBaseModel.CurScore > BestScore)
        {
            BestScore = gameStateSaveBaseModel.CurScore;
        }
    }
}

public class GameStateSaveBaseModel
{
    public int CurScore { get; set; }
}

/// <summary>
/// 2048小游戏保存model
/// </summary>
public class Game2048StateModel : GameStateSaveBaseModel
{
    public int[,] tailValues = new int[4, 4];
}

public class Game2048SaveModel : GameStateSaveBaseModel
{
    public Game2048StateModel CurGameState { get; set; }
}


