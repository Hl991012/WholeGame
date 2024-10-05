using System;
using GameFrame;
using UnityEngine;

public class GameCenter : MonoSingleton<GameCenter>
{
    public enum GameState
    {
        Home, // 等待开始
        Game,
    }

    public GameType CurGameType { get; private set; }

    public GameState CurGameState { get; private set; } = GameState.Home;
    public Action OnGameStateChanged;

    public void ChangeState(GameState gameState, GameType gameType = GameType.None)
    {
        CurGameType = gameType;
        CurGameState = gameState;
        switch (CurGameState)
        {
            case GameState.Home:
                CameraController.Instance.Reset();
                break;
            case GameState.Game:
                Time.timeScale = 1;
                switch (CurGameType)
                {
                    case GameType.TextAdventure:
                        TextAdventureGameController.Instance.ChangeState(TextAdventureGameController.GameState.Home);
                        break;
                }
                break;
        }
        OnGameStateChanged?.Invoke();
    }
}

public enum GameType
{
    None,
    PutBlockGame, // 放置方块
    TextAdventure, // 文字冒险
    DrawLineGame, // 一笔画成
}
