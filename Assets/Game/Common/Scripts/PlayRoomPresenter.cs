using GameFrame;
using PuzzleGame.Gameplay.Puzzle1010;
using UnityEngine;

public class PlayRoomPresenter : MonoSingleton<PlayRoomPresenter>
{
    [SerializeField] private PutBlockGameController putBlockGameController;
    [SerializeField] private TextAdventureGameController textAdventureGameController;
    
    public void RefreshView()
    {
        putBlockGameController.gameObject.SetActive(GameCenter.Instance.CurGameType == GameType.PutBlockGame);
        textAdventureGameController.gameObject.SetActive(GameCenter.Instance.CurGameType == GameType.TextAdventure);
        switch (GameCenter.Instance.CurGameType)
        {
            case GameType.PutBlockGame:
                // 创建游戏
                break;
            case GameType.TextAdventure:
                textAdventureGameController.ChangeState(TextAdventureGameController.GameState.Home);
                break;
        }
    }
}