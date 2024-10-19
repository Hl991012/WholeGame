using Newtonsoft.Json;
using GameFrame;
using UnityEngine;

public class DrawLineGameConctrol : MonoSingleton<DrawLineGameConctrol>
{ 
    public DrawLineGameRoomPresenter DrawLineGameRoomPresenter { get; private set; }

    public DrawLineGameRoomUIPresenter DrawLineGameRoomUIPresenter { get; private set; }

    public void Register(DrawLineGameRoomPresenter drawLineGameRoomPresenter)
    {
        DrawLineGameRoomPresenter = drawLineGameRoomPresenter;
    }

    public void Register(DrawLineGameRoomUIPresenter drawLineGameRoomUIPresenter)
    {
        DrawLineGameRoomUIPresenter = drawLineGameRoomUIPresenter;
    }

    private MapData curMapData;

    public MapData CurMapData => curMapData;

    public int GameLevel
    {
        get => PlayerPrefs.GetInt(nameof(GameLevel), 1);
        set => PlayerPrefs.SetInt(nameof(GameLevel), value);
    }
    
    public void Win()
    {
        GameLevel++;
        GameLevel = Mathf.Clamp(GameLevel, 0, 800);
    }

    public MapData LoadConfig()
    {
        var level = GameLevel - 1;
        var temp1 = level / 40;
        var temp2 = level % 40;
        var path = $"Config/lp{temp1}/lvl{temp2}";
        curMapData = JsonConvert.DeserializeObject<MapData>(Resources.Load<TextAsset>(path).text);

        return curMapData;
    }
}
