using GameFrame;
using PuzzleGame.Gameplay.Bricks2048;
using PuzzleGame.Gameplay.Puzzle1010;
using UnityEngine;

public class PlayRoomPresenter : MonoSingleton<PlayRoomPresenter>
{
    [SerializeField] private PutBlockGameController putBlockGameController;
    [SerializeField] private TextAdventureGameController textAdventureGameController;
    [SerializeField] private DrawLineGameRoomPresenter drawLineGameRoomPresenter;
    [SerializeField] private GameObject game2048Prefab;
    [SerializeField] private X2BlocksGameController x2BlocksGameController;
    
    public void RefreshView()
    {
        putBlockGameController.gameObject.SetActive(GameCenter.Instance.CurGameType == GameType.PutBlockGame);
        textAdventureGameController.gameObject.SetActive(GameCenter.Instance.CurGameType == GameType.TextAdventure);
        drawLineGameRoomPresenter.gameObject.SetActive(GameCenter.Instance.CurGameType == GameType.DrawLineGame);
        game2048Prefab.gameObject.SetActive(GameCenter.Instance.CurGameType == GameType.Game2048);
        x2BlocksGameController.gameObject.SetActive(GameCenter.Instance.CurGameType == GameType.X2BlockGame);
        switch (GameCenter.Instance.CurGameType)
        {
            case GameType.PutBlockGame:
                // 创建游戏
                break;
            case GameType.TextAdventure:
                textAdventureGameController.ChangeState(TextAdventureGameController.GameState.Home);
                break;
            case GameType.DrawLineGame:
                drawLineGameRoomPresenter.StartGame();
                break;
            case GameType.Game2048:
                break;
            case GameType.X2BlockGame:
                x2BlocksGameController.PlayGame();
                break;
        }
    }
}
