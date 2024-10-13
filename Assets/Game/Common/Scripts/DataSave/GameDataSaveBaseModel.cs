using Newtonsoft.Json;

public class GameDataSaveBaseModel
{
    public int BestScore { get; set; }
    
    public int CurScore { get; set; }

    public int StageLevel { get; set; }

    public int Coin { get; set; }

    [JsonConverter(typeof(CustomJsonConvert))]  // 使用自定义转换器
    public GameStateSaveBaseModel CurGameStateSaveBaseModel { get; set; }

    public void SetGameState(GameStateSaveBaseModel gameStateSaveBaseModel)
    {
        CurGameStateSaveBaseModel = gameStateSaveBaseModel;
    }
}

public class GameStateSaveBaseModel
{
    
}

/// <summary>
/// 2048小游戏保存model
/// </summary>
public class Game2048StateModel : GameStateSaveBaseModel
{
    public int[,] tailValues = new int[4, 4];
}

public class Game2048SaveModel : GameDataSaveBaseModel
{
    public Game2048StateModel CurGameState { get; set; }
}


