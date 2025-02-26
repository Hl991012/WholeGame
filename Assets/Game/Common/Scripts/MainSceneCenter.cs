using System;
using BlockEliminateGame;
using GameFrame;
using PutBlockGame;
using TMPro;
using UniRx;
using UnityEngine;
using WeChatWASM;

public class MainSceneCenter : MonoSingleton<MainSceneCenter>
{
    [field: SerializeField] public MainUIPresenter MainUIPresenter { get; set; }
    [field: SerializeField] public BlockEliminatePlayRoomUI BlockEliminatePlayRoomUI { get; set; }
    [SerializeField] private PlayRoomPresenter playRoomPresenter;
    [SerializeField] private PopUpsUI popUpsUI;
    [SerializeField] private TMP_FontAsset tmpFontAsset;
    [field: SerializeField] public BuyGoodsUIPresenter BuyGoodsUIPresenter { get; set; }
    [field: SerializeField] public GetRewardUIPresenter GetRewardUIPresenter { get; set; }

    private void Awake()
    {
        GameCenter.Instance.onGameStateChanged += OnGameStateChanged;
        
        WX.GetWXFont("", font =>
        {
            tmpFontAsset.fallbackFontAssetTable.Add(TMP_FontAsset.CreateFontAsset(font));
        });
    }

    private void Start()
    {
        GameCenter.Instance.ChangeState(GameCenter.GameState.Home);
        WXSDKManager.Instance.ShowInterstitialVideo(null);
        
        WXSDKManager.Instance.ShowCustomAd1();

        Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(3))
            .TakeWhile(_ => !WXSDKManager.Instance.IsShowBanner)
            .Subscribe(_ =>
            {
                WXSDKManager.Instance.ShowCustomAd();
                // WXSDKManager.Instance.ShowCustomAd1();
            }).AddTo(this);
    }

    private void OnGameStateChanged()
    {
        MainUIPresenter.gameObject.SetActive(GameCenter.Instance.CurGameState == GameCenter.GameState.Home);
        playRoomPresenter.gameObject.SetActive(GameCenter.Instance.CurGameState == GameCenter.GameState.Game);
        switch (GameCenter.Instance.CurGameState)
        {
            case GameCenter.GameState.Home:
                MainUIPresenter.RefreshView();
                WXSDKManager.Instance.ShowCustomAd1();
                break;
            case GameCenter.GameState.Game:
                playRoomPresenter.RefreshView();
                WXSDKManager.Instance.CloseCustomAd1();
                break;
        }
    }

    public void ShowTips(string tips)
    {
        popUpsUI.Show(tips);
    }
}
