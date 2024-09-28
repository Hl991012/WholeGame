using System;
using GameFrame;
using UnityEngine;

public class UIPresenter : MonoSingleton<UIPresenter>
{
    [SerializeField] private MainUI mainUI;
    [SerializeField] private GameUI gameUI;
    [SerializeField] private ResultUI resultUI;
    [SerializeField] private ReviveUI reviveUI;
    [SerializeField] private PopUpsUI popUpsUI;

    private void Start()
    {
        GameCenter.Instance.OnGameStateChanged += OnGameStateChanged;
        OnGameStateChanged();
    }

    private void OnGameStateChanged()
    {
        mainUI.gameObject.SetActive(GameCenter.Instance.CurGameState == GameCenter.GameState.Home);
        gameUI.gameObject.SetActive(GameCenter.Instance.CurGameState == GameCenter.GameState.Playing ||
                                    GameCenter.Instance.CurGameState == GameCenter.GameState.Reviving ||
                                    GameCenter.Instance.CurGameState == GameCenter.GameState.Settlement);
        resultUI.gameObject.SetActive(GameCenter.Instance.CurGameState == GameCenter.GameState.Settlement);
        reviveUI.gameObject.SetActive(GameCenter.Instance.CurGameState == GameCenter.GameState.Reviving);
        switch (GameCenter.Instance.CurGameState)
        {
            case GameCenter.GameState.Home:
                mainUI.RefreshView();
                break;
            case GameCenter.GameState.Playing:
                gameUI.RefreshView();
                break;
            case GameCenter.GameState.Settlement:
                resultUI.RefreshView();
                break;
        }
    }

    public void ShowPopUps(string des)
    {
        popUpsUI.Show(des);
    }
}
