using System;
using UnityEngine;

public class MainSceneCenter : MonoBehaviour
{
    [SerializeField] private UIPresenter uiPresenter;
    [SerializeField] private PlayRoomPresenter playRoomPresenter;

    private void Awake()
    {
        GameCenter.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void Start()
    {
        OnGameStateChanged();
    }

    // private void Update()
    // {
    //     if (!WXSDKManager.Instance.IsShowBanner)
    //     {
    //         WXSDKManager.Instance.ShowCustomAd();
    //     }
    // }

    private void OnGameStateChanged()
    {
        uiPresenter.gameObject.SetActive(GameCenter.Instance.CurGameState == GameCenter.GameState.Home);
        playRoomPresenter.gameObject.SetActive(GameCenter.Instance.CurGameState == GameCenter.GameState.Game);
        switch (GameCenter.Instance.CurGameState)
        {
            case GameCenter.GameState.Home:
                break;
            case GameCenter.GameState.Game:
                playRoomPresenter.RefreshView();
                break;
        }
    }
}
